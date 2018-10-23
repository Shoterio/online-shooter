﻿using System;
using System.Collections.Generic;
using GameServer.Game;
using GameServer.MapObjects;
using GameServer.Models;
using GameServer.States;

namespace GameServer.Physics
{
    public class PhysicsEngine
    {
        private GameEngine _gameEngine;
        public PhysicsEngine(GameEngine gameEngine)
        {
            _gameEngine = gameEngine;
        }

        public void ApplyPhysics()
        {
            foreach (Player player in GameState.Instance.Players)
            {
                player.Speed *= 1 - Config.PLAYER_DECCELERATION;

                var speedVector = player.Speed + GetSpeedFromPlayerInput(player);

                UpdatePlayerPosition(player, speedVector);
            }
        }

        public static Vector2 GetSpeedFromPlayerInput(Player player)
        {
            Vector2 calculatedSpeedVector = new Vector2();

            // First get direction
            if (player.Keys.Contains(KeyEnum.Up))
                calculatedSpeedVector += Vector2.UP_VECTOR;
            if (player.Keys.Contains(KeyEnum.Down))
                calculatedSpeedVector += Vector2.DOWN_VECTOR;
            if (player.Keys.Contains(KeyEnum.Left))
                calculatedSpeedVector += Vector2.LEFT_VECTOR;
            if (player.Keys.Contains(KeyEnum.Right))
                calculatedSpeedVector += Vector2.RIGHT_VECTOR;

            // Scale vector to be speed length
            if (!calculatedSpeedVector.IsDegenerated())
            {
                var normalizedSpeed = calculatedSpeedVector.Normalize();
                var vectorLength = Config.PLAYER_SPEED / Config.SERVER_TICK;
                calculatedSpeedVector = normalizedSpeed * vectorLength;
            }

            return calculatedSpeedVector;
        }

        public void UpdatePlayerPosition(Player player, Vector2 speedVector)
        {
            var movementVector = CalculatePossibleMovementVector(player, speedVector, out double spareLength);
            player.Position += movementVector;
            player.Speed = movementVector;

            if (spareLength > 0)
            {
                var spareSpeedVector = speedVector.Normalize() * spareLength;

                var horizontalVector = Physic.ProjectVector(spareSpeedVector, Vector2.LEFT_VECTOR);
                var verticalVector = Physic.ProjectVector(spareSpeedVector, Vector2.UP_VECTOR);
                
                var parallelHorizontalMovementVector = CalculatePossibleMovementVector(player, horizontalVector, out double horizontalSpareLength);
                var parallelVerticalMovementVector = CalculatePossibleMovementVector(player, verticalVector, out double verticalSpareLength);

                var parallelMovementVector = parallelVerticalMovementVector;
                if (horizontalSpareLength < verticalSpareLength)
                    parallelMovementVector = parallelHorizontalMovementVector;

                player.Position += parallelMovementVector;
                player.Speed = parallelMovementVector;
            }
        }

        public Vector2 CalculatePossibleMovementVector(Player player, Vector2 speedvector, out double spareLength)
        {
            spareLength = 0;
            if (speedvector.IsDegenerated())
                return Vector2.ZERO_VECTOR;

            // Variables used to calculate speed vector
            var offset = 0d;
            var speedVectorLength = speedvector.Length();
            var speedVectorNormalized = speedvector.Normalize();
            var currentPrecision = speedVectorLength / 2d;
            MapObject intersectionObject = null;

            do
            {
                // Create moved sphere
                var checkPosition = player.Position + (speedVectorNormalized * (offset + currentPrecision));

                // Check for intersection
                var validationObject = new MapCircle(checkPosition, player.Radius);
                intersectionObject = CheckAnyIntersectionWithWorld(validationObject);

                // Update new position and offset
                if (intersectionObject == null) // No object found, increase offset
                    offset += currentPrecision;

                currentPrecision /= 2.0;

            } // Do this as long as we reach desired precision
            while (currentPrecision >= Config.INTERSECTION_INTERVAL);

            spareLength = speedVectorLength - offset;
            return speedVectorNormalized * offset;
        }
        
        public MapObject CheckAnyIntersectionWithWorld(MapCircle s)
        {
            // Check intersection with all map objects
            foreach (MapObject obj in MapState.Instance.MapObjects)
            {
                bool intersects = false;
                if (obj is MapRect)
                    intersects = Intersection.CheckIntersection((MapRect)obj, s);
                else if (obj is MapCircle)
                    intersects = Intersection.CheckIntersection((MapCircle)obj, s);

                if (intersects)
                    return obj;
            }

            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using GameServer.Hubs;
using GameServer.MapObjects;
using GameServer.Models;
using GameServer.Physics;
using GameServer.States;
using GameServer.World;
using Microsoft.AspNetCore.SignalR;

namespace GameServer.Game
{
    public interface IGameEngine
    {
        bool AddPlayer(Player player);

        bool RemovePlayer(Player player);

        GameState GameState { get; }
    }

    public class GameEngine : IGameEngine
    {
        public Timer Ticker;
        private GameState _gameState = GameState.Instance;
        public GameEvents GameEvents;
        public PhysicsEngine PhysicsEngine;
        private readonly IHubContext<GameHub> _hubContext;
        private IConfig _config;
        private static int bulletId = 1;
        private Random random;

        GameState IGameEngine.GameState { get => _gameState; }

        public GameEngine(IHubContext<GameHub> hubContext, IConfig config, IWorldLoader worldLoader, IMapState mapState)
        {
            _hubContext = hubContext;
            _config = config;
            GameEvents = new GameEvents(this);
            PhysicsEngine = new PhysicsEngine(config, mapState);
            worldLoader.LoadMap();
            random = new Random();
            Ticker = new Timer(Tick, null, 0, _config.ServerTickMilliseconds);
        }

        private void ApplyShooting()
        {
            foreach (Player player in GameState.Instance.Players)
            {
                var weapon = WeaponService.GetWeaponFromWeaponEnumOrNull(player.PlayerWeapon.WeaponEnum);

                if (player.MouseClicked && MilisecondsSince(player.PlayerWeapon.LastShotDate) > weapon.ShootTime)
                {
                    //Shoot
                    player.PlayerWeapon.LastShotDate = DateTime.Now;

                    var bullet = new Bullet()
                    {
                        Id = bulletId++,
                        PlayerId = player.Id,
                        Position = player.Position,
                        Radius = weapon.BulletSize,
                        Speed = player.Speed + (Vector2.RadianToVector2(player.Angle + Math.PI/2) * (weapon.BulletSpeed / _config.ServerTicksPerSecond))
                    };

                    GameState.Instance.Bullets.Add(bullet);
                }
            }
        }

        private static double MilisecondsSince(DateTime time)
        {
            return (DateTime.Now - time).TotalMilliseconds;
        }

        public void PerformSingleTick()
        {
            Tick(null);
        }

        private void Tick(object state)
        {
            var stopwatch = Stopwatch.StartNew();

            foreach (var player in _gameState.Players)
            {
                player.Keys.Clear();
                player.Angle = (random.NextDouble() * 2 * Math.PI) - Math.PI;
                player.MouseClicked = true;

                var pressedKeys = random.Next(2); // Pressed 0, 1 or 2 keys
                for(int i = 0; i < pressedKeys; i++)
                {
                    // Keys from 1 to 4 (up, down, left, right)
                    player.Keys.Add((KeyEnum) (random.Next(4) + 1));
                }

                if (!player.IsAlive)
                {
                    player.MilisecondsToResurect -= _config.ServerTickMilliseconds;
                    if (player.MilisecondsToResurect <= 0)
                    {
                        SpawnService.SpawnPlayer(player);
                        player.Health = _config.MaxPlayerHealth;
                    }
                }
            }

            ApplyShooting();

            PhysicsEngine.ApplyPhysics();

            ApplyDamage();

            // Send game state for every connected client
            _hubContext?.Clients.All.SendAsync("updateGameState", GameState.Instance);

            stopwatch.Stop();
            try
            {
                File.AppendAllText("wwwroot\\ticks.txt", $"{stopwatch.ElapsedTicks}{Environment.NewLine}");
            }
            catch(Exception)
            {

            }
        }

        private void ApplyDamage()
        {
            var bulletHitIds = new List<int>();
            foreach (var bullet in _gameState.Bullets)
            {
                var hit = false;
                if (_config.Interpolation)
                {
                    var previousPosition = bullet.Position - (bullet.Speed * (1.0 / _config.BulletDecceleraionFactorPerTick));
                    var stopwatch = Stopwatch.StartNew();
                    hit = CheckBulletInterpolatedHit(bullet, previousPosition);
                    stopwatch.Stop();

                    try
                    {
                        File.AppendAllText("wwwroot\\interpolation.txt", $"{stopwatch.ElapsedTicks}{Environment.NewLine}");
                    }
                    catch(Exception)
                    {

                    }

                }
                else
                    hit = CheckBulletHitAtPosition(bullet, bullet.Position);

                if(hit)
                    bulletHitIds.Add(bullet.Id);
            }

            GameState.Instance.Bullets.RemoveAll(b => bulletHitIds.Contains(b.Id));
        }

        private bool CheckBulletInterpolatedHit(Bullet bullet, Vector2 previousPosition)
        {
            var shiftVector = bullet.Position - previousPosition;
            var length = shiftVector.Length();

            for (var i = 0.0; i < length; i += _config.IntersectionInterval)
            {
                var interpolatedPosition = previousPosition + (shiftVector.Normalize() * i);

                if (CheckBulletHitAtPosition(bullet, interpolatedPosition))
                    return true;
            }

            return false;
        }

        private bool CheckBulletHitAtPosition(Bullet bullet, Vector2 position)
        {
            var bulletCopy = new Bullet(bullet)
            {
                Position = position
            };

            var colidedPlayer = CheckAnyIntersectionWithPlayers(bulletCopy);
            if (colidedPlayer != null)
            {
                var attacker = _gameState.Players.First(x => x.Id == bulletCopy.PlayerId);
                var damage = attacker.PlayerWeapon.GetWeapon().BulletDamage;
                DamagePlayer(attacker, colidedPlayer, damage);

                return true;
            }

            return false;
        }

        private void DamagePlayer(Player attacker, Player victim, int damage)
        {
            victim.Health -= damage;

            if(victim.Health <= 0)
            {
                victim.IsAlive = false;
                victim.MilisecondsToResurect = _config.MilisecondsToResurect;
                
                _hubContext.Clients.All.SendAsync("playerKilled", new object[] { attacker.Name, victim.Name });
            }
        }

        private Player CheckAnyIntersectionWithPlayers(Bullet bullet)
        {
            foreach (var player in _gameState.Players)
            {
                if (bullet.PlayerId != player.Id && player.IsAlive && Intersection.CheckIntersection(new MapCircle(bullet.Position, bullet.Radius),
                        new MapCircle(player.Position, player.Radius)))
                {
                    return player;
                }
            }
            return null;
        }

        public bool AddPlayer(Player player)
        {
            GameState.Instance.Players.Add(player);

            GameEvents.OnPlayerAdded(player);

            return true;
        }

        public bool RemovePlayer(Player player)
        {
            GameEvents.OnPlayerRemoved(player);

            GameState.Instance.Players.Remove(player);

            return true;
        }
    }
}

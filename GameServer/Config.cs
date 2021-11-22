using GameServer.Models;
using System;

namespace GameServer
{
    public interface IConfig
    {
        int BufferSize { get; set; }
        int ClientStateIntervalMilliseconds { get; set; }
        double PlayerRadius { get; set; }
        double IntersectionInterval { get; set; }
        double MinBulletSpeed { get; set; }
        int MaxPlayerHealth { get; set; }
        bool Extrapolation { get; set; }
        bool Interpolation { get; set; }
        int ServerTickMilliseconds { get; set; }
        double ServerTicksPerSecond { get; set; }
        double PlayerMaxSpeedPerTick { get; set; }
        double PlayerDeccelerationFactorPerTick { get; set; }
        double PlayerSpeedPerTick { get; set; }
        double BulletDecceleraionFactorPerTick { get; set; }
        int MilisecondsToResurect { get; set; }
    }

    public class Config : IConfig
    {
        public Config()
        {
            BufferSize = 4 * 1024;
            ClientStateIntervalMilliseconds = 50;
            PlayerRadius = 16;
            IntersectionInterval = 0.01;
            MinBulletSpeed = 0.1;
            MaxPlayerHealth = 100;
            Extrapolation = true;
            Interpolation = true;
            MilisecondsToResurect = 2000;

            ServerTickMilliseconds = 50;
            ServerTicksPerSecond = 1000.0 / ServerTickMilliseconds;
            
            PlayerSpeedPerTick = 100.0 / ServerTicksPerSecond;
            PlayerDeccelerationFactorPerTick = Math.Pow(0.1, 1.0 / ServerTicksPerSecond);
            BulletDecceleraionFactorPerTick = Math.Pow(0.5, 1.0 / ServerTicksPerSecond);
        }

        public int BufferSize { get; set; }
        public int ClientStateIntervalMilliseconds { get; set; }
        public double PlayerRadius { get; set; }
        public double IntersectionInterval { get; set; }
        public double MinBulletSpeed { get; set; }
        public int MaxPlayerHealth { get; set; }
        public bool Extrapolation { get; set; }
        public bool Interpolation { get; set; }
        public int MilisecondsToResurect { get; set; }

        public int ServerTickMilliseconds { get; set; }
        public double ServerTicksPerSecond { get; set; }
        public double PlayerMaxSpeedPerTick { get; set; }
        public double PlayerDeccelerationFactorPerTick { get; set; }
        public double PlayerSpeedPerTick { get; set; }
        public double BulletDecceleraionFactorPerTick { get; set; }
    }
}

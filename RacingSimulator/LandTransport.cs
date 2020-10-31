using System;
using System.Collections.Generic;
using System.Text;

namespace RacingSimulator
{
    public interface ITransport
    {
        public RaceType Type { get;}
        double Speed { get; set; }
        public TimeSpan CalcTime(double distance);
    }
    public class LandTransport : ITransport
    {
        protected double speed;
        protected TimeSpan movingTime;
        protected TimeSpan restingTime;
        protected Random rnd = new Random();
        public double Speed
        {
            get => this.speed;
            set => this.speed = value;
        }
        public TimeSpan MovingTime
        {
            get => this.movingTime;
            set => this.movingTime = value;
        }
        public TimeSpan RestingTime
        {
            get => this.restingTime;
            set => this.restingTime = value;
        }
        public RaceType Type { get => RaceType.Land; }
        protected virtual double CalcRestTime(int intervals, double restTime)
        {
            if (intervals == 0)
            {
                return restTime;
            }
            return restTime + CalcRestTime(intervals - 1, restTime + 30);
        }
        public TimeSpan CalcTime(double distance)
        {
            if (distance <= 0)
            {
                return new TimeSpan(0, 0, 0);
            }
            double timeValue = distance / Speed;
            int intervals = (int)((timeValue * 60 * 60) / MovingTime.TotalSeconds);
            if (intervals != 0)
            {
                //Add resting time (+30sec every time)
                timeValue += CalcRestTime(intervals - 1, RestingTime.TotalSeconds) / 60 / 60;
            }

            int hours = (int)Math.Round(timeValue);
            int minutes = (int)Math.Round(timeValue * 60) - hours * 60;
            int seconds = (int)Math.Round(timeValue * 60 * 60) - hours * 60 * 60 - minutes * 60;
            return new TimeSpan(hours, minutes, seconds);
        }
    }
    public class BactrianCamel : LandTransport
    {
        private static int idCount = 0;
        private int id;
        public BactrianCamel()
        {
            this.id = BactrianCamel.idCount;
            BactrianCamel.idCount++;
            this.speed = Math.Round(rnd.NextDouble() + rnd.Next(55, 75), 2);
            this.movingTime = new TimeSpan(0, 10, 0);
            this.restingTime = new TimeSpan(0, 15, 30);
        }
        protected override double CalcRestTime(int intervals, double restTime)
        {
            if (intervals == 0)
            {
                return restTime;
            }
            return restTime + CalcRestTime(intervals - 1, restTime + 30);
        }
        public override string ToString()
        {
            return "Bactrian Camel with id: " + this.id;
        }
    }
    public class SpeedCamel : LandTransport
    {
        private static int idCount = 0;
        private int id;
        public SpeedCamel()
        {
            this.id = SpeedCamel.idCount;
            SpeedCamel.idCount++;
            this.speed = Math.Round(rnd.NextDouble() + rnd.Next(90, 120), 2);
            this.movingTime = new TimeSpan(0, 7, 0);
            this.restingTime = new TimeSpan(0, 15, 00);
        }
        protected override double CalcRestTime(int intervals, double restTime)
        {
            if (intervals == 0)
            {
                return restTime;
            }
            return restTime + CalcRestTime(intervals - 1, restTime + 50);
        }
        public override string ToString()
        {
            return "Speed Camel with id: " + this.id;
        }
    }
    public class Centaur : LandTransport
    {
        private static int idCount = 0;
        private int id;
        public Centaur()
        {
            this.id = Centaur.idCount;
            Centaur.idCount++;
            this.speed = Math.Round(rnd.NextDouble() + rnd.Next(90, 120), 2);
            this.movingTime = new TimeSpan(0, 7, 0);
            this.restingTime = new TimeSpan(0, 15, 00);
        }
        protected override double CalcRestTime(int intervals, double restTime)
        {
            if (intervals == 0)
            {
                return restTime;
            }
            return restTime + CalcRestTime(intervals - 1, restTime + 45);
        }
        public override string ToString()
        {
            return "Centaur with id: " + this.id;
        }
    }
    public class Boots : LandTransport
    {
        private static int idCount = 0;
        private int id;
        public Boots()
        {
            this.id = Boots.idCount;
            Boots.idCount++;
            this.speed = Math.Round(rnd.NextDouble() + rnd.Next(90, 120), 2);
            this.movingTime = new TimeSpan(0, 7, 0);
            this.restingTime = new TimeSpan(0, 15, 00);
        }
        protected override double CalcRestTime(int intervals, double restTime)
        {
            if (intervals == 0)
            {
                return restTime;
            }
            return restTime + CalcRestTime(intervals - 1, restTime + 60);
        }
        public override string ToString()
        {
            return "Boots with id: " + this.id;
        }
    }
}

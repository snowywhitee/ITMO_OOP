using System;
using System.Collections.Generic;
using System.Text;

namespace RacingSimulator
{
    public class AirTransport : ITransport
    {
        public RaceType Type { get => RaceType.Air; }
        protected double speed;
        protected double shortenDistance;
        protected Random rnd = new Random();
        public double Speed
        {
            get => this.speed;
            set => this.speed = value;
        }
        public double ShortenDistance
        {
            get => this.shortenDistance;
            set => this.shortenDistance = value;
        }
        public virtual TimeSpan CalcTime(double distance)
        {
            if (distance <= 0)
            {
                return new TimeSpan(0, 0, 0);
            }
            double timeValue = (distance - distance * (shortenDistance / 100)) / Speed;
            int hours = (int)Math.Round(timeValue);
            int minutes = (int)Math.Round(timeValue * 60) - hours * 60;
            int seconds = (int)Math.Round(timeValue * 60 * 60) - hours * 60 * 60 - minutes * 60;
            return new TimeSpan(hours, minutes, seconds);
        }
    }
    public class MagicCarpet : AirTransport
    {
        private static int idCount = 0;
        private int id;
        public MagicCarpet()
        {
            this.id = MagicCarpet.idCount;
            MagicCarpet.idCount++;
            this.speed = Math.Round(rnd.NextDouble() + rnd.Next(550, 620), 2);
            //this.shortenDistance = Math.Round(rnd.NextDouble() + rnd.Next(5, 10), 2);
        }
        public override TimeSpan CalcTime(double distance)
        {
            if (distance <= 0)
            {
                return new TimeSpan(0, 0, 0);
            }

            if (distance < 1000) ShortenDistance = 0;
            if (distance >= 1000 && distance < 5000) ShortenDistance = 3.0;
            if (distance >= 5000 && distance < 10000) ShortenDistance = 10.0;
            if (distance >= 10000) ShortenDistance = 5.0;
            
            double timeValue = (distance - distance*(shortenDistance/100)) / Speed;
            int hours = (int)Math.Round(timeValue);
            int minutes = (int)Math.Round(timeValue * 60) - hours * 60;
            int seconds = (int)Math.Round(timeValue * 60 * 60) - hours * 60 * 60 - minutes * 60;
            return new TimeSpan(hours, minutes, seconds);
        }
        public override string ToString()
        {
            return "Magic Carpet with id: " + this.id;
        }
    }
    public class Mortar : AirTransport
    {
        private static int idCount = 0;
        private int id;
        public Mortar()
        {
            this.id = Mortar.idCount;
            Mortar.idCount++;
            this.speed = Math.Round(rnd.NextDouble() + rnd.Next(220, 500), 2);
            //this.shortenDistance = Math.Round(rnd.NextDouble() + rnd.Next(3, 6), 2);
            this.shortenDistance = 6.0;

        }
        public override string ToString()
        {
            return "Mortar with id: " + this.id;
        }
    }
    public class MagicBroom : AirTransport
    {
        private static int idCount = 0;
        private int id;
        public MagicBroom()
        {
            this.id = MagicBroom.idCount;
            MagicBroom.idCount++;
            this.speed = Math.Round(rnd.NextDouble() + rnd.Next(500, 600), 2);
            this.shortenDistance = Math.Round(rnd.NextDouble() + rnd.Next(2, 3), 2);

        }
        public override TimeSpan CalcTime(double distance)
        {
            if (distance <= 0)
            {
                return new TimeSpan(0, 0, 0);
            }
            double timeValue;
            if (distance > 1000)
            {
                timeValue = (1000 - 1000 * (shortenDistance / 100)) / speed;
            }
            else
            {
                timeValue = (distance - distance * (shortenDistance / 100)) / speed;
            }
            
            shortenDistance++;
            int hours = (int)Math.Round(timeValue);
            int minutes = (int)Math.Round(timeValue * 60) - hours * 60;
            int seconds = (int)Math.Round(timeValue * 60 * 60) - hours * 60 * 60 - minutes * 60;
            return new TimeSpan(hours, minutes, seconds) + CalcTime(distance - 1000);
        }
        public override string ToString()
        {
            return "Magic Broom with id: " + this.id;
        }
    }
}

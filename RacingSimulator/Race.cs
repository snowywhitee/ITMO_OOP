using System;
using System.Collections.Generic;
using System.Text;

//Error codes:
//1 - invalid transport type (air/land)
//2 - no participants in the race (start race) / negative distance

namespace RacingSimulator
{
    public interface IRace
    {
        public void AddToRace(ITransport transport);
        public List<ITransport> StartRace(double distance);
        public void PrintResults();
    }
    public class Race : IRace
    {
        private RaceType type;
        public RaceType Type
        {
            get
            {
                return type;
            }
        }
        protected List<ITransport> list = new List<ITransport>();
        protected Dictionary<TimeSpan, ITransport> results = new Dictionary<TimeSpan, ITransport>();
        private List<ITransport> winners = new List<ITransport>();
        public Race(RaceType type)
        {
            this.type = type;
        }
        public virtual void AddToRace(ITransport transport)
        {
            if (this.type == RaceType.Air)
            {
                try
                {
                    if (transport.Type != RaceType.Air)
                    {
                        throw new LabException($"You can't add ILandTransport to te AirRace. Problem: {transport}");
                    }
                    list.Add(transport);
                }
                catch (LabException exception)
                {
                    exception.PrintExit(1);
                }
            }
            else if (this.type == RaceType.Land)
            {
                try
                {
                    if (transport.Type != RaceType.Land)
                    {
                        throw new LabException($"You can't add IAirTransport to te LandRace. Problem: {transport}");
                    }
                    list.Add(transport);
                }
                catch (LabException exception)
                {
                    exception.PrintExit(1);
                }
            }
            else if (this.type == RaceType.All)
            {
                list.Add(transport);
            }
            
        }
        public List<ITransport> StartRace(double distance)
        {
            winners.Clear();
            results.Clear();
            try
            {
                if (list.Count == 0)
                {
                    throw new LabException("No participants in the Race!");
                }
                if (distance <= 0)
                {
                    throw new LabException("Distance can't be negative or zero.");
                }
                if (list.Count == 1)
                {
                    winners.Add(list[0]);
                    return winners;
                }
                TimeSpan bestTime = list[0].CalcTime(distance);
                results.Add(bestTime, list[0]);
                for (int i = 1; i < list.Count; i++)
                {
                    TimeSpan curTime = list[i].CalcTime(distance);
                    results.Add(curTime, list[i]);
                    if (curTime < bestTime)
                    {
                        bestTime = curTime;
                    }
                }
                foreach (var item in results)
                {
                    if (item.Key == bestTime)
                    {
                        this.winners.Add(item.Value);
                    }
                }
            }
            catch (LabException exception)
            {
                exception.PrintExit(2);
            }

            return this.winners;
        }
        public void PrintResults()
        {
            Console.WriteLine("Race results:");
            foreach (var item in results)
            {
                Console.WriteLine($"{item.Key}, {item.Value}, speed: {item.Value.Speed}");
            }
        }
    }
    public enum RaceType
    {
        Air,
        Land,
        All
    }
    public class LabException : Exception
    {
        public LabException(string msg)
        : base(msg)
        {
        }
        public void PrintExit(int code)
        {
            Console.WriteLine(this.Message);
            System.Environment.Exit(code);
        }
    }
    
}

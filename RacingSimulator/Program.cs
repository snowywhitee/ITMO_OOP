using System;

namespace RacingSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            IRace race = new Race(RaceType.All);
            race.AddToRace(new MagicBroom());
            race.AddToRace(new MagicBroom());
            race.AddToRace(new MagicBroom());
            race.AddToRace(new BactrianCamel());
            var winners = race.StartRace(100);
            for (int i = 0; i < winners.Count; i++)
            {
                Console.WriteLine("Winner: " + winners[i]);
            }
            race.PrintResults();
        }
    }
    
}

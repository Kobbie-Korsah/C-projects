using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessTrackerBeta1
{
    class Workout
    {
        public string Name { get; set; }
        public int DurationMinutes { get; set; } // duration in minutes

        public Workout(string name, int durationMinutes)
        {
            Name = name;
            DurationMinutes = durationMinutes;
        }

        public override string ToString()
        {
            return $"{Name} — {DurationMinutes} min";
        }
    }

    class Program
    {
        static List<Workout> workouts = new List<Workout>();

        static void Main(string[] args)
        {
            Console.WriteLine("Fitness Tracker — Beta 1");
            while (true)
            {
                Console.WriteLine("1) Add Workout");
                Console.WriteLine("2) View Log");
                Console.WriteLine("3) Exit");
                Console.Write("Choose: ");
                var key = Console.ReadLine();
                if (key == "1") AddWorkout();
                else if (key == "2") ViewLog();
                else if (key == "3") break;
                else Console.WriteLine("Invalid option.");
            }
        }

        static void AddWorkout()
        {
            Console.Write("Workout name: ");
            string name = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Name cannot be empty.");
                return;
            }

            Console.Write("Duration (minutes): ");
            if (!int.TryParse(Console.ReadLine(), out int duration) || duration <= 0)
            {
                Console.WriteLine("Invalid duration.");
                return;
            }

            workouts.Add(new Workout(name, duration));
            Console.WriteLine("Logged: " + workouts[workouts.Count - 1]);
        }

        static void ViewLog()
        {
            if (workouts.Count == 0)
            {
                Console.WriteLine("No workouts logged yet.");
                return;
            }

            Console.WriteLine("\nWorkout Log:");
            for (int i = 0; i < workouts.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {workouts[i]}");
            }
        }
    }
}


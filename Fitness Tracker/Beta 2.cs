using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FitnessTrackerBeta2
{
    // Compared to Beta 1:
    // - Adds a Date property to each workout (Beta1 had only name and duration).
    // - Adds CSV persistence (Beta1 was session-only).
    // - Adds commands to query by date or date range (Beta1 could only view everything).
    class Workout
    {
        public DateTime Date { get; set; }   // NEW in Beta2
        public string Name { get; set; }
        public int DurationMinutes { get; set; }

        public Workout(DateTime date, string name, int durationMinutes)
        {
            Date = date;
            Name = name;
            DurationMinutes = durationMinutes;
        }

        public override string ToString()
        {
            return $"{Date:yyyy-MM-dd} | {Name} — {DurationMinutes} min";
        }

        // CSV line: yyyy-MM-dd,Name,Duration
        public string ToCsv() => $"{Date:yyyy-MM-dd},{EscapeCsv(Name)},{DurationMinutes}";
        public static string EscapeCsv(string s) => $"\"{s.Replace("\"", "\"\"")}\"";
        public static Workout FromCsv(string csvLine)
        {
            // Very simple CSV parsing expecting exactly 3 columns. Names may contain commas but are quoted.
            var parts = SplitCsv(csvLine);
            var date = DateTime.ParseExact(parts[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var name = parts[1];
            var duration = int.Parse(parts[2]);
            return new Workout(date, name, duration);
        }

        static List<string> SplitCsv(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            var cur = "";
            foreach (var ch in line)
            {
                if (ch == '"') { inQuotes = !inQuotes; cur += ch; continue; }
                if (ch == ',' && !inQuotes) { result.Add(Unquote(cur)); cur = ""; continue; }
                cur += ch;
            }
            result.Add(Unquote(cur));
            return result;
        }

        static string Unquote(string s)
        {
            s = s.Trim();
            if (s.Length >= 2 && s[0] == '"' && s[s.Length - 1] == '"')
            {
                s = s.Substring(1, s.Length - 2).Replace("\"\"", "\"");
            }
            return s;
        }
    }

    class Program
    {
        static string DataFile = "workouts_beta2.csv";
        static List<Workout> workouts = new List<Workout>();

        static void Main(string[] args)
        {
            Load();
            Console.WriteLine("Fitness Tracker — Beta 2");
            while (true)
            {
                Console.WriteLine("\n(1) Add workout");
                Console.WriteLine("(2) View all");
                Console.WriteLine("(3) View by date ");
                Console.WriteLine("(4) View by range");
                Console.WriteLine("(5) Save");
                Console.WriteLine("(6) Exit");
                Console.Write("Enter a number: ");
                var key = Console.ReadLine();
                if (key == "1") AddWorkout();
                else if (key == "2") ViewAll();
                else if (key == "3") ViewByDate();
                else if (key == "4") ViewByRange();
                else if (key == "5") Save();
                else if (key == "6") { Save(); break; }
                else Console.WriteLine("Invalid option.");
            }
        }

        static void AddWorkout()
        {
            Console.Write("Date (yyyy-MM-dd) or blank for today: ");
            var dateInput = Console.ReadLine();
            DateTime date;
            if (string.IsNullOrWhiteSpace(dateInput)) date = DateTime.Today;
            else if (!DateTime.TryParseExact(dateInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            Console.Write("Workout name: ");
            var name = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(name)) { Console.WriteLine("Name empty."); return; }

            Console.Write("Duration (minutes): ");
            if (!int.TryParse(Console.ReadLine(), out int duration) || duration <= 0) { Console.WriteLine("Invalid duration."); return; }

            workouts.Add(new Workout(date, name, duration));
            Console.WriteLine("Logged: " + workouts[workouts.Count - 1]);

        }

        static void ViewAll()
        {
            if (!workouts.Any()) { Console.WriteLine("No workouts."); return; }
            var sorted = workouts.OrderBy(w => w.Date).ThenBy(w => w.Name);
            Console.WriteLine("\nAll workouts:");
            foreach (var w in sorted) Console.WriteLine(w);
        }

        static void ViewByDate()
        {
            Console.Write("Date (yyyy-MM-dd): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                Console.WriteLine("Invalid date.");
                return;
            }
            var matches = workouts.Where(w => w.Date.Date == date.Date).OrderBy(w => w.Name).ToList();
            if (!matches.Any()) { Console.WriteLine("No workouts that day."); return; }
            Console.WriteLine($"\nWorkouts on {date:yyyy-MM-dd}:");
            foreach (var w in matches) Console.WriteLine(w);
        }

        static void ViewByRange()
        {
            Console.Write("Start date (yyyy-MM-dd): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime start)) { Console.WriteLine("Invalid date."); return; }
            Console.Write("End date (yyyy-MM-dd): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime end)) { Console.WriteLine("Invalid date."); return; }
            if (end < start) { Console.WriteLine("End before start."); return; }

            var matches = workouts.Where(w => w.Date.Date >= start.Date && w.Date.Date <= end.Date).OrderBy(w => w.Date).ThenBy(w => w.Name).ToList();
            if (!matches.Any()) { Console.WriteLine("No workouts in that range."); return; }
            Console.WriteLine($"\nWorkouts from {start:yyyy-MM-dd} to {end:yyyy-MM-dd}:");
            foreach (var w in matches) Console.WriteLine(w);
        }

        static void Load()
        {
            if (!File.Exists(DataFile)) return;
            try
            {
                var lines = File.ReadAllLines(DataFile);
                workouts = lines.Where(l => !string.IsNullOrWhiteSpace(l)).Select(Workout.FromCsv).ToList();
                Console.WriteLine($"Loaded {workouts.Count} workouts from {DataFile}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load data: " + ex.Message);
                workouts = new List<Workout>();
            }
        }

        static void Save()
        {
            try
            {
                File.WriteAllLines(DataFile, workouts.Select(w => w.ToCsv()));
                Console.WriteLine($"Saved {workouts.Count} workouts to {DataFile}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to save: " + ex.Message);
            }
        }
    }
}

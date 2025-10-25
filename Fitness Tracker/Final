using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;




namespace FitnessTrackerFinal
{
    class Workout
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public int DurationMinutes { get; set; }

        public override string ToString()
        {
            // Compatible way of taking the first 8 characters of the ID
            return "ID: " + Id.ToString().Substring(0, 8) + " | " + Date.ToString("yyyy-MM-dd") + " | " + Name + " — " + DurationMinutes + " min";
        }
    }

    class Program
    {
        private static readonly string DataFile = "workouts_final.json";
        private static List<Workout> workouts = new List<Workout>();

        static void Main()
        {
            Console.Title = "Fitness Tracker — Final Edition";
            Load();

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("      FITNESS TRACKER — FINAL EDITION      ");
            Console.WriteLine("═══════════════════════════════════════════");
            Console.ResetColor();

            while (true)
            {
                Console.WriteLine("\nMenu Options:");
                Console.WriteLine(" 1. Add Workout");
                Console.WriteLine(" 2. View All");
                Console.WriteLine(" 3. Search by Name");
                Console.WriteLine(" 4. Filter by Date Range");
                Console.WriteLine(" 5. Edit Workout");
                Console.WriteLine(" 6. Delete Workout");
                Console.WriteLine(" 7. View Stats");
                Console.WriteLine(" 8. Export to CSV");
                Console.WriteLine(" 9. Save");
                Console.WriteLine(" 0. Exit");
                Console.Write("\nSelect an option: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddWorkout(); break;
                    case "2": ViewAll(); break;
                    case "3": SearchByName(); break;
                    case "4": FilterByRange(); break;
                    case "5": EditWorkout(); break;
                    case "6": DeleteWorkout(); break;
                    case "7": ShowStats(); break;
                    case "8": ExportCsv(); break;
                    case "9": Save(); break;
                    case "0": Save(); return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid choice, please try again.");
                        Console.ResetColor();
                        break;
                }
            }
        }

        private static void AddWorkout()
        {
            Console.Write("Date (yyyy-MM-dd) or blank for today: ");
            string input = Console.ReadLine();
            DateTime date = DateTime.Today;

            if (!string.IsNullOrWhiteSpace(input) &&
                !DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid date format. Please use yyyy-MM-dd.");
                Console.ResetColor();
                return;
            }

            Console.Write("Workout name: ");
            string name = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(name))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Workout name cannot be empty.");
                Console.ResetColor();
                return;
            }

            Console.Write("Duration (minutes): ");
            int duration;
            if (!int.TryParse(Console.ReadLine(), out duration) || duration <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid duration. Must be a positive number.");
                Console.ResetColor();
                return;
            }

            Workout workout = new Workout { Date = date, Name = name, DurationMinutes = duration };
            workouts.Add(workout);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Workout added successfully: " + workout);
            Console.ResetColor();
        }

        private static void ViewAll()
        {
            if (workouts.Count == 0)
            {
                Console.WriteLine("No workouts logged yet.");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nAll Workouts:");
            Console.ResetColor();

            foreach (Workout w in workouts.OrderBy(w => w.Date))
                Console.WriteLine(w);
        }

        private static void SearchByName()
        {
            Console.Write("Enter part of workout name: ");
            string term = Console.ReadLine().Trim().ToLower();
            if (string.IsNullOrEmpty(term))
            {
                Console.WriteLine("Search term cannot be empty.");
                return;
            }

            List<Workout> results = workouts
                .Where(w => w.Name.ToLower().Contains(term))
                .OrderBy(w => w.Date)
                .ToList();

            if (results.Count == 0)
            {
                Console.WriteLine("No workouts found for that search.");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nResults (" + results.Count + "):");
            Console.ResetColor();

            foreach (Workout r in results)
                Console.WriteLine(r);
        }

        private static void FilterByRange()
        {
            Console.Write("Start date (yyyy-MM-dd): ");
            DateTime start;
            if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out start))
            {
                Console.WriteLine("Invalid start date.");
                return;
            }

            Console.Write("End date (yyyy-MM-dd): ");
            DateTime end;
            if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out end))
            {
                Console.WriteLine("Invalid end date.");
                return;
            }

            List<Workout> range = workouts
                .Where(w => w.Date >= start && w.Date <= end)
                .OrderBy(w => w.Date)
                .ToList();

            if (range.Count == 0)
            {
                Console.WriteLine("No workouts found in that date range.");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nWorkouts from " + start.ToString("yyyy-MM-dd") + " to " + end.ToString("yyyy-MM-dd") + ":");
            Console.ResetColor();

            foreach (Workout r in range)
                Console.WriteLine(r);
        }

        private static void EditWorkout()
        {
            Console.Write("Enter first 8 characters of workout ID to edit: ");
            string idPart = Console.ReadLine().Trim();
            Workout match = workouts.FirstOrDefault(w => w.Id.ToString().StartsWith(idPart, StringComparison.OrdinalIgnoreCase));

            if (match == null)
            {
                Console.WriteLine("Workout not found.");
                return;
            }

            Console.WriteLine("Editing: " + match);
            Console.Write("New date (yyyy-MM-dd) or blank to keep [" + match.Date.ToString("yyyy-MM-dd") + "]: ");
            string dateIn = Console.ReadLine();
            DateTime newDate;
            if (!string.IsNullOrWhiteSpace(dateIn) && DateTime.TryParseExact(dateIn, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out newDate))
                match.Date = newDate;

            Console.Write("New name or blank to keep [" + match.Name + "]: ");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name)) match.Name = name.Trim();

            Console.Write("New duration or blank to keep [" + match.DurationMinutes + "]: ");
            string dur = Console.ReadLine();
            int d;
            if (!string.IsNullOrWhiteSpace(dur) && int.TryParse(dur, out d) && d > 0)
                match.DurationMinutes = d;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Workout updated successfully!");
            Console.ResetColor();
        }

        private static void DeleteWorkout()
        {
            Console.Write("Enter first 8 characters of workout ID to delete: ");
            string idPart = Console.ReadLine().Trim();
            Workout match = workouts.FirstOrDefault(w => w.Id.ToString().StartsWith(idPart, StringComparison.OrdinalIgnoreCase));

            if (match == null)
            {
                Console.WriteLine("Workout not found.");
                return;
            }

            Console.WriteLine("Found: " + match);
            Console.Write("Type 'yes' to confirm delete: ");
            if (Console.ReadLine().Trim().ToLower() == "yes")
            {
                workouts.Remove(match);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Workout deleted successfully.");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("Deletion cancelled.");
            }
        }

        private static void ShowStats()
        {
            if (workouts.Count == 0)
            {
                Console.WriteLine("No workouts available for statistics.");
                return;
            }

            int total = workouts.Sum(w => w.DurationMinutes);
            double avg = workouts.Average(w => w.DurationMinutes);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nTotal Sessions: " + workouts.Count);
            Console.WriteLine("Total Minutes: " + total);
            Console.WriteLine("Average Duration: " + avg.ToString("F1") + " min");
            Console.ResetColor();

            Console.WriteLine("\nMinutes by Workout Type:");
            var byType = workouts
                .GroupBy(w => w.Name)
                .Select(g => new { Type = g.Key, Total = g.Sum(x => x.DurationMinutes), Count = g.Count() })
                .OrderByDescending(x => x.Total);

            foreach (var g in byType)
                Console.WriteLine(g.Type + ": " + g.Total + " min over " + g.Count + " sessions (avg " + (g.Total / (double)g.Count).ToString("F1") + " min)");
        }

        private static void ExportCsv()
        {
            Console.Write("CSV filename (default workouts.csv): ");
            string file = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(file)) file = "workouts.csv";

            try
            {
                List<string> lines = new List<string>();
                lines.Add("Date,Name,DurationMinutes");
                foreach (Workout w in workouts)
                    lines.Add(w.Date.ToString("yyyy-MM-dd") + ",\"" + w.Name + "\"," + w.DurationMinutes);

                File.WriteAllLines(file, lines.ToArray(), Encoding.UTF8);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Exported " + workouts.Count + " workouts to " + file);
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Export failed: " + ex.Message);
            }
        }

        private static void Load()
        {
            if (!File.Exists(DataFile)) return;
            try
            {
                string json = File.ReadAllText(DataFile);
                workouts = JsonSerializer.Deserialize<List<Workout>>(json);
                if (workouts == null) workouts = new List<Workout>();
                Console.WriteLine("Loaded " + workouts.Count + " workouts.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading data: " + ex.Message);
                workouts = new List<Workout>();
            }
        }

        private static void Save()
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.WriteIndented = true;
                string json = JsonSerializer.Serialize(workouts, options);
                File.WriteAllText(DataFile, json, Encoding.UTF8);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Data saved successfully.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving data: " + ex.Message);
            }
        }
    }

}

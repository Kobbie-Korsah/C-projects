using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FitnessTrackerBeta3
{
    // Compared to Beta 2:
    // - Adds a Guid Id to each workout for unique identification (used for edit/delete).
    // - Switches from CSV persistence to JSON (simpler structure, allows Ids easily).
    // - Adds new menu features: Search by name, Edit, Delete, Stats, and Export CSV.
    // - Removes “View by date” (replaced by name search and date-range filter).
    // - Uses improved display formatting and ordering.
    // - Adds weekly and per-type statistics.
    // - Uses JSON serialization instead of custom CSV parsing logic.

    class Workout
    {
        public Guid Id { get; set; } = Guid.NewGuid();  // NEW in Beta3: unique ID for each workout
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public int DurationMinutes { get; set; }

        // Updated display includes ID prefix
        public override string ToString() => $"{Id.ToString().Substring(0, 8)} | {Date:yyyy-MM-dd} | {Name} — {DurationMinutes} min";
    }

    class Program
    {
        static string DataFile = "workouts_beta3.json";   // CHANGED from CSV file to JSON file
        static List<Workout> workouts = new List<Workout>();

        static void Main(string[] args)
        {
            Load();
            Console.WriteLine("Fitness Tracker — Beta 3");
            while (true)
            {
                // Menu greatly expanded vs. Beta2
                Console.WriteLine("\nOptions:");
                Console.WriteLine("(1) Add ");
                Console.WriteLine("(2) View all ");
                Console.WriteLine("(3) Search name ");           // NEW
                Console.WriteLine("(4) Filter by date range ");  // Same concept as ViewByRange in Beta2
                Console.WriteLine("(5) Edit ");                  // NEW
                Console.WriteLine("(6) Delete ");                // NEW
                Console.WriteLine("(7) Stats ");                 // NEW
                Console.WriteLine("(8) Export CSV ");            // NEW (reverse of Beta2’s CSV save)
                Console.WriteLine("(9) Save ");                  // Equivalent to Beta2 Save
                Console.WriteLine("(0) Exit ");
                Console.Write("Choose: ");
                var key = Console.ReadLine();
                switch (key)
                {
                    case "1": AddWorkout(); break;
                    case "2": ViewAll(); break;
                    case "3": SearchByName(); break;
                    case "4": ViewByRange(); break;
                    case "5": EditWorkout(); break;
                    case "6": DeleteWorkout(); break;
                    case "7": ShowStats(); break;
                    case "8": ExportCsv(); break;
                    case "9": Save(); break;
                    case "0": Save(); return;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
        }

        static void AddWorkout()
        {
            // Similar logic to Beta2 but creates object with Guid Id
            Console.Write("Date (yyyy-MM-dd) or blank for today: ");
            var dateInput = Console.ReadLine();
            DateTime date;
            if (string.IsNullOrWhiteSpace(dateInput)) date = DateTime.Today;
            else if (!DateTime.TryParseExact(dateInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                Console.WriteLine("Invalid date format."); return;
            }

            Console.Write("Workout name: ");
            var name = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(name)) { Console.WriteLine("Name empty."); return; }

            Console.Write("Duration (minutes): ");
            if (!int.TryParse(Console.ReadLine(), out int duration) || duration <= 0) { Console.WriteLine("Invalid duration."); return; }

            var w = new Workout { Date = date, Name = name, DurationMinutes = duration };
            workouts.Add(w);
            Console.WriteLine("Added: " + w);
        }

        static void ViewAll()
        {
            // Simplified compared to Beta2 (no sorting by name, just by date)
            if (!workouts.Any()) { Console.WriteLine("No workouts."); return; }
            foreach (var w in workouts.OrderBy(w => w.Date)) Console.WriteLine(w);
        }

        static void SearchByName() 
        {
            Console.Write("Search term: ");
            var term = Console.ReadLine()?.Trim().ToLower();
            if (string.IsNullOrEmpty(term)) { Console.WriteLine("Empty search."); return; }
            var matches = workouts.Where(w => w.Name.ToLower().Contains(term)).OrderBy(w => w.Date).ToList();
            if (!matches.Any()) { Console.WriteLine("No matches."); return; }
            foreach (var m in matches) Console.WriteLine(m);
        }

        static void ViewByRange()
        {
            // Nearly identical to Beta2’s ViewByRange, but no extra validation for end < start
            Console.Write("Start date (yyyy-MM-dd): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime start)) { Console.WriteLine("Invalid date."); return; }
            Console.Write("End date (yyyy-MM-dd): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime end)) { Console.WriteLine("Invalid date."); return; }
            var found = workouts.Where(w => w.Date.Date >= start.Date && w.Date.Date <= end.Date).OrderBy(w => w.Date).ToList();
            if (!found.Any()) { Console.WriteLine("No workouts in range."); return; }
            foreach (var f in found) Console.WriteLine(f);
        }

        static void EditWorkout() 
        {
            // Uses Guid substring to find and edit workouts (wasn’t possible in Beta2)
            Console.Write("Enter first 8 chars of Id to edit: ");
            var idPart = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(idPart)) { Console.WriteLine("No id entered."); return; }
            var match = workouts.FirstOrDefault(w => w.Id.ToString().StartsWith(idPart, StringComparison.OrdinalIgnoreCase));
            if (match == null) { Console.WriteLine("Not found."); return; }

            Console.WriteLine("Editing: " + match);
            Console.Write($"New date (yyyy-MM-dd) or blank to keep ({match.Date:yyyy-MM-dd}): ");
            var d = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(d) && DateTime.TryParseExact(d, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime nd)) match.Date = nd;

            Console.Write($"New name or blank to keep ({match.Name}): ");
            var n = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(n)) match.Name = n.Trim();

            Console.Write($"New duration (minutes) or blank to keep ({match.DurationMinutes}): ");
            var dur = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(dur) && int.TryParse(dur, out int newDur) && newDur > 0) match.DurationMinutes = newDur;

            Console.WriteLine("Updated to: " + match);
        }

        static void DeleteWorkout() 
        {
            Console.Write("Enter first 8 chars of Id to delete: ");
            var idPart = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(idPart)) { Console.WriteLine("No id entered."); return; }
            var match = workouts.FirstOrDefault(w => w.Id.ToString().StartsWith(idPart, StringComparison.OrdinalIgnoreCase));
            if (match == null) { Console.WriteLine("Not found."); return; }

            Console.WriteLine("Found: " + match);
            Console.Write("Type 'yes' to confirm delete: ");
            if (Console.ReadLine()?.Trim().ToLower() == "yes")
            {
                workouts.Remove(match);
                Console.WriteLine("Deleted.");
            }
            else Console.WriteLine("Aborted.");
        }

        static void ShowStats() 
        {
            // Introduces summary analytics — absent in Beta2
            if (!workouts.Any()) { Console.WriteLine("No data for stats."); return; }

            var totalMinutes = workouts.Sum(w => w.DurationMinutes);
            var count = workouts.Count;
            var avg = totalMinutes / (double)count;
            Console.WriteLine($"\nTotal sessions: {count}");
            Console.WriteLine($"Total minutes: {totalMinutes}");
            Console.WriteLine($"Average minutes per session: {avg:F1}");

            // Grouped totals by workout type
            Console.WriteLine("\nMinutes by workout type:");
            var byType = workouts.GroupBy(w => w.Name, StringComparer.OrdinalIgnoreCase)
                                 .Select(g => new { Name = g.Key, Total = g.Sum(x => x.DurationMinutes), Count = g.Count() })
                                 .OrderByDescending(x => x.Total);
            foreach (var t in byType) Console.WriteLine($"{t.Name}: {t.Total} min over {t.Count} sessions (avg {t.Total / (double)t.Count:F1} min)");

            // NEW: weekly rollups using ISO week calculation
            Console.WriteLine("\nWeekly totals (ISO week starting Monday):");
            var weekly = workouts.GroupBy(w =>
            {
                var d = w.Date;
                var ci = CultureInfo.InvariantCulture;
                var cal = ci.Calendar;
                var week = cal.GetWeekOfYear(d, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                return (d.Year, week);
            }).OrderBy(g => g.Key.Year).ThenBy(g => g.Key.week);
            foreach (var w in weekly)
            {
                var sum = w.Sum(x => x.DurationMinutes);
                Console.WriteLine($"Year {w.Key.Year} Week {w.Key.week}: {sum} min ({w.Count()} sessions)");
            }
        }

        static void ExportCsv() 
        {
            // Allows exporting JSON data to CSV — the reverse of Beta2’s storage method
            Console.Write("Export filename (leave blank for workouts_export.csv): ");
            var filename = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(filename)) filename = "workouts_export.csv";
            var lines = new List<string> { "Date,Name,DurationMinutes" };
            lines.AddRange(workouts.OrderBy(w => w.Date).Select(w => $"{w.Date:yyyy-MM-dd},\"{w.Name.Replace("\"", "\"\"")}\",{w.DurationMinutes}"));
            try
            {
                File.WriteAllLines(filename, lines, Encoding.UTF8);
                Console.WriteLine($"Exported {workouts.Count} rows to {filename}.");
            }
            catch (Exception ex) { Console.WriteLine("Failed to export: " + ex.Message); }
        }

        static void Load()
        {
            // Replaces Beta2’s CSV load with JSON deserialization
            if (!File.Exists(DataFile)) return;
            try
            {
                var json = File.ReadAllText(DataFile);
                workouts = JsonSerializer.Deserialize<List<Workout>>(json) ?? new List<Workout>();
                Console.WriteLine($"Loaded {workouts.Count} workouts.");
            }
            catch (Exception ex) { Console.WriteLine("Failed to load JSON: " + ex.Message); workouts = new List<Workout>(); }
        }

        static void Save()
        {
            // Replaces Beta2’s CSV Save() with JSON serialization
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(workouts, options);
                File.WriteAllText(DataFile, json, Encoding.UTF8);
                Console.WriteLine($"Saved {workouts.Count} workouts.");
            }
            catch (Exception ex) { Console.WriteLine("Failed to save: " + ex.Message); }
        }
    }
}

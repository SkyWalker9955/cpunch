using System.ComponentModel.Design;

namespace CpunchApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize work types if they don't exist
            if (!File.Exists(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CpunchApp", "worktypes.json")))
            {
                InitializeWorkTypes();
            }

            var workTypes = WorkTypeDao.LoadWorkTypes();

            if (args.Length == 0)
            {
                // List all available work types
                Console.WriteLine("Available Work Types:");
                for (int i = 0; i < workTypes.Count; i++)
                {
                    Console.WriteLine($"{i + 1} - {workTypes[i].Name} - ${workTypes[i].HourlyRate}/h");
                }
                Console.WriteLine("Use 'cpunch start <number>' to punch in with a specific work type.");
                return;
            }

            switch (args[0].ToLower())
            {
                case "start":
                    int workTypeIndex = -1;
                    if (args.Length > 1 && int.TryParse(args[1], out workTypeIndex))
                    {
                        StartPunch(workTypeIndex - 1); // Adjusting for zero-based index
                    }
                    else
                    {
                        Console.WriteLine("Please provide a valid work type number. Use 'cpunch' to see available work types.");
                    }
                    break;
                case "stop":
                    StopPunch();
                    break;
                case "addtype":
                    AddWorkType();
                    break;
                case "listtypes":
                    ListWorkTypes();
                    break;
                case "report":
                    GenerateReport();
                    break;
                case "help":
                    Help();
                    break;
                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }

        // Methods
        static void Help()
        {
            //TODO: Implement
            throw new NotImplementedException();
        }

        static void StartPunch(int workTypeIndex)
        {
            var workTypes = WorkTypeDao.LoadWorkTypes();
            if (workTypes.Count == 0)
            {
                Console.WriteLine("No work types available. Add a work type first.");
                return;
            }

            if (workTypeIndex < 0 || workTypeIndex >= workTypes.Count)
            {
                Console.WriteLine("Invalid work type index. Use 'cpunch' to see available work types.");
                return;
            }

            var selectedWorkType = workTypes[workTypeIndex];

            var punches = PunchStorageDao.LoadPunchRecords();

            // Check if there's an ongoing punch
            if (punches.Exists(p => p.PunchOutTime == null))
            {
                Console.WriteLine("You have an ongoing punch. Please punch out first.");
                return;
            }

            punches.Add(new PunchRecord
            {
                WorkTypeName = selectedWorkType.Name,
                PunchInTime = DateTime.Now
            });

            PunchStorageDao.SavePunchRecords(punches);

            Console.WriteLine($"Punched in for {selectedWorkType.Name} at {DateTime.Now}");
        }

        static void StopPunch()
        {
            var punches = PunchStorageDao.LoadPunchRecords();
            var ongoingPunch = punches.Find(p => p.PunchOutTime == null);

            if (ongoingPunch == null)
            {
                Console.WriteLine("No ongoing punch found.");
                return;
            }

            ongoingPunch.PunchOutTime = DateTime.Now;
            PunchStorageDao.SavePunchRecords(punches);

            Console.WriteLine($"Punched out at {DateTime.Now}");

            // Calculate the total time and amount
            TimeSpan duration = ongoingPunch.PunchOutTime.Value - ongoingPunch.PunchInTime;
            var workTypes = WorkTypeDao.LoadWorkTypes();
            var workType = workTypes.Find(wt => wt.Name == ongoingPunch.WorkTypeName);
            decimal amount = (decimal)duration.TotalHours * workType.HourlyRate;

            Console.WriteLine($"Total time: {duration.TotalHours:F2} hours");
            Console.WriteLine($"Amount: ${amount:F2}");
        }

        static void AddWorkType()
        {
            Console.Write("Enter work type name: ");
            string name = Console.ReadLine();

            Console.Write("Enter hourly rate: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal rate))
            {
                Console.WriteLine("Invalid rate.");
                return;
            }

            var workTypes = WorkTypeDao.LoadWorkTypes();
            workTypes.Add(new WorkType { Name = name, HourlyRate = rate });
            WorkTypeDao.SaveWorkTypes(workTypes);

            Console.WriteLine("Work type added successfully.");
        }

        static void ListWorkTypes()
        {
            var workTypes = WorkTypeDao.LoadWorkTypes();
            if (workTypes.Count == 0)
            {
                Console.WriteLine("No work types available.");
                return;
            }

            Console.WriteLine("Available Work Types:");
            for (int i = 0; i < workTypes.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {workTypes[i].Name} - ${workTypes[i].HourlyRate}/h");
            }
        }

        static void GenerateReport()
        {
            var punches = PunchStorageDao.LoadPunchRecords();
            var workTypes = WorkTypeDao.LoadWorkTypes();

            if (punches.Count == 0)
            {
                Console.WriteLine("No punch records found.");
                return;
            }

            // Group punches by work type
            var reportData = punches
                .Where(p => p.PunchOutTime != null)
                .GroupBy(p => p.WorkTypeName)
                .Select(g =>
                {
                    var totalHours = g.Sum(p => (p.PunchOutTime.Value - p.PunchInTime).TotalHours);
                    var hourlyRate = workTypes.FirstOrDefault(wt => wt.Name == g.Key)?.HourlyRate ?? 0;
                    var totalAmount = (decimal)totalHours * hourlyRate;

                    return new
                    {
                        WorkTypeName = g.Key,
                        TotalHours = totalHours,
                        TotalAmount = totalAmount
                    };
                });

            Console.WriteLine("Report:");
            foreach (var data in reportData)
            {
                Console.WriteLine($"- {data.WorkTypeName}: {data.TotalHours:F2} hours, Amount: ${data.TotalAmount:F2}");
            }
        }

        static void InitializeWorkTypes()
        {
            var workTypes = new List<WorkType>
            {
                new WorkType { Name = "Grosure Project", HourlyRate = 20 },
                new WorkType { Name = "Pet Projects", HourlyRate = 20 },
                new WorkType { Name = "USCIS Work", HourlyRate = 30 },
                new WorkType { Name = "Gaming", HourlyRate = -8 },
                new WorkType { Name = "Home Cleaning", HourlyRate = 13 },
                new WorkType { Name = "Family Biz", HourlyRate = 30 }
            };
            WorkTypeDao.SaveWorkTypes(workTypes);
        }
    }
}

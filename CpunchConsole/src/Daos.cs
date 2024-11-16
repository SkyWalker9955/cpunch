using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CpunchApp
{
    public static class WorkTypeDao
    {
        private static readonly string dataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CpunchApp"
        );
        private static readonly string filePath = Path.Combine(dataDirectory, "worktypes.json");

        public static List<WorkType> LoadWorkTypes()
        {
            if (!Directory.Exists(dataDirectory))
                Directory.CreateDirectory(dataDirectory);

            if (!File.Exists(filePath))
                return [];

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<WorkType>>(json);
        }

        public static void SaveWorkTypes(List<WorkType> workTypes)
        {
            if (!Directory.Exists(dataDirectory))
                Directory.CreateDirectory(dataDirectory);

            var json = JsonSerializer.Serialize(workTypes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }

    public static class PunchStorageDao
    {
        private static string dataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CpunchApp"
        );
        private static string filePath = Path.Combine(dataDirectory, "punches.json");

        public static List<PunchRecord> LoadPunchRecords()
        {
            if (!Directory.Exists(dataDirectory))
                Directory.CreateDirectory(dataDirectory);

            if (!File.Exists(filePath))
                return new List<PunchRecord>();

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<PunchRecord>>(json);
        }

        public static void SavePunchRecords(List<PunchRecord> punches)
        {
            if (!Directory.Exists(dataDirectory))
                Directory.CreateDirectory(dataDirectory);

            var json = JsonSerializer.Serialize(punches, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }
}

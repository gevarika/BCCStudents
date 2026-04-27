using BCCStudents.Domain.Entities;
using Newtonsoft.Json;

namespace BCCStudents.Infrastructure.Data.JSON
{
    /// <summary>
    /// JSON-ის დახმარებით სტუდენტების შენახვა და ჩატვირთვა
    /// </summary>
    public static class StudentJsonHelper
    {
        private static readonly string JsonFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BCCStudents",
            "failed_students.json");

        /// <summary>
        /// JSON ფაილიდან სტუდენტების ჩატვირთვა
        /// </summary>
        public static List<Student> LoadStudents()
        {
            try
            {
                if (!File.Exists(JsonFilePath))
                    return new List<Student>();

                var json = File.ReadAllText(JsonFilePath);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<Student>();

                var students = JsonConvert.DeserializeObject<List<Student>>(json);
                return students ?? new List<Student>();
            }
            catch (Exception ex)
            {
                // Log error if needed
                return new List<Student>();
            }
        }

        /// <summary>
        /// სტუდენტების შენახვა JSON ფაილში
        /// </summary>
        public static void SaveStudents(List<Student> students)
        {
            try
            {
                var directory = Path.GetDirectoryName(JsonFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonConvert.SerializeObject(students, Formatting.Indented);
                File.WriteAllText(JsonFilePath, json);
            }
            catch (Exception ex)
            {
                // Log error if needed
                throw;
            }
        }
    }
}


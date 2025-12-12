using System;
using System.Collections.Generic;
using System.IO;
using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using Newtonsoft.Json;

namespace BCCStudents.Infrastructure.Services
{
    /// <summary>
    /// სტუდენტების JSON-ით შენახვისა და ჩატვირთვის სერვისი
    /// იმპლემენტირებს IStudentJsonService ინტერფეისს
    /// </summary>
    public class StudentJsonService : IStudentJsonService
    {
        private readonly string _jsonFilePath;

        /// <summary>
        /// კონსტრუქტორი
        /// </summary>
        public StudentJsonService()
        {
            _jsonFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BCCStudents",
                "failed_students.json");
        }

        /// <summary>
        /// კონსტრუქტორი კონკრეტული ფაილის გზით
        /// </summary>
        /// <param name="jsonFilePath">JSON ფაილის სრული გზა</param>
        public StudentJsonService(string jsonFilePath)
        {
            _jsonFilePath = jsonFilePath ?? throw new ArgumentNullException(nameof(jsonFilePath));
        }

        /// <summary>
        /// JSON ფაილიდან სტუდენტების ჩატვირთვა
        /// </summary>
        /// <returns>სტუდენტების სია</returns>
        public List<Student> LoadStudents()
        {
            try
            {
                if (!File.Exists(_jsonFilePath))
                    return new List<Student>();

                var json = File.ReadAllText(_jsonFilePath);
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
        /// <param name="students">სტუდენტების სია</param>
        public void SaveStudents(List<Student> students)
        {
            try
            {
                var directory = Path.GetDirectoryName(_jsonFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonConvert.SerializeObject(students, Formatting.Indented);
                File.WriteAllText(_jsonFilePath, json);
            }
            catch (Exception ex)
            {
                // Log error if needed
                throw;
            }
        }
    }
}


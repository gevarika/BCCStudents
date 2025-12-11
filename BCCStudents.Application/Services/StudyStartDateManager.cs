using BCCStudents.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCCStudents.Application.Services {
    public class StudyStartDateManager
    {
        private static readonly string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BCCStudents", "study_start_date.dat");
        private static IStudentRepository _studentRepository;
        private static IGroupRepository _groupRepository;
        private static ISubGroupRepository _subGroupRepository;

        public StudyStartDateManager(ISubGroupRepository subGroupRepository, IGroupRepository groupRepository, IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
            _groupRepository = groupRepository;
            _subGroupRepository = subGroupRepository;
        }
        // 🔐 მონაცემების დაშიფვრა
        private static string Encrypt(string plainText)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(bytes);
        }

        // 🔓 მონაცემების გაშიფვრა
        private static string Decrypt(string encryptedText)
        {
            byte[] bytes = Convert.FromBase64String(encryptedText);
            return Encoding.UTF8.GetString(bytes);
        }

        // 📌 თარიღის შენახვა ფაილში
        public static void SaveStudyStartDate(DateTime startDate, StudyStartDateManager mng)
        {
            // ვამოწმებთ და ვქმნით დირექტორიას თუ საჭიროა
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            string encryptedDate = Encrypt(startDate.ToString("dd-MM-yyyy"));
            File.WriteAllText(filePath, encryptedDate);
            mng.SetStudentPaymentDataForAll(startDate);
        }

        public void SetStudentPaymentDataForAll(DateTime selectedDate)
        {
            // მხოლოდ ახალი მოსწავლეებისთვის გადახდის თარიღის დაყენება
            // ეს მეთოდი გამოიძახება მხოლოდ პირველი მოსწავლის რეგისტრაციისას
            Console.WriteLine($"SetStudentPaymentDataForAll გამოიძახება თარიღით: {selectedDate:dd-MM-yyyy}");
            Console.WriteLine("ეს მეთოდი ახლა მხოლოდ ინფორმაციულია - ახალი მოსწავლეები იღებენ თარიღს რეგისტრაციისას");
            
            // ახალი მოსწავლეები იღებენ თარიღს StudentManagementForm-ში
            // ამიტომ აქ არაფერი გვჭირდება
        }
        // 📌 თარიღის აღება ფაილიდან
        public static DateTime? GetStudyStartDate()
        {
            if (!File.Exists(filePath))
                return null;

            string encryptedDate = File.ReadAllText(filePath);
            return DateTime.Parse(Decrypt(encryptedDate));
        }

        // 📌 ფაილის არსებობის შემოწმება
        public static bool IsStudyStartDateSet()
        {
            return File.Exists(filePath);
        }
    }
}



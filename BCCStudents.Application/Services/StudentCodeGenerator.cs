using System;
using System.Collections.Generic;
using BCCStudents.Domain.Interfaces;

using BCCStudents.Application.Interfaces;

namespace BCCStudents.Application.Services
{
    /// <summary>
    /// სტუდენტის კოდის გენერატორი
    /// </summary>
    public class StudentCodeGenerator : IStudentCodeGenerator
    {
        private readonly IStudentRepository _studentRepository;

        public StudentCodeGenerator(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        }

        /// <summary>
        /// გენერირებს ახალ სტუდენტის კოდს (STU-0001, STU-1234 და ა.შ.)
        /// </summary>
        public string GenerateStudentCode()
        {
            string newCode;
            Random random = new Random();

            do
            {
                int numericPart = random.Next(1, 9999); // 0001 - 9999
                newCode = $"STU-{numericPart:D4}"; // ფორმატი STU-0001, STU-1234 და ა.შ.

            } while (_studentRepository.StudentExistsByCode(newCode)); // თუ კოდი უკვე არსებობს, გენერაცია თავიდან ხდება

            return newCode;
        }

        /// <summary>
        /// გენერირებს უნიკალურ სტუდენტის კოდს, რომელიც არ არსებობს არც existingCodes-ში და არც pendingCodes-ში
        /// </summary>
        public string GenerateUniqueStudentCode(HashSet<string> existingCodes, HashSet<string> pendingCodes)
        {
            string code;
            int i = 1;
            do
            {
                code = $"STU-{i.ToString("D4")}";
                i++;
            }
            while (existingCodes.Contains(code) || pendingCodes.Contains(code));
            return code;
        }
    }
}


namespace BCCStudents.Domain.Entities
{
    /// <summary>
    /// სისტემური კონფიგურაციის Entity
    /// ინახავს სწავლის დაწყების თარიღს, გადახდის თარიღს და დასვენებების თარიღებს
    /// </summary>
    public class SystemConfiguration
    {
        public int Id { get; set; }

        /// <summary>
        /// კონფიგურაციის Key (მაგ: "StudyStartDate", "DefaultPaymentDate", "Vacation_Start_2024", "Vacation_End_2024")
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// კონფიგურაციის Value (თარიღი JSON ფორმატში ან სხვა მონაცემი)
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// კონფიგურაციის ტიპი (მაგ: "Date", "VacationPeriod")
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// შენიშვნა/აღწერა
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// შექმნის თარიღი
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// განახლების თარიღი
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// ნაგულისხმევი კონსტრუქტორი
        /// </summary>
        public SystemConfiguration()
        {
            CreatedAt = DateTime.Now;
        }
    }
}

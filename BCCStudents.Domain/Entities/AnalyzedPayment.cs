namespace BCCStudents.Domain.Entities
{
    /// <summary>
    /// გაანალიზებული გადახდის მოდელი - შეიცავს ანალიზის შედეგებს
    /// </summary>
    public class AnalyzedPayment
    {
        public bool IsValid { get; set; }
        public int? StudentId { get; set; }
        public string StudentName { get; set; }
        public string GroupName { get; set; }
        public double MatchConfidence { get; set; }
        public string MatchType { get; set; }
        public string MatchDetails { get; set; }
        public string ErrorMessage { get; set; }
        public bool RequiresReview { get; set; }
        public bool IsSelected { get; set; }
    }
}


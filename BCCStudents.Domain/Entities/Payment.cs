namespace BCCStudents.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; } // გადახდის უნიკალური იდენტიფიკატორი
        public int? StudentId { get; set; } // სტუდენტის ID
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public decimal Amount { get; set; } // გადახდილი თანხა
        public DateTime PaymentDate { get; set; } // გადახდის თარიღი
        public string Description { get; set; }
        public PaymentSource PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string Note { get; set; }
        public string FailureReason { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public long? PersonalId { get; set; }
        public string PayerName { get; set; }
        public string MatchedStudentName { get; set; }
        public string MatchedGroupName { get; set; }
        public string AnalysisResult { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsMatched { get; set; }
        public bool RequiresReview { get; set; }
        public bool IsSelected { get; set; }
    }

    public class FailedPayment
    {
        public int Id { get; set; }
        public int RowNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public long? PersonalId { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ImportedPaymentLog
    {
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public long? PersonalId { get; set; }
        public string Description { get; set; }
        public string ImportSource { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PaymentSummary
    {
        public string StudentCode { get; set; }
        public int StudentID { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GroupName { get; set; }
        public decimal TuitionFee { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal AmountDue => TuitionFee - TotalPaid;
        public DateTime? NextPaymentDate { get; set; }
        public bool IsSuccessful { get; set; }
    }

    public class SuccessfulPayment
    {
        public DateTime NextPaymentDate { get; set; }
        public decimal TotalPaid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
    }

}


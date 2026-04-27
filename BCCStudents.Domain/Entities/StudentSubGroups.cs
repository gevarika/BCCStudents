namespace BCCStudents.Domain.Entities
{
    public class StudentSubGroups
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int GroupId { get; set; }
        public int SubGroupId { get; set; }
        public bool Status { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? DateOfPayment { get; set; }
        public decimal Price { get; set; }
        public double Discount { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}


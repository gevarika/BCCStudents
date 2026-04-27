namespace BCCStudents.Domain.Entities
{
    public class StudentGroups
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StudentId { get; set; }
        public int GroupId { get; set; }
        public int? SubGroupId { get; set; }
        public bool Status { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? DateOfPayment { get; set; }
        public double Discount { get; set; }
        //public bool ActiveStatus { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}


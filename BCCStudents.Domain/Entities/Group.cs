namespace BCCStudents.Domain.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Teacher { get; set; }
        public Boolean Status { get; set; }
        public int StudentCount { get; set; }
        public int MaxStudents { get; set; }
        public string ContractTemplatePath { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}


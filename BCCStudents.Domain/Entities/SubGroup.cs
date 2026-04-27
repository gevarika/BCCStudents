namespace BCCStudents.Domain.Entities
{
    public class SubGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal TuitionFee { get; set; }
        public int MaxStudents { get; set; }
        public string ParentGroupName { get; set; }
        public int GroupId { get; set; }
        public int StudentCount { get; set; }
        public Boolean Status { get; set; }
        //public bool ActiveStatus { get; set; }
        public DateTime UpdatedAt { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}


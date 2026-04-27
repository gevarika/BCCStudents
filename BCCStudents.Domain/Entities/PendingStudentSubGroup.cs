namespace BCCStudents.Domain.Entities
{
    public class PendingStudentSubGroup
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int SubGroupId { get; set; }
        public int GroupId { get; set; }
        public string Status { get; set; }
    }
}


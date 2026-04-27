namespace BCCStudents.Domain.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string ParentName { get; set; }
        public string PhoneNumber { get; set; }
        public long Id_Numb { get; set; }
        public string Address { get; set; }
        public DateTime RegistrationDate { get; set; }
        //public DateTime? PaymentStartDate { get; set; }
        public DateTime? DateOfPayment { get; set; }
        public decimal TuitionFee { get; set; }
        public int GroupId { get; set; }
        public int SubGroup { get; set; }
        public double Discount { get; set; }
        public string StudentCode { get; set; }
        //public bool ActiveStatus { get; set; }
        public bool Status { get; set; }
        public string Info { get; set; }
        public string PaymentStatus { get; set; }
        public string IdCardPath { get; set; }
        public string AdditionalDocsPath { get; set; }
        public int User_Id { get; set; }
        public decimal Balance { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string GroupName { get; set; }
        //public string Description { get; set; }
        public StudentGroups StudentGroups { get; set; }
        public List<Group> Groups { get; set; }
        public List<StudentGroups> StudentGroupsList { get; set; }
        public List<SubGroup> StudentSubGroupsList { get; set; }
        public List<SubGroup> SubGroups { get; set; }
    }
    public class StudentViewDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Balance { get; set; }
        public int Age { get; set; }
        public string ParentName { get; set; }
        public long Id_Numb { get; set; }
        public string Address { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? PaymentStartDate { get; set; }
        public DateTime? DateOfPayment { get; set; }
        public decimal TuitionFee { get; set; }
        public double Discount { get; set; }
        public string StudentCode { get; set; }
        public bool Status { get; set; }
        public string PaymentStatus { get; set; }
        public string IdCardPath { get; set; }
        public string AdditionalDocsPath { get; set; }
        public string GroupName { get; set; }
        public int User_Id { get; set; }
    }

    /// <summary>
    /// მოსწავლის გასაღები (იმპორტის დროს დუბლიკატების შესამოწმებლად)
    /// </summary>
    public class StudentKey
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long IdNumb { get; set; }
        public string Address { get; set; }
        public string StudentCode { get; set; }
    }

}


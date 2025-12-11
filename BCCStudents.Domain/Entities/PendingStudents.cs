using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCCStudents.Domain.Entities {
    public class PendingStudent
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string ParentName { get; set; }
        public string PhoneNumber { get; set; }
        public long Id_Numb { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime RegistrationDate { get; set; }
        public decimal TuitionFee { get; set; }
        public int Discount { get; set; }
        public string StudentCode { get; set; }
        //public bool Status { get; set; }
        public string IdCardPath { get; set; }
        public string AdditionalDocsPath { get; set; }
        public decimal Balance { get; set; }
        // სხვა საჭირო ველები, როგორიცაა Discount, Id_Numb, GroupId, და ა.შ.
    }

}


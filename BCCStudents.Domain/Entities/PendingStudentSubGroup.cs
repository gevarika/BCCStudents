using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCCStudents.Domain.Entities {
    public class PendingStudentSubGroup
    {
        public int StudentId { get; set; }
        public int SubGroupId { get; set; }
        public int GroupId { get; set; }
        public string Status { get; set; }
    }
}


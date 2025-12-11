using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCCStudents.Domain.Entities {
    public class ApprovalResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } // მიზეზი თუ ჩავარდა
    }
}


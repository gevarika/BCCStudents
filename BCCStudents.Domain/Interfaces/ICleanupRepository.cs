using BCCStudents.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCCStudents.Domain.Interfaces
{
    public interface ICleanupRepository
    {
        void ResetAllData();
    }
}



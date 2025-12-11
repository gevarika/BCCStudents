using BCCStudents.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCCStudents.Domain.Interfaces
{
    public interface IPendingStudentRepository
    {
        List<PendingStudent> GetAll();
        void Delete(int id);
        PendingStudent GetById(int id);
        void Update(PendingStudent student);
        void UpdatePartial(int id, Dictionary<string, object> fields);
    }
}



using BCCStudents.Domain.Entities;

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



using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// სტუდენტების JSON-ით შენახვისა და ჩატვირთვის სერვისის ინტერფეისი
    /// Clean Architecture-ის დაცვით - Application Layer არ დამოკიდებულია Infrastructure Layer-ზე პირდაპირ
    /// </summary>
    public interface IStudentJsonService
    {
        /// <summary>
        /// JSON ფაილიდან სტუდენტების ჩატვირთვა
        /// </summary>
        /// <returns>სტუდენტების სია</returns>
        List<Student> LoadStudents();

        /// <summary>
        /// სტუდენტების შენახვა JSON ფაილში
        /// </summary>
        /// <param name="students">სტუდენტების სია</param>
        void SaveStudents(List<Student> students);
    }
}


using System.Threading.Tasks;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// SMS სერვისის ინტერფეისი
    /// Clean Architecture-ის დაცვით - Application Layer არ დამოკიდებულია Infrastructure Layer-ზე პირდაპირ
    /// </summary>
    public interface ISmsService
    {
        /// <summary>
        /// SMS-ის გაგზავნა
        /// </summary>
        /// <param name="phoneNumber">ტელეფონის ნომერი</param>
        /// <param name="message">შეტყობინება</param>
        /// <returns>SMS გაგზავნის შედეგი</returns>
        Task<SmsSendResult> SendSmsAsync(string phoneNumber, string message);
    }
}


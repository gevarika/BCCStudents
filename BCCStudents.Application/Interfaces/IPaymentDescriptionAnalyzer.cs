using System.Threading.Tasks;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// გადახდის აღწერის ანალიზატორის ინტერფეისი
    /// </summary>
    public interface IPaymentDescriptionAnalyzer
    {
        /// <summary>
        /// აანალიზებს გადახდის აღწერას და აბრუნებს შესაბამის სტუდენტს
        /// </summary>
        /// <param name="description">გადახდის აღწერა</param>
        /// <param name="personalId">პირადი ნომერი (ოფციონალური)</param>
        /// <returns>ანალიზის შედეგი</returns>
        Task<AnalyzedPayment> AnalyzeDescription(string description, long? personalId = null);
    }
}


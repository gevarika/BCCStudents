using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCCStudents.Application.Services;
using BCCStudents.Domain.Entities;
using Core.Models;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// გადახდის სერვისის ინტერფეისი
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// გადახდის დამუშავება - მოსწავლის ბალანსიდან ან ახლად ჩარიცხული თანხიდან
        /// </summary>
        /// <param name="studentId">მოსწავლის ID</param>
        /// <param name="paymentAmount">ახლად ჩარიცხული თანხა (0 = მხოლოდ ბალანსიდან)</param>
        /// <param name="useFullBalance">თუ true - გამოიყენებს სრულ ბალანსს (რამდენ თვესაც ფარავს), თუ false - მხოლოდ ერთი თვე</param>
        /// <returns>გადახდის შედეგი</returns>
        Task<PaymentResult> ProcessPayment(int studentId, decimal paymentAmount, bool useFullBalance = true);

        /// <summary>
        /// ავტომატური გადახდების დამუშავება ყველა მოსწავლისთვის
        /// </summary>
        /// <param name="progressCallback">პროგრესის კალბექი (current, total)</param>
        /// <param name="useFullBalance">თუ true - გამოიყენებს სრულ ბალანსს</param>
        Task ProcessAutoPaymentsForAllStudentsAsync(Action<int, int> progressCallback = null, bool useFullBalance = true);

        /// <summary>
        /// აბრუნებს გადახდების შეჯამებას
        /// </summary>
        List<PaymentSummary> GetPaymentSummaries();
    }
}


using System;

namespace Core.Models
{
    /// <summary>
    /// áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ“áƒ”áƒ¢áƒáƒšáƒ£áƒ áƒ˜ áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ˜áƒ¡ áƒ™áƒšáƒáƒ¡áƒ˜
    /// </summary>
    public class PaymentDetail
    {
        /// <summary>
        /// áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ ID
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ áƒ¡áƒáƒ®áƒ”áƒšáƒ˜
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒšáƒ˜ áƒ—áƒáƒœáƒ®áƒ
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜ (Paid, Partial, Credited, NoPayment)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// áƒ“áƒáƒ áƒ©áƒ”áƒœáƒ˜áƒšáƒ˜ áƒ’áƒáƒ“áƒáƒ¡áƒáƒ®áƒ“áƒ”áƒšáƒ˜ áƒ—áƒáƒœáƒ®áƒ (áƒœáƒáƒ¬áƒ˜áƒšáƒáƒ‘áƒ áƒ˜áƒ•áƒ˜ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ¨áƒ”áƒ›áƒ—áƒ®áƒ•áƒ”áƒ•áƒáƒ¨áƒ˜)
        /// </summary>
        public decimal? RemainingAmount { get; set; }

        /// <summary>
        /// áƒ¨áƒ”áƒ›áƒ“áƒ”áƒ’áƒ˜ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜
        /// </summary>
        public DateTime? NextPaymentDate { get; set; }

        /// <summary>
        /// áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜
        /// </summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// áƒ“áƒáƒ›áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ—áƒ˜ áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ¨áƒ”áƒ¡áƒáƒ®áƒ”áƒ‘
        /// </summary>
        public string Note { get; set; }
    }
} 

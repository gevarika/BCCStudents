namespace Core.Models
{
    /// <summary>
    /// გადახდის დეტალური ინფორმაციის კლასი
    /// </summary>
    public class PaymentDetail
    {
        /// <summary>
        /// ჯგუფის ID
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// ჯგუფის სახელი
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// გადახდილი თანხა
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// გადახდის სტატუსი (Paid, Partial, Credited, NoPayment)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// დარჩენილი გადასახდელი თანხა (ნაწილობრივი გადახდის შემთხვევაში)
        /// </summary>
        public decimal? RemainingAmount { get; set; }

        /// <summary>
        /// შემდეგი გადახდის თარიღი
        /// </summary>
        public DateTime? NextPaymentDate { get; set; }

        /// <summary>
        /// გადახდის თარიღი
        /// </summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// დამატებითი ინფორმაცია გადახდის შესახებ
        /// </summary>
        public string Note { get; set; }
    }
}

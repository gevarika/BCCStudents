using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCCStudents.Application.Services {
    public class DiscountCalculator
    {
        public decimal TuitionFee { get; }
        public decimal DiscountAmount { get; }

        public DiscountCalculator(decimal tuitionFee, decimal discountAmount)
        {
            TuitionFee = tuitionFee;
            DiscountAmount = discountAmount;
        }

        public decimal GetFinalAmount()
        {
            // ფასდაკლების გამოთვლა პროცენტულად
            decimal discountValue = (TuitionFee * DiscountAmount) / 100;
            decimal finalAmount = TuitionFee - discountValue;
            return (finalAmount < 0 ? 0 : finalAmount); // უარყოფითს არ დავაბრუნებთ
        }

        public decimal GetDiscountValue()
        {
            // ფასდაკლების თანხა
            return (TuitionFee * DiscountAmount) / 100;
        }
    }
}



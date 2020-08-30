using System;
using Trendyol.ShoppingCart.Enums;

namespace Trendyol.ShoppingCart.Models
{
    public class Coupon: BaseModel
    {
        public double MinPrice { get; set; }
        public double DiscountAmount { get; set; }
        public DiscountType TypeOfDiscount { get; set; }

        public Coupon(double minPrice, double amount, DiscountType discountType)
        {
            MinPrice = minPrice;
            DiscountAmount = amount;
            TypeOfDiscount = discountType;
        }
    }
}

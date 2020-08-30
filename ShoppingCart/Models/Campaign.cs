using System;
using Trendyol.ShoppingCart.Enums;

namespace Trendyol.ShoppingCart.Models
{
    public class Campaign: BaseModel
    {
        public Category Category { get; set; }
        public DiscountType TypeOfDiscount { get; set; }
        public int MinQuantity { get; set; }
        public double Amount { get; set; }

        public Campaign(Category category, double discount, int minQuantity,DiscountType discountType)
        {
            Category = category;
            Amount = discount;
            MinQuantity = minQuantity;
            TypeOfDiscount = discountType;
        }
    }
}

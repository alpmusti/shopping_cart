using System;
using Trendyol.ShoppingCart.Contracts;
using Trendyol.ShoppingCart.Enums;
using Trendyol.ShoppingCart.Models;

namespace Trendyol.ShoppingCart
{
    class Program
    {
        static void Main(string[] args)
        {
            Category food = new Category("Food");
            //Category food2 = new Category("Food2");
            Category dress = new Category("Dress");
            Category phoneAccessory = new Category("Phone Accessories");

            Product apple = new Product("Apple", 100.0, food);
            Product almond = new Product("Almond", 150.0, food);
            Product shirt = new Product("T-Shirt", 30, dress);

            IShoppingCart shoppingCart = new ShoppingCart(new DeliveryCostCalculator(1, 5));
            shoppingCart.AddItem(apple, 3);
            shoppingCart.AddItem(almond, 1);
            shoppingCart.AddItem(shirt, 1);

            Campaign campaign1 = new Campaign(food, 20.0, 3, DiscountType.Rate);
            Campaign campaign2 = new Campaign(dress, 50.0, 5, DiscountType.Rate);
            Campaign campaign3 = new Campaign(phoneAccessory, 5.0, 5, DiscountType.Amount);

            shoppingCart.ApplyDiscounts(campaign1, campaign2, campaign3);

            Coupon coupon = new Coupon(100, 10, DiscountType.Rate);
            shoppingCart.ApplyCoupon(coupon);

            shoppingCart.Print();
        }
    }
}

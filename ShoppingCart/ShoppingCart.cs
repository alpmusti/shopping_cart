using System;
using System.Collections.Generic;
using System.Linq;
using Trendyol.ShoppingCart.Contracts;
using Trendyol.ShoppingCart.Manager;
using Trendyol.ShoppingCart.Models;

namespace Trendyol.ShoppingCart
{
    public class ShoppingCart: BaseModel, IShoppingCart
    {
        public Dictionary<Product, int> Products { get; set; }
        private Coupon CartCoupon { get; set; }
        private List<Campaign> Campaigns { get; set; }
        private IDeliveryCostCalculator DeliveryCostCalculator { get; set; }
        private CampaignManager _campaignManager;

        public ShoppingCart(IDeliveryCostCalculator deliveryCostCalculator) {
            DeliveryCostCalculator = deliveryCostCalculator;
            Campaigns = new List<Campaign>();
            Products = new Dictionary<Product, int>();
            _campaignManager = new CampaignManager();
        }

        public void AddItem(Product product,int quantity)
        {
            if (product != null && quantity > 0)
            {
                if(Products.TryGetValue(product, out int productQuantity))
                {
                    Products[product] = productQuantity + quantity;
                }else
                {
                    Products.Add(product, quantity);
                }
            }
        }


        public double GetNumberOfDeliveries()
        {
            return Products.GroupBy(product => product.Key.Category.Title).Count();
        }

        public double GetNumberOfProducts()
        {
            return Products.Count();
        }

        public double GetDeliveryCost()
        {
            return DeliveryCostCalculator.CalculateFor(this);
        }

        public void ApplyDiscounts(params Campaign[] campaigns)
        {
            if(campaigns == null)
            {
                throw new ArgumentNullException("campaigns must not be Null");
            }
            Campaigns.AddRange(campaigns);
        }

        public void ApplyCoupon(Coupon coupon)
        {
            if(coupon == null)
            {
                throw new ArgumentNullException("coupon must not be Null");
            }
            CartCoupon = coupon;
        }

        public double GetCouponDiscount()
        {
            // We need to apply campaigns first then coupon
            double cartCheckoutAmount = GetTotalAmounts() - GetCampaignDiscount();
            return _campaignManager.ApplyCoupon(cartCheckoutAmount, CartCoupon);
        }

        public double GetCampaignDiscount()
        {
            double cartCheckoutAmount = GetTotalAmounts();
            return _campaignManager.ApplyCampaigns(Products, cartCheckoutAmount, Campaigns);
        }

        public double GetTotalAmountAfterDiscount()
        {
            double cartCheckoutAmount = GetTotalAmounts();
            cartCheckoutAmount -= GetCampaignDiscount(); //_campaignManager.ApplyCampaigns(Products, cartCheckoutAmount, Campaigns);
            cartCheckoutAmount -= GetCouponDiscount(); //_campaignManager.ApplyCoupon(cartCheckoutAmount, CartCoupon);

            return cartCheckoutAmount;
        }
        

        public double GetTotalAmounts()
        {
            return Products.Sum(product => product.Value * product.Key.Price);
        }

        private double GetTotalAmountOf(Product product)
        {
            return product.Price * Products[product];
        }


        public void Print()
        {
            List<string> categoryNames = Products.Keys.ToList().GroupBy(product => product.Category.Title).ToList().Select(category => category.Key).ToList();
            const int padding = 20;

            categoryNames.ForEach(categoryName =>
            {
                Console.WriteLine($"\n{"Category" ,padding} {"Product", padding} {"Unit Price", padding} {"Quantity", padding} {"Total Price", padding}");
                Products.Keys.Where(product => product.Category.Title == categoryName).ToList().ForEach(product =>
                {
                    Console.WriteLine($"{categoryName,padding} {product.Title,padding} {product.Price,padding}TL {Products[product],padding} {GetTotalAmountOf(product),padding}TL");
                });
            });

            Console.WriteLine($"\n{"Total Amount", padding * 2} {GetTotalAmounts(), padding * 2}TL");
            Console.WriteLine($"{"Total Discounts", padding * 2} {(GetTotalAmounts() - GetTotalAmountAfterDiscount()), padding * 2}TL");
            Console.WriteLine($"{"Total Amount After Checkout", padding * 2} {GetTotalAmountAfterDiscount(), padding * 2}TL");
        }
    }
}

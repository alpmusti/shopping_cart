using System.Collections.Generic;
using System.Linq;
using Trendyol.ShoppingCart.Contracts;
using Trendyol.ShoppingCart.Models;

namespace Trendyol.ShoppingCart.Manager
{
    public class CampaignManager
    {
        public double ApplyCoupon(double checkoutAmount, Coupon coupon)
        {
            if (checkoutAmount == 0 || coupon == null || checkoutAmount < coupon.MinPrice)
                return 0;

            switch (coupon.TypeOfDiscount)
            {
                case Enums.DiscountType.Amount:
                    return coupon.DiscountAmount;
                case Enums.DiscountType.Rate:
                    return checkoutAmount * (coupon.DiscountAmount / 100);
                default: return 0;
            }
        }

        public double ApplyCampaigns(Dictionary<Product, int> products, double checkoutAmount, List<Campaign> campaigns)
        {
            if (checkoutAmount == 0)
                return 0;

            double discountAmount = 0;
            campaigns.ForEach(campaign =>
            {
                bool isApplicable = false;
                List<Product> campaignProduct = products.Keys.Where(product => product.Category.Title == campaign.Category.Title).ToList();

                if(campaignProduct != null)
                {
                    isApplicable = campaignProduct.Sum(product => products[product]) >= campaign.MinQuantity;
                }


                if (isApplicable)
                {
                    double calculatedDiscount = ApplyCampaign(checkoutAmount, campaign);
                    if (calculatedDiscount > discountAmount)
                    {
                        discountAmount = checkoutAmount - calculatedDiscount;
                    }
                }
            });

            return discountAmount;
        }

        private double ApplyCampaign(double checkoutAmount, Campaign campaign)
        {
            switch (campaign.TypeOfDiscount)
            {
                case Enums.DiscountType.Amount:
                    checkoutAmount -= campaign.Amount;
                    break;
                case Enums.DiscountType.Rate:
                    checkoutAmount -= checkoutAmount * (campaign.Amount / 100);
                    break;
                default: break;
            }

            return checkoutAmount;
        }
    }
}

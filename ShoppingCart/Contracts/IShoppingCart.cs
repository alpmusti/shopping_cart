using Trendyol.ShoppingCart.Models;

namespace Trendyol.ShoppingCart.Contracts
{
    public interface IShoppingCart
    {
        void ApplyDiscounts(params Campaign[] campaigns);
        void ApplyCoupon(Coupon coupon);
        void AddItem(Product product, int quantity);
        double GetDeliveryCost();
        double GetCouponDiscount();
        double GetTotalAmountAfterDiscount();
        double GetCampaignDiscount();
        double GetTotalAmounts();
        double GetNumberOfDeliveries();
        double GetNumberOfProducts();
        void Print();
    }
}

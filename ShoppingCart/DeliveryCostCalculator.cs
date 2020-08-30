using System;
using Trendyol.ShoppingCart.Constants;
using Trendyol.ShoppingCart.Contracts;

namespace Trendyol.ShoppingCart
{
    public class DeliveryCostCalculator : IDeliveryCostCalculator
    {
        private double CostPerDelivery {get; set; }
        private double CostPerProduct { get; set; }

        public DeliveryCostCalculator(double costPerDelivery, double costPerProduct)
        {
            CostPerDelivery = costPerDelivery;
            CostPerProduct = costPerProduct;
        }

        public double CalculateFor(IShoppingCart cart)
        {
            if(cart == null)
            {
                throw new ArgumentNullException("Cart must not be null in order to calculating the delivery cost.");
            }

            return (CostPerDelivery * cart.GetNumberOfDeliveries()) + (CostPerProduct * cart.GetNumberOfProducts()) + CostConstants.FixedRate;
        }
    }
}

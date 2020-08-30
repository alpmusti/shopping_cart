using System;
using Moq;
using Trendyol.ShoppingCart;
using Trendyol.ShoppingCart.Constants;
using Trendyol.ShoppingCart.Contracts;
using Xunit;

namespace TrendyolShoppingCartTests
{
    public class DeliveryCalculatorTests
    {
        DeliveryCostCalculator calculator;
        Mock<IShoppingCart> shoppingCart;

        public DeliveryCalculatorTests()
        {
            shoppingCart = new Mock<IShoppingCart>();
        }

        [Fact]
        public void CalculateFor_NullCart_Should_RaiseException()
        {
            calculator = new DeliveryCostCalculator(5, 10);
            Assert.Throws<ArgumentNullException>(() => calculator.CalculateFor(null));
        }

        [Fact]
        public void CalculateFor_ShouldReturnFixedCost_When_CartIsEmpty()
        {
            calculator = new DeliveryCostCalculator(5, 10);
            shoppingCart.Setup(cart => cart.GetNumberOfDeliveries()).Returns(0);
            shoppingCart.Setup(cart => cart.GetNumberOfProducts()).Returns(0);
            
            Assert.Equal(CostConstants.FixedRate, calculator.CalculateFor(shoppingCart.Object));
        }

        [Fact]
        public void CalculateFor_ShouldReturnValidCost_When_CartHasOneDeliveryAndOneProduct()
        {
            calculator = new DeliveryCostCalculator(5, 10);
            shoppingCart.Setup(cart => cart.GetNumberOfDeliveries()).Returns(1);
            shoppingCart.Setup(cart => cart.GetNumberOfProducts()).Returns(1);

            double expected = (5 * 1) + (10 * 1) + CostConstants.FixedRate;

            Assert.Equal(expected, calculator.CalculateFor(shoppingCart.Object));
        }
    }
}

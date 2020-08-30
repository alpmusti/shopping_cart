using System;
using Moq;
using Trendyol.ShoppingCart;
using Trendyol.ShoppingCart.Contracts;
using Trendyol.ShoppingCart.Models;
using Xunit;

namespace TrendyolShoppingCartTests
{
    public class ShoppingCartTests
    {
        Mock<IDeliveryCostCalculator> calculator;
        ShoppingCart cart;

        public ShoppingCartTests()
        {
            calculator = new Mock<IDeliveryCostCalculator>();
            cart = new ShoppingCart(calculator.Object);
        }

        #region Add Item

        [Fact]
        public void AddItem_ShouldAddSingleItem_When_ItemIsValid()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 3, category);

            cart.AddItem(product, 3);

            Assert.Single(cart.Products);
            Assert.True(cart.Products.ContainsKey(product));
            Assert.True(cart.Products[product] == 3);
        }

        [Fact]
        public void AddItem_ShouldAddSameItemWithDifferentQuantity()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 3, category);

            cart.AddItem(product, 3);
            cart.AddItem(product, 2);

            Assert.Single(cart.Products);
            Assert.True(cart.Products.ContainsKey(product));
            Assert.True(cart.Products[product] == 5);
        }

        [Fact]
        public void AddItem_ShouldAddSameItemWithDifferentInstance()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 3, category);
            Product product2 = new Product("Apple", 13, category);

            cart.AddItem(product, 5);
            cart.AddItem(product2, 3);

            Assert.Equal(2, cart.Products.Count);
            Assert.True(cart.Products.ContainsKey(product));
            Assert.True(cart.Products.ContainsKey(product2));
            Assert.True(cart.Products[product] == 5);
            Assert.True(cart.Products[product2] == 3);
        }


        [Fact]
        public void AddItem_ShouldNotAdd_When_ProductIsNull()
        {
            Product product = null;

            cart.AddItem(product, 100);

            Assert.Empty(cart.Products);
        }

        [Fact]
        public void AddItem_ShouldNotAdd_When_ProductQuantityIsZero()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 13, category);

            cart.AddItem(product, 0);

            Assert.Empty(cart.Products);
        }

        [Fact]
        public void AddItem_ShouldNotAdd_When_QuantityIsNegative()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 13, category);

            cart.AddItem(product, -10);

            Assert.Empty(cart.Products);
        }
        #endregion

        #region Number of Deliveries
        [Fact]
        public void GetNumberOfDeliveries_ShouldReturnZero_When_CartIsEmpty()
        {
            Assert.Equal(0, cart.GetNumberOfDeliveries());
        }

        [Fact]
        public void GetNumberOfDeliveries_ShouldReturnOne_When_SingleCategoryAdded()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 3, category);

            cart.AddItem(product, 3);

            Assert.Equal(1, cart.GetNumberOfDeliveries());
        }

        [Fact]
        public void GetNumberOfDeliveries_ShouldReturnTwo_When_TwoDifferentCategoryAdded()
        {
            Category category = new Category("Food");
            Category category2 = new Category("Beverage");
            Product product = new Product("Apple", 3, category);
            Product product2 = new Product("Coke", 1, category2);

            cart.AddItem(product, 3);
            cart.AddItem(product2, 1);

            Assert.Equal(2, cart.GetNumberOfDeliveries());
        }

        [Fact]
        public void GetNumberOfDeliveries_ShouldReturnOne_When_TwoDifferentItemWithSameCategoryAdded()
        {
            Category category = new Category("Food");
            
            Product product = new Product("Apple", 3, category);
            cart.AddItem(product,1);
            product = new Product("Pear", 5, category);
            cart.AddItem(product, 1);

            Assert.Equal(1, cart.GetNumberOfDeliveries());
        }
        #endregion

        #region Number of Product
        [Fact]
        public void GetNumberOfProducts_ShouldReturnZero_When_CartIsEmpty()
        {
            Assert.Equal(0, cart.GetNumberOfProducts());
        }

        [Fact]
        public void GetNumberOfProducts_ShouldReturnOne_When_SingleItemAdded()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 3, category);

            cart.AddItem(product, 3);

            Assert.Equal(1, cart.GetNumberOfProducts());
        }

        [Fact]
        public void GetNumberOfProducts_ShouldReturnTwo_When_TwoSameItemAdded()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 3, category);
            Product product2 = new Product("Pear", 5, category);

            cart.AddItem(product, 3);
            cart.AddItem(product2, 5);

            Assert.Equal(2, cart.GetNumberOfProducts());
        }

        #endregion

        #region Delivery Cost
        [Fact]
        public void GetDeliveryCost_ShouldReturnMockValueUsingMoq()
        {
            calculator.Setup(calc => calc.CalculateFor(cart)).Returns(3);
            Assert.True(cart.GetDeliveryCost() == 3);
        }
        #endregion

        #region Campaign Discounts
        [Fact]
        public void ApplyDiscounts_ShouldThrowArgumentNullException_When_CampaignsAreNull()
        {
            Assert.Throws<ArgumentNullException>(() => cart.ApplyDiscounts(null));
        }

        [Fact]
        public void GetCampaignDiscount_ShouldCampaignReturnZero_When_CartIsEmpty()
        {
            Assert.Equal(0, cart.GetCampaignDiscount());
        }

        [Fact]
        public void GetCampaignDiscount_ShouldReturnFive_When_OneCampaignOneProductGreaterThanMinimum_AmountType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 100, category);

            cart.AddItem(product, 3);

            Campaign discount = new Campaign(category, discount: 5, minQuantity: 2, Trendyol.ShoppingCart.Enums.DiscountType.Amount);
            cart.ApplyDiscounts(discount);

            Assert.Equal(5, cart.GetCampaignDiscount());
        }

        [Fact]
        public void GetCampaignDiscount_ShouldReturnFive_When_OneCampaignSingleProductGreaterThanMinimum_RateType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);

            cart.AddItem(product, 4);

            Campaign discount = new Campaign(category, discount: 5, minQuantity: 2, Trendyol.ShoppingCart.Enums.DiscountType.Rate);
            cart.ApplyDiscounts(discount);

            Assert.Equal(5, cart.GetCampaignDiscount());
        }

        [Fact]
        public void GetCampaignDiscount_ShouldReturnZero_When_OneCampaignSingleProductLessThanMinimum_AmountType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);

            cart.AddItem(product, 1);

            Campaign discount = new Campaign(category, discount: 5, minQuantity: 2, Trendyol.ShoppingCart.Enums.DiscountType.Amount);
            cart.ApplyDiscounts(discount);

            Assert.Equal(0, cart.GetCampaignDiscount());
        }

        [Fact]
        public void GetCampaignDiscount_ShouldReturnZero_When_OneCampaignSingleProductLessThanMinimum_RateType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);

            cart.AddItem(product, 1);

            Campaign discount = new Campaign(category, discount: 5, minQuantity: 2, Trendyol.ShoppingCart.Enums.DiscountType.Rate);
            cart.ApplyDiscounts(discount);

            Assert.Equal(0, cart.GetCampaignDiscount());
        }

        [Fact]
        public void GetCampaignDiscount_ShouldReturnTen_When_OneCampaignTwoProductGreaterThanMinimum_RateType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);
            Product product2 = new Product("Pear", 50, category);

            cart.AddItem(product, 4);
            cart.AddItem(product2, 2);

            Campaign discount = new Campaign(category, discount: 5, minQuantity: 5, Trendyol.ShoppingCart.Enums.DiscountType.Rate);
            cart.ApplyDiscounts(discount);

            Assert.Equal(10, cart.GetCampaignDiscount());
        }

        [Fact]
        public void GetCampaignDiscount_ShouldReturnFive_When_OneCampaignTwoProductGreaterThanMinimum_AmountType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);
            Product product2 = new Product("Pear", 50, category);

            cart.AddItem(product, 4);
            cart.AddItem(product2, 2);

            Campaign discount = new Campaign(category, discount: 5, minQuantity: 5, Trendyol.ShoppingCart.Enums.DiscountType.Amount);
            cart.ApplyDiscounts(discount);

            Assert.Equal(5, cart.GetCampaignDiscount());
        }

        [Fact]
        public void GetCampaignDiscount_ShouldReturnZero_When_OneCampaignTwoProductLessThanMinimum_RateType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);
            Product product2 = new Product("Pear", 50, category);

            cart.AddItem(product, 2);
            cart.AddItem(product2, 2);

            Campaign discount = new Campaign(category, discount: 5, minQuantity: 5, Trendyol.ShoppingCart.Enums.DiscountType.Rate);
            cart.ApplyDiscounts(discount);

            Assert.Equal(0, cart.GetCampaignDiscount());
        }

        [Fact]
        public void GetCampaignDiscount_ShouldReturnZero_When_OneCampaignTwoProductLessThanMinimum_AmountType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);
            Product product2 = new Product("Pear", 50, category);

            cart.AddItem(product, 2);
            cart.AddItem(product2, 2);

            Campaign discount = new Campaign(category, discount: 5, minQuantity: 5, Trendyol.ShoppingCart.Enums.DiscountType.Amount);
            cart.ApplyDiscounts(discount);

            Assert.Equal(0, cart.GetCampaignDiscount());
        }

        [Fact]
        public void GetCampaignDiscount_ShouldReturnZero_When_TwoCampaignSingleProductLessThanMinimum_AmountType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);

            cart.AddItem(product, 2);

            Campaign discount = new Campaign(category, discount: 5, minQuantity: 5, Trendyol.ShoppingCart.Enums.DiscountType.Amount);
            Campaign discount2 = new Campaign(category, discount: 5, minQuantity: 10, Trendyol.ShoppingCart.Enums.DiscountType.Amount);

            cart.ApplyDiscounts(discount, discount2);

            Assert.Equal(0, cart.GetCampaignDiscount());
        }

        [Fact]
        public void GetCampaignDiscount_ShouldReturnZero_When_TwoCampaignSingleProductLessThanMinimum_RateType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);

            cart.AddItem(product, 2);

            Campaign discount = new Campaign(category, discount: 5, minQuantity: 5, Trendyol.ShoppingCart.Enums.DiscountType.Rate);
            Campaign discount2 = new Campaign(category, discount: 5, minQuantity: 10, Trendyol.ShoppingCart.Enums.DiscountType.Rate);

            cart.ApplyDiscounts(discount, discount2);

            Assert.Equal(0, cart.GetCampaignDiscount());
        }

        [Fact]
        public void GetCampaignDiscount_ShouldReturnFive_When_TwoCampaignSingleProductGreaterThanMinimum_AmountType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);

            cart.AddItem(product, 4);

            Campaign discount = new Campaign(category, discount: 5, minQuantity: 3, Trendyol.ShoppingCart.Enums.DiscountType.Amount);
            Campaign discount2 = new Campaign(category, discount: 25, minQuantity: 10, Trendyol.ShoppingCart.Enums.DiscountType.Amount);

            cart.ApplyDiscounts(discount, discount2);

            Assert.Equal(5, cart.GetCampaignDiscount());
        }

        [Fact]
        public void GetCampaignDiscount_ShouldReturnFive_When_TwoCampaignSingleProductGreaterThanMinimum_RateType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);

            cart.AddItem(product, 4);

            Campaign discount = new Campaign(category, discount: 5, minQuantity: 3, Trendyol.ShoppingCart.Enums.DiscountType.Rate);
            Campaign discount2 = new Campaign(category, discount: 20, minQuantity: 10, Trendyol.ShoppingCart.Enums.DiscountType.Rate);

            cart.ApplyDiscounts(discount, discount2);

            Assert.Equal(5, cart.GetCampaignDiscount());
        }

        [Fact]
        public void GetCampaignDiscount_ShouldReturnZero_When_TwoCampaignTwoProductLessThanMinimum_AmountType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);
            Product product2 = new Product("Pear", 25, category);

            cart.AddItem(product, 2);
            cart.AddItem(product2, 3);

            Campaign discount = new Campaign(category, discount: 5, minQuantity: 6, Trendyol.ShoppingCart.Enums.DiscountType.Amount);
            Campaign discount2 = new Campaign(category, discount: 5, minQuantity: 10, Trendyol.ShoppingCart.Enums.DiscountType.Amount);

            cart.ApplyDiscounts(discount, discount2);

            Assert.Equal(0, cart.GetCampaignDiscount());
        }

        [Fact]
        public void GetCampaignDiscount_ShouldReturnZero_When_TwoCampaignTwoProductLessThanMinimum_RateType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);
            Product product2 = new Product("Pear", 25, category);

            cart.AddItem(product, 2);
            cart.AddItem(product2, 5);

            Campaign discount = new Campaign(category, discount: 5, minQuantity: 8, Trendyol.ShoppingCart.Enums.DiscountType.Rate);
            Campaign discount2 = new Campaign(category, discount: 5, minQuantity: 10, Trendyol.ShoppingCart.Enums.DiscountType.Rate);

            cart.ApplyDiscounts(discount, discount2);

            Assert.Equal(0, cart.GetCampaignDiscount());
        }

        [Fact]
        public void GetCampaignDiscount_ShouldReturnTen_When_TwoCampaignTwoProductGreaterThanMinimum_AmountType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);
            Product product2 = new Product("Pear", 50, category);

            cart.AddItem(product, 4);
            cart.AddItem(product2, 2);

            Campaign discount = new Campaign(category, discount: 10, minQuantity: 3, Trendyol.ShoppingCart.Enums.DiscountType.Amount);
            Campaign discount2 = new Campaign(category, discount: 25, minQuantity: 10, Trendyol.ShoppingCart.Enums.DiscountType.Amount);

            cart.ApplyDiscounts(discount, discount2);

            Assert.Equal(10, cart.GetCampaignDiscount());
        }

        [Fact]
        public void GetCampaignDiscount_ShouldReturnFifteen_When_TwoCampaignTwoProductGreaterThanMinimum_RateType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);
            Product product2 = new Product("Pear", 50, category);

            cart.AddItem(product, 4);
            cart.AddItem(product2, 4);

            Campaign discount = new Campaign(category, discount: 5, minQuantity: 3, Trendyol.ShoppingCart.Enums.DiscountType.Rate);
            Campaign discount2 = new Campaign(category, discount: 20, minQuantity: 10, Trendyol.ShoppingCart.Enums.DiscountType.Rate);

            cart.ApplyDiscounts(discount, discount2);

            Assert.Equal(15, cart.GetCampaignDiscount());
        }
        #endregion

        #region Coupon Discounts
        [Fact]
        public void ApplyCoupon_ShouldThrowArgumentNullException_When_CouponIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => cart.ApplyCoupon(null));
        }

        [Fact]
        public void GetCouponDiscount_ShouldReturnZero_When_CartIsEmpty()
        {
            Assert.True(cart.GetCouponDiscount() == 0);
        }

        [Fact]
        public void GetCouponDiscount_ShouldReturnZero_When_SingleProduct_LessThanMinimum_AmountType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 10, category);

            cart.AddItem(product, 4);

            Coupon coupon = new Coupon(minPrice: 100, amount: 5, Trendyol.ShoppingCart.Enums.DiscountType.Amount);
            cart.ApplyCoupon(coupon);

            Assert.Equal(0, cart.GetCouponDiscount());
        }

        [Fact]
        public void GetCouponDiscount_ShouldReturnZero_When_SingleProduct_LessThanMinimum_RateType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 5, category);

            cart.AddItem(product, 4);

            Coupon coupon = new Coupon(minPrice: 100, amount: 5, Trendyol.ShoppingCart.Enums.DiscountType.Rate);
            cart.ApplyCoupon(coupon);

            Assert.Equal(0, cart.GetCouponDiscount());
        }

        [Fact]
        public void GetCouponDiscount_ShouldReturnZero_When_TwoProductLessThanMinimum_AmountType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 10, category);
            Product product2 = new Product("Pear", 10, category);

            cart.AddItem(product, 4);
            cart.AddItem(product2, 2);

            Coupon coupon = new Coupon(minPrice: 100, amount: 5, Trendyol.ShoppingCart.Enums.DiscountType.Amount);
            cart.ApplyCoupon(coupon);

            Assert.Equal(0, cart.GetCouponDiscount());
        }

        [Fact]
        public void GetCouponDiscount_ShouldReturnZero_When_TwoProductLessThanMinimum_RateType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 10, category);
            Product product2 = new Product("Pear", 10, category);

            cart.AddItem(product, 4);
            cart.AddItem(product2, 2);

            Coupon coupon = new Coupon(minPrice: 100, amount: 5, Trendyol.ShoppingCart.Enums.DiscountType.Rate);
            cart.ApplyCoupon(coupon);

            Assert.Equal(0, cart.GetCouponDiscount());
        }

        [Fact]
        public void GetCouponDiscount_ShouldReturnFive_When_SingleProductGreaterThanMinimum_AmountType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);

            cart.AddItem(product, 4);

            Coupon coupon = new Coupon(minPrice: 50, amount: 5, Trendyol.ShoppingCart.Enums.DiscountType.Amount);
            cart.ApplyCoupon(coupon);

            Assert.Equal(5, cart.GetCouponDiscount());
        }

        [Fact]
        public void GetCouponDiscount_ShouldReturnFive_When_SingleProductGreaterThanMinimum_RateType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);

            cart.AddItem(product, 4);

            Coupon coupon = new Coupon(minPrice: 50, amount: 5, Trendyol.ShoppingCart.Enums.DiscountType.Rate);
            cart.ApplyCoupon(coupon);

            Assert.Equal(5, cart.GetCouponDiscount());
        }

        [Fact]
        public void GetCouponDiscount_ShouldReturnFive_When_TwoProductGreaterThanMinimum_AmountType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);
            Product product2 = new Product("Pear", 50, category);

            cart.AddItem(product, 2);
            cart.AddItem(product2, 1);

            Coupon coupon = new Coupon(minPrice: 100, amount: 5, Trendyol.ShoppingCart.Enums.DiscountType.Amount);
            cart.ApplyCoupon(coupon);

            Assert.Equal(5, cart.GetCouponDiscount());
        }

        [Fact]
        public void GetCouponDiscount_ShouldReturnFive_When_TwoProductGreaterThanMinimum_RateType()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);
            Product product2 = new Product("Pear", 50, category);

            cart.AddItem(product, 2);
            cart.AddItem(product2, 1);

            Coupon coupon = new Coupon(minPrice: 100, amount: 5, Trendyol.ShoppingCart.Enums.DiscountType.Rate);
            cart.ApplyCoupon(coupon);

            Assert.Equal(5, cart.GetCouponDiscount());
        }
        #endregion

        #region Total Amount
        [Fact]
        public void GetTotalAmounts_ShouldReturnZero_When_CartIsEmpty()
        {
            Assert.Equal(0, cart.GetTotalAmounts());
        }

        [Fact]
        public void GetTotalAmounts_ShouldReturnThreeOneProductAdded()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 3, category);

            cart.AddItem(product, 1);

            Assert.Equal(3, cart.GetTotalAmounts());
        }

        [Fact]
        public void GetTotalAmounts_ShouldReturnTenTwoProductAdded()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 3, category);
            Product product2 = new Product("Pear", 7, category);

            cart.AddItem(product, 1);
            cart.AddItem(product2, 1);

            Assert.Equal(10, cart.GetTotalAmounts());
        }

        [Fact]
        public void GetTotalAmountAfterDiscount_ShouldReturnZero_When_CartIsEmpty()
        {
            Assert.True(cart.GetTotalAmountAfterDiscount() == 0);
        }

        [Fact]
        public void GetTotalAmountAfterDiscount_ShouldReturnTotalAmount_When_NoCampaignApplied()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);

            cart.AddItem(product, 4);

            Assert.True(cart.GetTotalAmounts() == cart.GetTotalAmountAfterDiscount());
        }

        [Fact]
        public void GetTotalAmountAfterDiscount_ShouldReturnSameWithTotalAmount_When_NotApplicableCampaignAdded()
        {
            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);

            cart.AddItem(product, 4);

            Campaign discount = new Campaign(category, discount: 50, minQuantity: 5, Trendyol.ShoppingCart.Enums.DiscountType.Amount);
            cart.ApplyDiscounts(discount);

            Assert.Equal(cart.GetTotalAmounts(), cart.GetTotalAmountAfterDiscount());
        }

        [Fact]
        public void GetTotalAmountAfterDiscount_ShouldReturnTotalMinusCampaignDiscount_When_OneCampaignSingleProductNoCoupon_AmounType()
        {

            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);

            cart.AddItem(product, 4);

            Campaign discount = new Campaign(category, discount: 50, minQuantity: 2, Trendyol.ShoppingCart.Enums.DiscountType.Amount);
            cart.ApplyDiscounts(discount);

            double expected = cart.GetTotalAmounts() - cart.GetCampaignDiscount();

            Assert.Equal(expected, cart.GetTotalAmountAfterDiscount());
        }

        [Fact]
        public void GetTotalAmountAfterDiscount_ShouldReturnTotalMinusCampaignDiscount_When_OneCampaignSingleProductNoCoupon_RateType()
        {

            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);

            cart.AddItem(product, 4);

            Campaign discount = new Campaign(category, discount: 10, minQuantity: 2, Trendyol.ShoppingCart.Enums.DiscountType.Rate);
            cart.ApplyDiscounts(discount);

            double expected = cart.GetTotalAmounts() - cart.GetCampaignDiscount();

            Assert.Equal(expected, cart.GetTotalAmountAfterDiscount());
        }

        [Fact]
        public void GetTotalAmountAfterDiscount_ShouldReturnTotalMinusCampaignDiscount_When_OneCampaignSingleProductOneCoupon_AmounType()
        {

            Category category = new Category("Food");
            Product product = new Product("Apple", 25, category);

            cart.AddItem(product, 4);

            Campaign discount = new Campaign(category, discount: 50, minQuantity: 2, Trendyol.ShoppingCart.Enums.DiscountType.Amount);
            cart.ApplyDiscounts(discount);

            Coupon coupon = new Coupon(minPrice: 100, amount: 5, Trendyol.ShoppingCart.Enums.DiscountType.Amount);
            cart.ApplyCoupon(coupon);

            double expected = cart.GetTotalAmounts() - (cart.GetCampaignDiscount() + cart.GetCouponDiscount());

            Assert.Equal(expected, cart.GetTotalAmountAfterDiscount());
        }

        [Fact]
        public void GetTotalAmountAfterDiscount_ShouldReturnTotalMinusCampaignDiscount_When_OneCampaignSingleProductOneCoupon_RateType()
        {

            Category category = new Category("Food");
            Product product = new Product("Apple", 50, category);

            cart.AddItem(product, 4);

            Campaign discount = new Campaign(category, discount: 5, minQuantity: 2, Trendyol.ShoppingCart.Enums.DiscountType.Rate);
            cart.ApplyDiscounts(discount);

            Coupon coupon = new Coupon(minPrice: 100, amount: 5, Trendyol.ShoppingCart.Enums.DiscountType.Rate);
            cart.ApplyCoupon(coupon);

            double expected = cart.GetTotalAmounts() - (cart.GetCampaignDiscount() + cart.GetCouponDiscount());

            Assert.Equal(expected, cart.GetTotalAmountAfterDiscount());
        }
        #endregion

    }
}

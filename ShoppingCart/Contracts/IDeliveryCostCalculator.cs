namespace Trendyol.ShoppingCart.Contracts
{
    public interface IDeliveryCostCalculator
    {
        double CalculateFor(IShoppingCart cart);
    }
}

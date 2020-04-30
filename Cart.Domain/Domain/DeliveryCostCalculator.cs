using System.Linq;

namespace Cart.Domain.Domain
{
    public class DeliveryCostCalculator
    {
        public double CostPerDelivery { get; set; }
        public double CostPerProduct { get; set; }
        public double FixedCost { get; set; }
        
        public double CalculateDeliveryCost(ShoppingCart cart)
        {
            var numberOfProducts = cart.CartItems
                .Select(x => x.Product.Id).Distinct().Count();
            var numberOfDeliveries = cart.CartItems
                .Select(x => x.Product.Category.Id).Distinct().Count();

            return (CostPerDelivery * numberOfDeliveries) + (CostPerProduct * numberOfProducts) + FixedCost;
        }
    }
}
using Cart.Domain.Enum;

namespace Cart.Domain.Domain
{
    public class Discount
    {
        public double Amount { get; set; }
        public DiscountType Type { get; set; }
    }
}
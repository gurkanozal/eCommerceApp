using System;
using Cart.Domain.Shared;

namespace Cart.Domain.Domain
{
    public class Coupon: Entity<Guid>
    {
        public Guid ShoppingCartId { get; set; }
        public double MinPurchaseAmount { get; set; }
        public Discount Discount { get; set; } 
    }
}
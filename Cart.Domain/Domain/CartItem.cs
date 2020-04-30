using System;
using Cart.Domain.Shared;

namespace Cart.Domain.Domain
{
    public class CartItem: Entity<Guid>
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        
        internal void IncreaseCount(int count)
        {
            Quantity += count;
        }
    }
}
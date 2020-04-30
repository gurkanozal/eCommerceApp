using System;
using Cart.Domain.Shared;

namespace Cart.Domain.Domain
{
    public class Campaign: Entity<Guid>
    {
        public Category Category { get; set; }
        public int MinProductQuantity { get; set; }
        public Discount Discount { get; set; }
    }
}
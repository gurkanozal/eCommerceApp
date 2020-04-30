using System;
using System.Collections.Generic;
using Cart.Domain.Shared;

namespace Cart.Domain.Domain
{
    public class Product : Entity<Guid>
    {
        public string Title { get; set; }
        public double Price { get; set; }
        public Category Category { get; set; }
    }
}
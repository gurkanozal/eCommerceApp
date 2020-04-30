using System.Collections.Generic;
using Cart.Domain.Domain;

namespace Cart.Application.Dtos
{
    public class PrintShoppingCartDto
    {
        public List<CategorizedProductsDto> CategorizedProducts { get; set; }
        public double TotalPrice { get; set; }
        public double TotalDiscount { get; set; }
        public double TotalAmount => TotalPrice - TotalDiscount;

        public double DeliveryCost { get; set; }
    }

    public class CategorizedProductsDto
    {
        public string CategoryName { get; set; }
        public List<Product> Products { get; set; }
    }
}
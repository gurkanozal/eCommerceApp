using System;
using System.Collections.Generic;
using System.Linq;
using Cart.Domain.Enum;
using Cart.Domain.Exception;
using Cart.Domain.Shared;

namespace Cart.Domain.Domain
{
    public class ShoppingCart: Entity<Guid>
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        public double GetCampaignDiscount(List<Campaign> campaigns)
        {
            var groupedCampaignsByCategory = campaigns.GroupBy(x => x.Category);
            return (
                    from groupedCampaign in groupedCampaignsByCategory
                    let categorizedCartItems = CartItems.Where(x => x.Product.Category.Id == groupedCampaign.Key.Id)
                        .ToList()
                    where categorizedCartItems.Any()
                    let itemCount = categorizedCartItems.Sum(x => x.Quantity)
                    let applicableCampaigns = groupedCampaign.Where(x => x.MinProductQuantity <= itemCount).ToList()
                    where applicableCampaigns.Any()
                    from cartItem in categorizedCartItems
                    select GetMaxDiscountAmountForCartItem(applicableCampaigns, cartItem)).Concat(new double[] {0})
                .Max();
        }

        public double GetCouponDiscount(Coupon coupon, double campaignDiscount)
        {
            var totalAmountWithCampaignDiscount = CartItems.Sum(x => x.Product.Price * x.Quantity) - campaignDiscount;
            if (!(totalAmountWithCampaignDiscount >= coupon.MinPurchaseAmount))
                throw new DomainException("Cart total is less than minimum amount! Coupon is not applicable.",
                    new InvalidOperationException());
            return coupon.Discount.Type switch
            {
                DiscountType.Rate => (coupon.Discount.Amount / 100 * totalAmountWithCampaignDiscount),
                DiscountType.Amount => coupon.Discount.Amount,
                _ => default
            };
        }
        public void AddItem(CartItem cartItem)
        {
            if (cartItem is null)
            {
                throw new DomainException("Cart item cannot be null!", new InvalidOperationException());
            }

            var existingCartItem = CartItems.SingleOrDefault(
                p => p.Product.Equals(cartItem.Product)
            );


            if (existingCartItem is null)
            {
                CartItems.Add(cartItem);
            }
            else
            {
                existingCartItem.IncreaseCount(cartItem.Quantity);
            }
        }
        
        private double GetMaxDiscountAmountForCartItem(List<Campaign> campaigns, CartItem cartItem)
        {
            return campaigns.SelectMany(x => new List<double>
            {
                x.Discount.Type == DiscountType.Rate
                    ? (x.Discount.Amount / 100) * cartItem.Product.Price
                    : x.Discount.Amount
            }).Max(x => x);
        }

    }
}
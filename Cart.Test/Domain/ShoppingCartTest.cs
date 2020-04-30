using System;
using System.Collections.Generic;
using System.Linq;
using Cart.Domain.Domain;
using Cart.Domain.Enum;
using Xunit;

namespace Cart.Test.Domain
{
    public class ShoppingCartTest
    {
        [Fact]
        public void Cart_should_apply_the_maximum_amount_of_discount_to_the_product()
        {
            //Arrange
            var shoppingCart = new ShoppingCart();
            shoppingCart.AddItem(new CartItem
            {
                Id = Guid.NewGuid(),
                Product = GetFakeApple(),
                Quantity = 4
            });
            shoppingCart.AddItem(new CartItem
            {
                Id = Guid.NewGuid(),
                Product = GetFakeAlmond(),
                Quantity = 3
            });
            
            //Act
            var campaignDiscount = shoppingCart.GetCampaignDiscount(GetFakeCampaigns());
            
            //Assert
            Assert.Equal(75,campaignDiscount);
        }
        
        [Fact(DisplayName = "Cart should not apply the discount because of quantity")]
        public void Cart_should_not_apply_the_discount()
        {
            //Arrange
            var shoppingCart = new ShoppingCart();
            shoppingCart.AddItem(new CartItem
            {
                Id = Guid.NewGuid(),
                Product = GetFakeApple(),
                Quantity = 1
            });
            shoppingCart.AddItem(new CartItem
            {
                Id = Guid.NewGuid(),
                Product = GetFakeAlmond(),
                Quantity = 1
            });
            
            //Act
            var campaignDiscount = shoppingCart.GetCampaignDiscount(GetFakeCampaigns());
            
            //Assert
            Assert.Equal(0,campaignDiscount);
        }
        
        [Fact(DisplayName = "Cart should apply the campaign1 because of quantity")]
        public void Cart_should_apply_the_campaign1()
        {
            //Arrange
            var shoppingCart = new ShoppingCart();
            shoppingCart.AddItem(new CartItem
            {
                Id = Guid.NewGuid(),
                Product = GetFakeApple(),
                Quantity = 2
            });
            shoppingCart.AddItem(new CartItem
            {
                Id = Guid.NewGuid(),
                Product = GetFakeAlmond(),
                Quantity = 1
            });
            
            //Act
             var campaignDiscount = shoppingCart.GetCampaignDiscount(GetFakeCampaigns());
            
            //Assert
            Assert.Equal(30,campaignDiscount);
        }
        
        [Fact]
        public void Cart_should_apply_the_coupon()
        {
            //Arrange
            var shoppingCart = new ShoppingCart();
            shoppingCart.AddItem(new CartItem
            {
                Id = Guid.NewGuid(),
                Product = GetFakeApple(),
                Quantity = 2
                
            });
            shoppingCart.AddItem(new CartItem
            {
                Id = Guid.NewGuid(),
                Product = GetFakeAlmond(),
                Quantity = 1
                
            });
            
            //Act
            var couponDiscount = shoppingCart.GetCouponDiscount(GetFakeCoupon(), 0);
            
            //Assert
            Assert.Equal(35,couponDiscount);
        }
        
        [Fact]
        public void Calculate_delivery_cost_successfully()
        {
            //Arrange
            var shoppingCart = new ShoppingCart();
            shoppingCart.AddItem(new CartItem
            {
                Id = Guid.NewGuid(),
                Product = GetFakeApple(),
                Quantity = 2
                
            });
            shoppingCart.AddItem(new CartItem
            {
                Id = Guid.NewGuid(),
                Product = GetFakeAlmond(),
                Quantity = 1
                
            });
            //NumberOfProduct is 2 and NumberOfDelivery is 1 
            //Formula is (CostPerDelivery * NumberOfDeliveries) + (CostPerProduct * NumberOfProduct) + FixedCost
            //So result is (8 * 1) + (6 * 2) + 2.99 = 22.99
            var deliveryCostCalculator = new DeliveryCostCalculator
            {
                FixedCost = 2.99,
                CostPerDelivery = 8.0,
                CostPerProduct = 6.0
            };
            //Act
            var deliveryCost = deliveryCostCalculator.CalculateDeliveryCost(shoppingCart);
            
            //Assert
            Assert.Equal(22.99, Math.Round(deliveryCost, 2));
        }
        private Category GetFakeCategory()
        {
            return new Category
            {
                Id = 1,
                Title = "Food"
            };
        }
        private Product GetFakeApple()
        {
            return  new Product
            {
                Id = Guid.NewGuid(),
                Price = 100.0,
                Title = "Apple",
                Category = GetFakeCategory()
            };
        }
        private Product GetFakeAlmond()
        {
            return new Product
            {
                Id = Guid.NewGuid(),
                Price = 150.0,
                Title = "Almond",
                Category = GetFakeCategory()
            };
        }
        public List<Campaign> GetFakeCampaigns()
        {
            var category = GetFakeCategory();
            var campaign1 = new Campaign
            {
                Id = Guid.NewGuid(),
                Category = category,
                Discount = new Discount
                {
                    Amount = 20.0,
                    Type = DiscountType.Rate
                },
                MinProductQuantity = 3
            };
            var campaign2 = new Campaign
            {
                Id = Guid.NewGuid(),
                Category = category,
                Discount = new Discount
                {
                    Amount = 50.0,
                    Type = DiscountType.Rate
                },
                MinProductQuantity = 5
            };
            var campaign3 = new Campaign
            {
                Id = Guid.NewGuid(),
                Category = category,
                Discount = new Discount
                {
                    Amount = 5.0,
                    Type = DiscountType.Amount
                },
                MinProductQuantity = 5
            };

            return new List<Campaign> {campaign1, campaign2, campaign3};
        }

        private Coupon GetFakeCoupon()
        {
            return new Coupon
            {
                Discount = new Discount
                {
                    Amount = 10,
                    Type = DiscountType.Rate
                },
                MinPurchaseAmount = 100
            };
        }
    }
}
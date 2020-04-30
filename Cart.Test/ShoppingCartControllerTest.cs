using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cart.API.Controllers;
using Cart.Domain;
using Cart.Domain.Domain;
using Cart.Domain.Enum;
using Cart.Test.Domain;
using Moq;
using Xunit;

namespace Cart.Test
{
    public class ShoppingCartControllerTest
    {
        private readonly Mock<IRepository<ShoppingCart, Guid>> _shoppingCartRepositoryMock;
        private readonly Mock<IRepository<Campaign, Guid>> _campaignRepositoryMock;
        private readonly Mock<IRepository<Coupon, Guid>> _couponRepositoryMock;

        public ShoppingCartControllerTest()
        {
            _campaignRepositoryMock = new Mock<IRepository<Campaign, Guid>>();
            _shoppingCartRepositoryMock = new Mock<IRepository<ShoppingCart, Guid>>();
            _couponRepositoryMock = new Mock<IRepository<Coupon, Guid>>();
        }

        [Fact]
        public async Task Get_total_amount_after_discounts_successfully()
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


            _shoppingCartRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).Returns(Task.FromResult(shoppingCart));
            _campaignRepositoryMock.Setup(x => x.GetAll())
                .Returns(GetFakeCampaigns().AsQueryable);
            _couponRepositoryMock.Setup(x => x.GetAll())
                .Returns(GetFakeCoupons().AsQueryable);

            //Act
            var shoppingCartController = new CartController(_shoppingCartRepositoryMock.Object, _campaignRepositoryMock.Object,_couponRepositoryMock.Object);
            var totalAmountAfterDiscounts = await shoppingCartController.GetTotalAmountAfterDiscounts(Guid.NewGuid());
            
            //Assert
            Assert.Equal(697.5, totalAmountAfterDiscounts);
        }

        private IEnumerable<Coupon> GetFakeCoupons()
        {
            return new List<Coupon>
            {
               new Coupon
               {
                   Discount = new Discount
                   {
                       Amount = 10,
                       Type = DiscountType.Rate
                   },
                   MinPurchaseAmount = 100 
               }
            };
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
            return new Product
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
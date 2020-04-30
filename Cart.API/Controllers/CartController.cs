using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cart.API.Startup;
using Cart.Application.Dtos;
using Cart.Domain;
using Cart.Domain.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Controllers
{
    [ApiController,
     Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly IRepository<ShoppingCart, Guid> _shoppingCartRepository;
        private readonly IRepository<Campaign, Guid> _campaignRepository;
        private readonly IRepository<Coupon, Guid> _couponRepository;

        public CartController(IRepository<ShoppingCart, Guid> shoppingCartRepository, IRepository<Campaign, Guid> campaignRepository, IRepository<Coupon, Guid> couponRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _campaignRepository = campaignRepository;
            _couponRepository = couponRepository;
        }

        [HttpGet("getTotalAmountAfterDiscounts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DomainProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<double> GetTotalAmountAfterDiscounts(Guid shoppingCartId)
        {
           var shoppingCart = await _shoppingCartRepository.GetById(shoppingCartId);
           var categoryIds = shoppingCart.CartItems.Select(x => x.Product.Category.Id).ToList();
           var campaigns = _campaignRepository.GetAll().Where(x => categoryIds.Contains(x.Category.Id)).ToList();
           var coupon = _couponRepository.GetAll().FirstOrDefault(x => x.ShoppingCartId == shoppingCart.Id);
           double campaignDiscount = 0;
           double couponDiscount = 0;
           if(campaigns.Any())
               campaignDiscount = shoppingCart.GetCampaignDiscount(campaigns);

           if (coupon != null)
               couponDiscount = shoppingCart.GetCouponDiscount(coupon, campaignDiscount);
           var totalAmountBeforeDiscount = shoppingCart.CartItems.Sum(x => x.Product.Price * x.Quantity);
           
           return totalAmountBeforeDiscount - campaignDiscount - couponDiscount;
        }
        
        [HttpGet("getCouponDiscount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DomainProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<double> GetCouponDiscount(Guid shoppingCartId)
        {
            var shoppingCart = await _shoppingCartRepository.GetById(shoppingCartId);
            var categoryIds = shoppingCart.CartItems.Select(x => x.Product.Category.Id).ToList();
            var campaigns = _campaignRepository.GetAll().Where(x => categoryIds.Contains(x.Category.Id)).ToList();
            var coupon = _couponRepository.GetAll().FirstOrDefault(x => x.ShoppingCartId == shoppingCart.Id);
            double campaignDiscount = 0;
            double couponDiscount = 0;
            if(campaigns.Any())
                campaignDiscount = shoppingCart.GetCampaignDiscount(campaigns);

            if (coupon != null)
                couponDiscount = shoppingCart.GetCouponDiscount(coupon, campaignDiscount);
           
            return couponDiscount;
        }
        
        [HttpGet("getCampaignDiscount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DomainProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<double> GetCampaignDiscount(Guid shoppingCartId)
        {
            var shoppingCart = await _shoppingCartRepository.GetById(shoppingCartId);
            return CampaignDiscount(shoppingCart);
        }

        [HttpGet("getDeliveryCost")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DomainProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<double> GetDeliveryCost(Guid shoppingCartId)
        {
            var shoppingCart = await _shoppingCartRepository.GetById(shoppingCartId);
            
            return CalculateDeliveryCost(shoppingCart);
        }

        [HttpGet("print")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DomainProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<List<PrintShoppingCartDto>> Print(Guid shoppingCartId)
        {
            var shoppingCart = await _shoppingCartRepository.GetById(shoppingCartId);
            var categories = shoppingCart.CartItems.Select(x => x.Product.Category).Distinct().ToList();

            return categories.Select(category => new PrintShoppingCartDto
                {
                    CategorizedProducts = new List<CategorizedProductsDto>
                    {
                        new CategorizedProductsDto
                        {
                            Products = shoppingCart.CartItems.Where(x => x.Product.Category.Id == category.Id).Select(x => x.Product).ToList(),
                            CategoryName = category.Title
                        }
                    }, 
                    DeliveryCost = CalculateDeliveryCost(shoppingCart),
                    TotalDiscount = CampaignDiscount(shoppingCart),
                    TotalPrice = shoppingCart.CartItems.Where(x => x.Product.Category.Id == category.Id).Sum(x => x.Product.Price * x.Quantity),
                })
                .ToList();
        }
        
        private static double CalculateDeliveryCost(ShoppingCart shoppingCart)
        {
            //TODO: Delivery costs should be dynamic
            var deliveryCostCalculator = new DeliveryCostCalculator
            {
                FixedCost = 2.99,
                CostPerDelivery = 8,
                CostPerProduct = 6
            };

            return deliveryCostCalculator.CalculateDeliveryCost(shoppingCart);
        }
        
        private double CampaignDiscount(ShoppingCart shoppingCart)
        {
            var categoryIds = shoppingCart.CartItems.Select(x => x.Product.Category.Id).ToList();
            var campaigns = _campaignRepository.GetAll().Where(x => categoryIds.Contains(x.Category.Id)).ToList();
            return campaigns.Any() ? shoppingCart.GetCampaignDiscount(campaigns) : default;
        }
    }
}
using eCommerce.DTOs;
using eCommerce.Entities;
using Microsoft.AspNetCore.Mvc;


namespace eCommerce.Controllers
{

    public class BasketController : BaseApiController
    {
        private readonly StoreContext context;

        public BasketController(StoreContext context)
        {
            this.context = context;
        }

        [HttpGet]

        public async Task<ActionResult<BasketDto>> GetBasket()
        {
            var basket = await RetrieveBasket();

            if (basket == null) return NotFound();

            return new BasketDto
            {
                Id = basket.Id,
                BuyerId = basket.BuyerId,
                Items = basket.Items.Select(item => new BasketItemDto
                {
                    ProductId = item.ProductId,
                    Name = item.Product.Name,
                    Price = item.Product.Price,
                    PictureUrl = item.Product.PictureUrl,
                    Type = item.Product.Type,
                    Brand = item.Product.Brand,
                    Quantity = item.Quantity
                }).ToList()
            };
        }



        [HttpPut]

        public async Task<ActionResult> AddItemBasket(int productId, int quantity)
        {
            //get basket
            var basket = await RetrieveBasket();

            //create basket
            if (basket == null) basket = CreateBasket();

            //get product
            var product = await context.Products.FindAsync(productId);
            if (productId == null) return NotFound();

            //add item
            basket.AddItem(product, quantity);

            //save changes
            var result = await context.SaveChangesAsync() > 0;
            if (result) return StatusCode(201);

            return BadRequest(new ProblemDetails { Title = "Problem saving item to basket" });

        }



        [HttpDelete]

        public async Task<ActionResult> RemoveBasketItem(int productId, int quantity)
        {

            //get basket
            var basket = await RetrieveBasket();

            if (basket == null) return NotFound();

            //remove item or reduce quantity
            basket.RemoveItem(productId, quantity);

            //save changes

            var result = await context.SaveChangesAsync() > 0;

            if (result) return Ok();
            return BadRequest(new ProblemDetails { Title = "Problem removing item from the basket" });

        }

        private async Task<Basket> RetrieveBasket()
        {
            return await context.Baskets
            .Include(i => i.Items)
            .ThenInclude(p => p.Product)
            .FirstOrDefaultAsync(x => x.BuyerId == Request.Cookies["buyerId"]);
        }

        private Basket CreateBasket()
        {
            var buyerId = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions { IsEssential = true, Expires = DateTime.Now.AddDays(30) };
            Response.Cookies.Append("buyerId", buyerId, cookieOptions);
            var basket = new Basket { BuyerId = buyerId };
            context.Baskets.Add(basket);
            return basket;
        }

    }
}
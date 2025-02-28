using MantisWebshop.Server.Sql.Data;
using MantisWebshop.Server.Sql.Extensions;
using MantisWebshop.Server.Sql.Models;
using MantisWebshop.Server.Sql.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MantisWebshop.Server.Sql.Controllers
{
    [ApiController]
    public class ShopController : MantisShopControllerBase<ShopController>
    {
        public ShopController(ILogger<ShopController> logger, MantisWebshopDbContext dbContext)
            : base(logger, dbContext)
        { }

        [HttpGet]
        [Route("[controller]")]
        public async Task<ActionResponse> GetProducts([FromQuery(Name = "page")] int page = 1, [FromQuery(Name = "perPage")] int itemsPerPage = 10)
        {
            try
            {
                return GetResponse(200, "Ok", (await DbContext.Products.Skip(PageOffset(page, itemsPerPage)).Take(itemsPerPage).ToListAsync()).Select(ModelDtoExtensions.ToProductDto));
            }
            catch (Exception ex)
            {
                Logger.LogError("Cannot fetch the products", ex.Message, ex.StackTrace);
                return GetResponse(500, "Unknown error");
            }
        }

        [HttpGet]
        [Route("[controller]/{id}")]
        public async Task<ActionResponse> GetProduct(string id)
        {
            try
            {
                var product = await DbContext.Products.SingleOrDefaultAsync(x => x.Id.ToString().Equals(id));
                if (product is null)
                    return GetResponse(404, "Product cannot be found", id);
                else
                    return GetResponse(200, "Ok", product.ToProductDto());
            }
            catch (Exception ex)
            {
                Logger.LogError($"Cannot fetch product with id {id}", ex.Message, ex.StackTrace);
                return GetResponse(500, "Unknown error");
            }
        }

        [HttpGet]
        [Route("[controller]/cart")]
        [Authorize]
        public async Task<ActionResponse> GetCart()
        {
            var user = await TryGetUserAsync();
            if (user is null)
                return GetResponse("Invalid credentials");

            try
            {
                return GetResponse(200, "Ok", user.CartItems?.Select(ModelDtoExtensions.ToCartItemDto));
            }
            catch (Exception ex)
            {
                Logger.LogError($"Cannot obtain cart for user {user.Id}", ex.Message, ex.StackTrace);
                return GetResponse(500, "Unknown error");
            }
        }

        [HttpPost]
        [Route("[controller]/cart")]
        [Authorize]
        public async Task<ActionResponse> PostAddToCart(CartInput cartInput)
        {
            var user = await TryGetUserAsync();
            if (user is null)
                return GetResponse("Invalid credentials");

            try
            {
                var product = await DbContext.Products.SingleOrDefaultAsync(x => x.Id.ToString().Equals(cartInput.Id));
                if (product is null)
                    return GetResponse(404, "Product cannot be found");

                int statusCode = 200;
                if (!(user.CartItems?.Any(x => x.Product.Id.Equals(product.Id)) ?? false))
                {
                    if (cartInput.Quantity <= 0)
                        return GetResponse(200, "Ok");

                    //user.CartItems ??= new List<CartItem>();
                    await DbContext.CartItems.AddAsync(
                        new CartItem
                        {
                            Id = Guid.NewGuid(),
                            Product = product,
                            Quantity = cartInput.Quantity,
                            User = user
                        });

                    await DbContext.SaveChangesAsync();
                    statusCode = 201;
                }
                else
                {
                    var cartItem = user.CartItems.Single(x => x.Product.Id.Equals(product.Id));
                    cartItem.Quantity = cartInput.Override ? cartInput.Quantity : cartItem.Quantity + cartInput.Quantity;
                    if (cartItem.Quantity <= 0)
                        user.CartItems.Remove(cartItem);

                    await DbContext.SaveChangesAsync();
                }

                return GetResponse(statusCode, "Ok");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Cannot add product {cartInput.Id} to cart for user {user.Id}", ex.Message, ex.StackTrace);
                return GetResponse(500, "Unknown error");
            }
        }

        [HttpGet]
        [Route("[controller]/orders")]
        [Authorize]
        public async Task<ActionResponse> GetOrders()
        {
            var user = await TryGetUserAsync();
            if (user is null)
                return GetResponse("Invalid credentials");

            try
            {
                return GetResponse(200, "Ok", user.Orders?.Select(ModelDtoExtensions.ToOrderDto));
            }
            catch (Exception ex)
            {
                Logger.LogError($"Cannot obtain orders of user [{user.Id}]", ex.Message, ex.StackTrace);
                return GetResponse(500, "Unknown error");
            }
        }

        [HttpPost]
        [Route("[controller]/checkout")]
        [Authorize]
        public async Task<ActionResponse> PostCheckout()
        {
            var user = await TryGetUserAsync();
            if (user is null)
                return GetResponse("Invalid credentials");

            try
            {
                if ((user.CartItems?.Count ?? 0) == 0)
                {
                    Logger.LogWarning($"User [{user.Id}] cannot checkout with empty cart");
                    return GetResponse(400, "Cart is empty"); 
                }

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    User = user,
                    ProductSnapshots = user.CartItems!.Select(x => x.Product.TakeSnapshot(x.Quantity)).ToList()
                };

                DbContext.CartItems.RemoveRange(user.CartItems!);
                await DbContext.Orders.AddAsync(order);
                await DbContext.SaveChangesAsync();

                Logger.LogInformation($"User [{user.Id}] successfully created order [{order.Id}]");
                return GetResponse(201, "Ok", order.Id.ToString());
            }
            catch (Exception ex)
            {
                Logger.LogError($"Cannot check user {user.Id} out", ex.Message, ex.StackTrace);
                return GetResponse(500, "Unknown error");
            }
        }
    }
}

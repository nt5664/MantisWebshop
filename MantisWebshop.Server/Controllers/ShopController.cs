using MantisWebshop.Server.Extensions;
using MantisWebshop.Server.Models;
using MantisWebshop.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using MongoDB.Driver;
using Const = MantisWebshop.Server.Constants;

namespace MantisWebshop.Server.Controllers
{
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly ILogger<ShopController> _logger;

        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Order> _orders;

        public ShopController(ILogger<ShopController> logger)
        {
            _products = MongoDbService.Instance.Db.GetCollection<Product>(Const.PRODUCT_COLLECTION);
            _users = MongoDbService.Instance.Db.GetCollection<User>(Const.USER_COLLECTION);
            _orders = MongoDbService.Instance.Db.GetCollection<Order>(Const.ORDER_COLLECTION);
            _logger = logger;
        }

        [HttpGet]
        [Route("[controller]")]
        public List<Product> GetProducts([FromQuery(Name = "page")] int page = 1)
        {
            try
            {
                return _products.Find(x => true)
                    .Skip(PageOffset(page))
                    .Limit(Const.PRODUCTS_PER_PAGE)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Trace: {ex.StackTrace}");
                Response.StatusCode = 500;
                return null!;
            }
        }

        [HttpGet]
        [Route("[controller]/{id}")]
        public Product GetProduct(string id) 
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                Response.StatusCode = 400;
                return null!; 
            }

            Product? prod = null;
            try
            {
                prod = _products.Find(x => x.Id.ToString().Equals(id)).First();
                _logger.LogInformation($"Product found by id {id}");
            }
            catch (InvalidOperationException)
            {
                _logger.LogWarning($"Product id [{id}] does not exist");
                Response.StatusCode = 404;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"id: {id}", $"StackTrace: {ex.StackTrace}");
                Response.StatusCode = 400;
            }

            return prod!;
        }

        [HttpGet]
        [Route("[controller]/cart")]
        [Authorize]
        public List<CartItemDto> GetCart()
        {
            if (!TryGetUser(out var user))
                return null!;

            try
            {
                var items = user.Cart?.Items;
                if (items is null || items.Count == 0)
                    return new List<CartItemDto>();

                return _products.Find(x => items.Any(y => y.ProductId.Equals(x.Id)))
                    .ToEnumerable()
                    .Select(x =>
                    {
                        return new CartItemDto
                        {
                            Product = x,
                            Quantity = items.Single(y => y.ProductId.Equals(x.Id)).Quantity
                        };
                    })
                    .ToList();
            }
            catch (InvalidOperationException)
            {
                return new List<CartItemDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Trace: {ex.StackTrace}");
                Response.StatusCode = 500;
                return null!;
            }
        }

        [HttpPost]
        [Route("[controller]/cart")]
        [Authorize]
        public void PostAddToCart(CartData cartData)
        {
            if (string.IsNullOrWhiteSpace(cartData.ProductId))
            {
                Response.StatusCode = 400;
                return;
            }

            if (!TryGetUser(out var user))
                return;

            try
            {
                if (user.Cart is null)
                    user.Cart = new User.UserCart();

                var cartItem = user.Cart.Items.SingleOrDefault(x => x.ProductId.ToString().Equals(cartData.ProductId));
                if (cartItem is null)
                {
                    if (cartData.Amount <= 0)
                    {
                        _logger.LogWarning($"Cannot remove product [{cartData.ProductId}] from cart since it does not present in it.");
                        Response.StatusCode = 422;
                        return;
                    }

                    cartItem = new CartItem
                    {
                        Id = ObjectId.GenerateNewId(),
                        ProductId = ObjectId.Parse(cartData.ProductId),
                        Quantity = cartData.Amount
                    };

                    user.Cart.Items.Add(cartItem);
                }
                else
                {
                    cartItem.Quantity += cartData.Amount;
                    if (cartItem.Quantity <= 0)
                    {
                        user.Cart.Items.Remove(cartItem);
                    }
                }

                _users.ReplaceOne(x => x.Id.Equals(user.Id), user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"CartData: {cartData}", $"Trace: {ex.StackTrace}");
                Response.StatusCode = 500;
            }
        }

        [HttpDelete]
        [Route("[controller]/cart")]
        [Authorize]
        public void DeleteCart()
        {
            if (!TryGetUser(out User user))
                return;

            try
            {
                if (user.Cart is null || user.Cart.Items.Count == 0)
                    return;

                user.Cart.Items.Clear();
                _users.ReplaceOne(x => x.Id.Equals(user.Id), user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Trace: {ex.StackTrace}");
                Response.StatusCode = 500;
            }
        }

        [HttpPost]
        [Route("[controller]/checkout")]
        [Authorize]
        public void PostCheckout()
        {
            if (!TryGetUser(out var user))
                return;

            try
            {
                if (user.Cart is null || user.Cart.Items.Count == 0)
                {
                    Response.StatusCode = 400;
                    return; 
                }

                var order = new Order
                {
                    Id = ObjectId.GenerateNewId(),
                    User = user,
                    Products = _products.Find(x => user.Cart.Items.Any(y => y.ProductId.Equals(x.Id)))
                        .ToEnumerable()
                        .ToDictionary(k => k, v => (uint)user.Cart.Items.Single(x => x.ProductId.Equals(v.Id)).Quantity)
                };

                _orders.InsertOne(order);
                user.Cart.Items.Clear();
                _users.ReplaceOne(x => x.Id.Equals(user.Id), user);
                Response.StatusCode = 201;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Trace: {ex.StackTrace}");
                Response.StatusCode = 500;
            }
        }

        [HttpGet]
        [Route("[controller]/orders")]
        [Authorize]
        public List<Order> GetOrders([FromQuery(Name = "page")] int page = 1)
        {
            if (!TryGetUser(out var user))
                return null!;

            try
            {
                return _orders.Find(x => user.Id.Equals(x.User.Id))
                    .Skip(PageOffset(page))
                    .Limit(Const.PRODUCTS_PER_PAGE)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Trace: {ex.StackTrace}");
                Response.StatusCode = 500;
                return null!;
            }
        }

        private bool TryGetUser(out User user)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("Invalid token, user ID cannot be obtained");
                Response.StatusCode = 401;
                user = null!;
                return false;
            }

            try
            {
                user = _users.Find(x => x.Id.ToString().Equals(userId)).First();
                return true;
            }
            catch (InvalidOperationException)
            {
                _logger.LogWarning($"Could not find user by ID {userId}");
                Response.StatusCode = 401;
                user = null!;
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"ID: {userId}", $"Trace: {ex.StackTrace}");
                Response.StatusCode = 500;
                user = null!;
                return false;
            }
        }

        private static int PageOffset(int page)
        {
            return (page <= 1 ? 0 : page - 1) * Const.PRODUCTS_PER_PAGE;
        }
    }
}

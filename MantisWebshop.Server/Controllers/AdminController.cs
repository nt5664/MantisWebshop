using MantisWebshop.Server.Extensions;
using MantisWebshop.Server.Models;
using MantisWebshop.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Const = MantisWebshop.Server.Constants;

namespace MantisWebshop.Server.Controllers
{
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;

        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<User> _users;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
            _products = MongoDbService.Instance.Db.GetCollection<Product>(Const.PRODUCT_COLLECTION);
            _users = MongoDbService.Instance.Db.GetCollection<User>(Const.USER_COLLECTION);
        }

        [HttpGet]
        [Route("[controller]/products")]
        [Authorize]
        public List<Product> GetProducts([FromQuery(Name = "page")] int page = 1) 
        {
            if (!TryGetUser(out var user))
                return null!;

            try
            {
                return _products.Find(x => x.UserId.Equals(user.Id))
                    .Skip(PageOffset())
                    .Limit(Const.PRODUCTS_PER_PAGE)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Trace: {ex.StackTrace}");
                Response.StatusCode = 500;
                return null!;
            }

            int PageOffset()
            {
                return (page <= 1 ? 0 : page - 1) * Const.PRODUCTS_PER_PAGE;
            }
        }

        [HttpGet]
        [Route("[controller]/products/{id}")]
        [Authorize]
        public Product GetProduct(string id)
        {
            if (!TryGetUser(out var user))
                return null!;

            Product? prod = null;
            try
            {
                prod = _products.Find(x => x.Id.ToString().Equals(id)).Single();
                if (!prod.Id.Equals(user.Id))
                {
                    _logger.LogWarning($"User [{user.Id}] is not authorized to view product [{prod.Id}]");
                    Response.StatusCode = 401;
                }

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
                Response.StatusCode = 500;
            }

            return prod!;
        }

        [HttpPost]
        [Route("[controller]/products/edit")]
        [Authorize]
        public Product PostEditProduct(Product product)
        {
            if (!TryGetUser(out var user))
                return null!;

            if (product is null)
            {
                _logger.LogWarning($"Invalid product: {product}");
                Response.StatusCode = 422;
                return null!;
            }

            try
            {
                if (product.IsNew)
                { 
                    product.UserId = user.Id;
                    _products.InsertOne(product);
                    Response.StatusCode = 201;
                }
                else
                {
                    var storedProduct = _products.Find(x => x.Id.Equals(product.Id)).Single();
                    product.UserId = storedProduct.UserId;
                    product.Version = storedProduct.Version;
                    if (!product.UserId.Equals(user.Id))
                    {
                        _logger.LogWarning($"User [{user.Id}] is not authorized to edit product [{product.Id}]");
                        Response.StatusCode = 401;
                        return null!;
                    }

                    ++product.Version;
                    _products.ReplaceOne(x => x.Id.Equals(product.Id), product);
                    Response.StatusCode = 200;
                }

                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Product: {product}", $"StackTrace: {ex.StackTrace}");
                Response.StatusCode = 422;
                return null!;
            }
        }

        [HttpDelete]
        [Route("[controller]/products/{id}/delete")]
        [Authorize]
        public string DeleteProduct(string id)
        {
            if (!TryGetUser(out var user))
                return null!;
            
            try
            {
                var result = _products.DeleteOne(x => x.UserId.Equals(user.Id) && x.Id.ToString().Equals(id));
                if (result.DeletedCount > 0)
                { 
                    _logger.LogInformation($"Product with Id [{id}] deleted");
                    Response.StatusCode = 200;
                    return id;
                }
                else
                {
                    _logger.LogWarning($"Product does not exists with ID [{id}]");
                    Response.StatusCode = 404;
                    return null!;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Id: {id}", $"Trace: {ex.StackTrace}");
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
    }
}

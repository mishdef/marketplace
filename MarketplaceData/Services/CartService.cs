using System;
using System.Linq;
using System.Threading.Tasks;
using MarketplaceData.Interfaces;
using MarketplaceData.Model.Cart;
using Microsoft.EntityFrameworkCore;
using VetClassLibrary.Services;

namespace MarketplaceData.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetUserCartAsync(int userId)
        {
            var clientInfo = await _context.Clients
                .Include(c => c.Cart)
                .ThenInclude(cart => cart!.CompanyCarts)
                .ThenInclude(cc => cc.Company)
                .Include(c => c.Cart)
                .ThenInclude(cart => cart!.CompanyCarts)
                .ThenInclude(cc => cc.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (clientInfo == null)
            {
                clientInfo = new MarketplaceData.Model.User.ClientInfo
                {
                    UserId = userId,
                    Address = string.Empty,
                    Cart = new Cart()
                };
                _context.Clients.Add(clientInfo);
                await _context.SaveChangesAsync();
            }

            if (clientInfo.Cart == null)
            {
                clientInfo.Cart = new Cart();
                _context.Carts.Add(clientInfo.Cart);
                await _context.SaveChangesAsync();
            }

            return clientInfo.Cart;
        }

        public async Task AddToCartAsync(int userId, int productId, double qty)
        {
            if (qty <= 0) throw new ArgumentException("Quantity must be positive");

            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new Exception("Product not found");

            var storage = await _context.StorageItems.FirstOrDefaultAsync(s => s.ItemId == productId);
            if (storage == null || storage.Qty < qty)
            {
                throw new Exception("Not enough quantity in stock");
            }

            var cart = await GetUserCartAsync(userId);

            var companyCart = cart.CompanyCarts.FirstOrDefault(cc => cc.CompanyId == product.CompanyId);
            if (companyCart == null)
            {
                companyCart = new CompanyCart
                {
                    CartId = cart.Id,
                    CompanyId = product.CompanyId
                };
                _context.CompanyCarts.Add(companyCart);
                cart.CompanyCarts.Add(companyCart);
                await _context.SaveChangesAsync();
            }

            var cartItem = companyCart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem != null)
            {
                if (storage.Qty < cartItem.Quantity + qty)
                {
                    throw new Exception("Not enough quantity in stock to add more");
                }
                cartItem.Quantity += qty;
            }
            else
            {
                cartItem = new CartItem
                {
                    CompanyCartId = companyCart.Id,
                    ProductId = productId,
                    Quantity = qty
                };
                _context.CartItems.Add(cartItem);
                companyCart.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(int userId, int cartItemId, double newQty)
        {
            if (newQty <= 0)
            {
                await RemoveFromCartAsync(userId, cartItemId);
                return;
            }

            var cartItem = await _context.CartItems
                .Include(ci => ci.CompanyCart)
                .ThenInclude(cc => cc.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

            if (cartItem == null) throw new Exception("Cart item not found");

            // verify user owns this cart
            var cart = await GetUserCartAsync(userId);
            if (cart.Id != cartItem.CompanyCart.CartId)
            {
                throw new Exception("Unauthorized");
            }

            var storage = await _context.StorageItems.FirstOrDefaultAsync(s => s.ItemId == cartItem.ProductId);
            if (storage == null || storage.Qty < newQty)
            {
                throw new Exception("Not enough quantity in stock");
            }

            cartItem.Quantity = newQty;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(int userId, int cartItemId)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.CompanyCart)
                .ThenInclude(cc => cc.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

            if (cartItem == null) return;

            var cart = await GetUserCartAsync(userId);
            if (cart.Id != cartItem.CompanyCart.CartId)
            {
                throw new Exception("Unauthorized");
            }

            _context.CartItems.Remove(cartItem);
            
            // if last item in company cart, remove company cart too
            if (cartItem.CompanyCart.CartItems.Count <= 1)
            {
                _context.CompanyCarts.Remove(cartItem.CompanyCart);
            }

            await _context.SaveChangesAsync();
        }
    }
}

using VetClassLibrary.Interfaces;
using VetClassLibrary.Model;
using VetClassLibrary.Model.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using MarketplaceData.Model.Cart;
using Domain;

namespace VetClassLibrary.Services
{
    public class OrderService : Repository<Order>, IOrderService
    {
        private readonly AppDbContext _db;
        private readonly IEnumerable<IOrderProcessor> _orderProcessors;

        public OrderService(AppDbContext db, IEnumerable<IOrderProcessor> orderProcessors) : base(db)
        {
            _db = db;
            _orderProcessors = orderProcessors;
        }

        public async Task ProcessCheckoutAsync(Order order, int userId)
        {
            if (!order.IsPaid || !order.IsPerformed)
            {
                order.ClientId = userId;
                order.Date = DateTime.Now;
                order.IsPerformed = true;

                await _db.Orders.AddAsync(order);
                await _db.SaveChangesAsync();

                await ProcessItemsAsync(order.CartItems);
            }
            else
            {
                throw new Exception("Order has already been processed");
            }
        }

        public async Task ProcessItemsAsync(IEnumerable<CartItem> items)
        {
            foreach (var item in items)
            {
                if (item.Product != null)
                {
                    var processor = _orderProcessors.FirstOrDefault(p => p.CanProcess(item.Product));

                    if (processor != null)
                    {
                        await processor.ProcessAsync(item.Product, item.Quantity);
                    }
                }
            }
        }

        public async Task<List<Order>> GetOrderHistoryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _db.Orders
                .Include(o => o.Client)
                .Include(o => o.CartItems)
                    .ThenInclude(i => i.Product)
                .Include(o => o.CartItems)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(o => o.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(o => o.Date <= endDate.Value);
            }

            return await query.OrderByDescending(o => o.Date).ToListAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Repositories
{
    public sealed class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> ListOrdersByOrganisationIdAsync(Guid organisationId)
        {
            return await _context.Order
                .Include(x => x.OrderStatus)
                .Include(x => x.OrganisationAddress)
                .Include(x => x.OrganisationContact)
                .Include(x => x.SupplierAddress)
                .Include(x => x.SupplierContact)
                .Where(o => o.OrganisationId == organisationId)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Order.FindAsync(orderId);
            if (order != null)
            {
                await _context.Entry(order).Reference(x => x.OrderStatus).LoadAsync();
                await _context.Entry(order).Reference(x => x.OrganisationAddress).LoadAsync();
                await _context.Entry(order).Reference(x => x.OrganisationContact).LoadAsync();
                await _context.Entry(order).Reference(x => x.SupplierAddress).LoadAsync();
                await _context.Entry(order).Reference(x => x.SupplierContact).LoadAsync();
            }

            return order;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            if (order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            _context.Order.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CreateOrderAsync(Order order)
        {
            if (order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            using (var dbContextTransaction = await _context.Database.BeginTransactionAsync())
            {
                order.OrderStatus = await _context.OrderStatus.FindAsync(order.OrderStatus.OrderStatusId) ?? order.OrderStatus;

                _context.Order.Add(order);
                await _context.SaveChangesAsync();
                await dbContextTransaction.CommitAsync();
            }

            return order.OrderId;
        }
    }
}

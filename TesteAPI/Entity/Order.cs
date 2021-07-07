using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TesteAPI.Data;
using TesteAPI.Enums;

namespace TesteAPI.Entity
{
    public class Order
    {
        private readonly DataContext _context;
        public Order(DataContext context)
        {
            _context = context;
        }

        [Key]
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public EOrderStatus Status { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public decimal Value { get; set; }
        public DateTime CreateDate { get; set; }

        public Order(string customerId)
        {
            Id = GeneratetId();
            CustomerId = customerId;
            Status = EOrderStatus.Created;
            CreateDate = DateTime.Now;
        }
        public Order()
        {
        }
        public void AddItem(OrderItem orderItem)
        {
            Items.Add(orderItem);
            _context.SaveChanges();
        }

        public string GeneratetId()
        {
            var id = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8).ToUpper();
            return id;
        }

        public void TotalValue(List<OrderItem> orderItems)
        {
            Value = 0;
            foreach (var item in orderItems)
            {
                Value += item.Price * item.Quantity;
            }
        }
                
        public async Task<ActionResult<List<Order>>> Get()
        {
            var listOrders = await _context.Orders.Include(x => x.Items).AsNoTracking().ToListAsync();
            return listOrders;
        }

        public async Task<ActionResult<Order>> GetById(string id)
        {
            var order = await _context.Orders.Include(x => x.Items).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return order;
        }

        public async Task<ActionResult<List<Order>>> GetByCustomer(string customerId)
        {
            var orders = await _context.Orders.Include(x => x.Items).AsNoTracking().
                Where(x => x.CustomerId == customerId).ToListAsync();
            return orders;
        }

        public void Pay(Order order)
        {
            order.Status = EOrderStatus.Paid;
        }

    }
}

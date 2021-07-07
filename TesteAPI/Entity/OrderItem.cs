using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TesteAPI.Data;

namespace TesteAPI.Entity
{
    public class OrderItem
    {
        private readonly DataContext _context;

        public OrderItem(DataContext context)
        {
            _context = context;
        }
        [Key]
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string CustomerId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }

        public OrderItem( string customerId,string productId, decimal quantity)
        {


            Id = GeneratetId();
            CustomerId = customerId;
            ProductId = productId;
            Quantity = quantity;


        }

        public OrderItem()
        {
        }

        public string GeneratetId()
        {
            var id = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8).ToUpper();
            return id;
        }


    }
}

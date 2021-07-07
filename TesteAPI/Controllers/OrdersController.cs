using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesteAPI.Data;
using TesteAPI.Entity;
using TesteAPI.Enums;

namespace TesteAPI.Controllers
{
    [Route("orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly Order _order;

        public OrdersController(DataContext context, Order order)
        {
            _context = context;
            _order = order;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Order>>> Get()
        {
            var orders = await _order.Get();

            return orders;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Order>> GetById(string id)
        {
            var order = await _order.GetById(id);
            return order;
        }

        [HttpGet("customers/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Order>>> GetByCustomer(string customerId)
        {
            var order = await _order.GetByCustomer(customerId);
            return order;
        }

        [HttpPost]
        [Authorize(Roles = "customer")]
        public async Task<ActionResult<Order>> Create(OrderItem model)
        {
            if (ModelState.IsValid)
            {
                var product = _context.Products.AsNoTracking().FirstOrDefault(x => x.Id == model.ProductId);

                var orderItem = new OrderItem(model.CustomerId, model.ProductId, model.Quantity);

                orderItem.Price = product.Price;

                if (product.AvailableQuantity < orderItem.Quantity)
                    return Ok("Quantidade desejada é maior que a quantidade disponível. " +
                        "Unidades disponíveis = " + product.AvailableQuantity);

                var order = new Order(model.CustomerId);

                order.Items.Add(orderItem);

                order.TotalValue(order.Items);

                product.AvailableQuantity -= orderItem.Quantity;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return order;
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "customer")]
        public async Task<ActionResult<Order>> AddOrderItem(string id, OrderItem model)
        {
            var order = _context.Orders.Include(x => x.Items).FirstOrDefault(x => x.Id == id);

            if (order == null)
                return Ok("Pedido informado não existe");

            if (order.Status != EOrderStatus.Created)
                return Ok("Este Pedido não está com o status Criado, não é possível alteralo. " +
                    "Status do pedido = " + order.Status);

            var product = _context.Products.AsNoTracking().FirstOrDefault(x => x.Id == model.ProductId);

            var orderItem = new OrderItem(model.CustomerId, model.ProductId, model.Quantity);

            orderItem.Price = product.Price;

            order.AddItem(orderItem);

            if (product.AvailableQuantity < orderItem.Quantity)
                return Ok("Quantidade desejada é maior que a quantidade disponível. " +
                    "Unidades disponíveis = " + product.AvailableQuantity);

            order.TotalValue(order.Items);

            _context.Entry(order).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return order;
        }

        [HttpPut("remove/{id}")]
        [Authorize(Roles = "customer")]
        public async Task<ActionResult<Order>> RemoveOrderItem(string id, OrderItem model)
        {
            var order = _context.Orders.Include(x => x.Items).FirstOrDefault(x => x.Id == id);

            if (order == null)
                return Ok("Order informado não existe");

            if (order.Status != EOrderStatus.Created)
                return Ok("Este Pedido não está com o status Criado, não é possível alteralo. " +
                    "Status do pedido = " + order.Status);

            var orderItem = order.Items.FirstOrDefault(x => x.Id == model.Id);
            order.Items.Remove(orderItem);

            order.TotalValue(order.Items);

            _context.Entry(order).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return order;
        }

        [HttpPut("editItem/{id}")]
        [Authorize(Roles = "customer")]
        public async Task<ActionResult<Order>> EditItemQuantity(string id, OrderItem model)
        {
            var order = _context.Orders.Include(x => x.Items).FirstOrDefault(x => x.Id == id);

            if (order == null)
                return Ok("Order informado não existe");

            if (order.Status != EOrderStatus.Created)
                return Ok("Este Pedido não está com o status Criado, não é possível alteralo. " +
                    "Status do pedido = " + order.Status);

            var orderItem = order.Items.FirstOrDefault(x => x.Id == model.Id);

            var quantity = model.Quantity;

            orderItem.Quantity += quantity;

            order.TotalValue(order.Items);

            _context.Entry(order).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return order;
        }

        [HttpPut("pay/{id}")]
        [Authorize(Roles = "customer")]
        public async Task<ActionResult<Order>> Pay(string id)
        {
            var order = _context.Orders.FirstOrDefault(x => x.Id == id);

            if (order == null)
                return Ok("Order não encontrado");

            if (order.Status != EOrderStatus.Created)
                return Ok("Este Pedido não está com o status Criado, não é possível alteralo. " +
                    "Status do pedido = " + order.Status);

            _order.Pay(order);

            _context.Entry(order).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return order;

        }

    }
}

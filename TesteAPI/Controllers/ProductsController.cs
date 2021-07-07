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

namespace TesteAPI.Controllers
{
    [Route("products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly Product _product;
        public ProductsController(DataContext context, Product produdct)
        {
            _context = context;
            _product = produdct;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get()
        {
            var products = await _context.Products.AsNoTracking().ToArrayAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById(string id)
        {
            var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync();

            return Ok(product);
        }

        [HttpGet("categories/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory(string id)
        {
            var category = await _context.Categories.Include(x => x.Products).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            var products = category.Products.ToList();

            return Ok(products);
        }

        /*[HttpPost]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Create([FromBody] Product model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = _context.Categories.AsNoTracking().FirstOrDefault(x => x.Id==model.CategoryId);

            if (category == null)
                return Ok("Categoria não cadastrada");

            model.Category = category;

        _product.GenerateId(model);
            var product = new Product(model.Id, model.Title,model.Description, model.Price,model.CategoryId,category);
                       
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;

        }*/

        [HttpPost]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Create([FromBody] Product model)
        {
            var category = _context.Categories.AsNoTracking().FirstOrDefault(x => x.Id == model.CategoryId);

            if (category == null)
                return Ok("Categoria informada não existe");

            _product.GenerateId(model);
            var product = new Product(model.Id, model.Title, model.Description, model.Price, model.CategoryId);
            category.Products.Add(product);


            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return product;
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Edit(string id, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != product.Id)
                return NotFound(new { message = "Produto não encontrado" });

            var category = _context.Categories.AsNoTracking().FirstOrDefault(x => x.Id == product.CategoryId);

            if (category == null)
                return Ok("Categoria informada não existe");

            try
            {
                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return product;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível editar o produto" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Delete(string id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return NotFound(new { message = "Produto não encontrado" });
            }

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Produto removido com sucesso" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível remover o produto" });
            }
        }

        [HttpPut("receiveproduct/{id}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> ReceiveProduct(ProductReceiving receiving)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == receiving.ProductId);

            if (receiving.Quantity <= 0)
                return Ok("não é possível fazer o recebimento de 0 ou menos unidades");

            product.AvailableQuantity += receiving.Quantity;
            await _context.SaveChangesAsync();

            return product;
        }

    }
}

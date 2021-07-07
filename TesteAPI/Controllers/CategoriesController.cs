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
    [Route("categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly Category _category;

        public CategoriesController(DataContext context, Category category)
        {
            _context = context;
            _category = category;
        }

        [HttpGet]
        public List<Category> Get()
        {
            var categories = _category.ListCategories();

            return categories;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetById(string id)
        {
            var category = await _category.GetByIdAsync(id);

            return category;
        }

        [HttpPost]
        public async Task<ActionResult<List<Category>>> Create([FromBody] Category model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _category.GenerateId(model);

            if (_context.Categories.Any(x => x.Title == model.Title))
                return Ok("Este nome de categoria já existe");

            try
            {
                await _context.Categories.AddAsync(model);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível criar a categoria" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<Category>>> Edit(string id, [FromBody] Category model)
        {
            if (model.Id != id)
                return NotFound(new { message = "Categoria não encontrada" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_context.Categories.Any(x => x.Title == model.Title))
                return Ok("Este nome de categoria já existe");

            try
            {
                _context.Entry<Category>(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Este registro já foi atualizado" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível atualizar esta categoria" });
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> Delete(string id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
                return NotFound(new { message = "Categoria não encontrada" });

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Categoria removida com sucesso" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível remover esta categoria" });
            }
        }
    }
}

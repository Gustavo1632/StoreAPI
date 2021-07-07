using FluentValidator;
using FluentValidator.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TesteAPI.Data;

namespace TesteAPI.Entity
{
    public class Category : Notifiable
    {
        private readonly DataContext _context;
        public Category(DataContext context)
        {
            _context = context;
        }

        [Key]
        public string Id { get; set; }

        [Required(ErrorMessage = "Título é obrigatório.")]
        [MaxLength(60, ErrorMessage = "Este campo deve conter enre 3 e 60 caracteres.")]
        [MinLength(3, ErrorMessage = "Este campo deve conter enre 3 e 60 caracteres.")]
        public string Title { get; set; }
        public virtual List<Product> Products { get; set; } = new List<Product>();

        public Category(string id, string title)
        {
            Id = id;
            Title = title;

            AddNotifications(new ValidationContract().IsFalse(CategoryExists(title), "Category", "Categoria já cadastrada"));
        }

        public Category()
        {
        }

        public void GenerateId(Category category)
        {
            category.Id = Guid.NewGuid().ToString().Replace("-", "").ToUpper().Substring(0, 8);
        }

        public List<Category> ListCategories()
        {
            var listCategories = _context.Categories.AsNoTracking().ToList();

            return listCategories;

        }

        public async Task<ActionResult<Category>> GetByIdAsync(string id)
        {
            var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            return category;

        }

        public bool CategoryExists(string categoryTitle)
        {
            var listCategories = ListCategories();

            if (listCategories.Any(x => x.Title == categoryTitle))
                return true;

            return false;
        }

    }
}

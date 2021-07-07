using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TesteAPI.Data;

namespace TesteAPI.Entity
{
    public class Product
    {

        private readonly DataContext _context;

        public Product(DataContext context)
        {
            _context = context;
        }
        [Key]
        public string Id { get; set; }

        [Required(ErrorMessage = "Título é obrigatório.")]
        [MaxLength(60, ErrorMessage = "Este campo deve conter enre 3 e 60 caracteres.")]
        [MinLength(3, ErrorMessage = "Este campo deve conter enre 3 e 60 caracteres.")]
        public string Title { get; set; }

        [MaxLength(1024, ErrorMessage = "Este campo deve ter no máximo 1024 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
        public decimal Price { get; set; }
        public decimal AvailableQuantity { get; set; }
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string CategoryId { get; set; }

        public Product(string id, string title, string description, decimal price, string categoryId)
        {
            Id = id;
            Title = title;
            Description = description;
            Price = price;

            CategoryId = categoryId;
            AvailableQuantity = 0;
        }
        public Product()
        {
        }

        public void GenerateId(Product product)
        {
            product.Id = Guid.NewGuid().ToString().Replace("-", "").ToUpper().Substring(0, 8);
        }
    }
}

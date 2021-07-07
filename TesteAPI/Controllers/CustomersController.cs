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
using TesteAPI.Services;
using TesteAPI.ValueObjects;

namespace TesteAPI.Controllers
{
    [Route("customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly Person _person;
        public CustomersController(DataContext context, Person person)
        {
            _context = context;
            _person = person;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Customer>> Create(Customer person)
        {
            if (ModelState.IsValid)
            {
                _person.GenerateId(person);
                var document = new Document(person.Document.Number, person.Id);

                var email = new Email(person.Email.Address, person.Id);

                var address = new Address(person.Address.Street, person.Address.Number, person.Address.District,
                person.Address.City, person.Address.State, person.Address.Country, person.Address.ZipCode, person.Id);

                var customer = new Customer(person.Id, document, email, address, person.FirstName, person.LastName,
                    person.Phone, person.BirthDate, person.Username, person.Password);

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return customer;
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Login(Customer model)
        {
            var customer = await _context.Customers.AsNoTracking().
                Where(x => x.Username == model.Username && x.Password == model.Password).
                FirstOrDefaultAsync();

            if (customer == null)
                return NotFound(new { message = "Usuário ou senha inválidos" });

            var token = TokenService.GenerateToken(customer);

            //Esconde a senha na hora de retornar pra tela
            model.Password = "";

            return new
            {
                user = customer,
                token = token
            };
        }
    }
}

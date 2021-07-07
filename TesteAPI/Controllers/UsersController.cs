using Microsoft.AspNetCore.Authorization;
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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TesteAPI.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly Email _email;
        private readonly Document _document;
        private readonly User _user;
        private readonly Person _person;
        private readonly Address _address;

        public UsersController(DataContext context, Email email, Document document, User user, Person person, Address address)
        {
            _context = context;
            _email = email;
            _document = document;
            _user = user;
            _person = person;
            _address = address;
        }


        // GET: api/<UsersController>
        [HttpGet]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            var users = await _user.ListUsersAsync();

            return users;
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<User>> Get(string id)
        {
            var user = await _user.GetByIdAsync(id);

            return user;
        }


        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Create([FromBody] User person)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _person.GenerateId(person);

            var listNotification = _person.VerifyPersonCreation(person);

            var document = new Document(person.Document.Number, person.Id);

            var email = new Email(person.Email.Address, person.Id);

            var address = new Address(person.Address.Street, person.Address.Number, person.Address.District,
                person.Address.City, person.Address.State, person.Address.Country, person.Address.ZipCode, person.Id);

            var user = new User(person.Id, document, email, address, person.FirstName, person.LastName,
                person.Phone, person.BirthDate, person.Username, person.Password);

            foreach (var x in user.Notifications)
            {
                listNotification.Add(x);
            }

            if (listNotification.Any())
                return Ok(listNotification);

            try
            {
                await _context.Users.AddAsync(user);
                await _context.Documents.AddAsync(document);
                await _context.Emails.AddAsync(email);
                await _context.Addresses.AddAsync(address);
                await _context.SaveChangesAsync();

                user.Password = "";

                return user;

            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });
            }

        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Login([FromBody] User model)
        {
            var user = await _context.Users.AsNoTracking().
                Where(x => x.Username == model.Username && x.Password == model.Password).
                FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválidos" });

            if (user.Active == false)
                return Ok("Usuário está inativo, favor ativar o mesmo para que seja possível efetuar o login");

            var token = TokenService.GenerateToken(user);

            //Esconde a senha na hora de retornar pra tela
            model.Password = "";

            return new
            {
                user = user,
                token = token
            };
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<User>> Edit(string id, [FromBody] User model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != model.Id)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            model.Active = true;

            var notifications = _user.IsActive(id);

            var user = await _user.GetByIdAsync(id);

            if (notifications.Any())
                return Ok(notifications);

            try
            {
                _document.EditDocument(user, model);
                _email.EditEmail(user, model);
                _address.EditAddress(user, model);
                _context.Entry(model).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return model;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível editar o usuário" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Delete(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return NotFound(new { message = "Usuário não encontrado" });

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Usuário removido com sucesso" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível remover este usuário" });
            }

        }

        [HttpPut("disable/{id}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<User>> Disable(string id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);

            _user.Disable(user);
            await _context.SaveChangesAsync();
            return user;

        }

        [HttpPut("activate/{id}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<User>> Activate(string id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);

            _user.Activate(user);
            await _context.SaveChangesAsync();
            return user;

        }

    }
}

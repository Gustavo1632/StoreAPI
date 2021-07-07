using FluentValidator;
using FluentValidator.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesteAPI.Data;
using TesteAPI.ValueObjects;

namespace TesteAPI.Entity
{
    public class User : Person
    {
        private readonly DataContext _context;

        public User (DataContext context)
        {
            _context = context;
        }
                        

        public User(string id, Document document, Email email, Address address, string firstName, string lastName,
        string phone, DateTime birthDate, string userName, string password) :
        base(id, document, email, address, firstName, lastName, phone, birthDate, userName, password)
        {
            Role = "employee";

            AddNotifications(new ValidationContract().Requires().IsEmail(Email.Address, "Email", "Email inválido"));
            AddNotifications(new ValidationContract().IsTrue(Document.IsCpf(Document.Number), "Document", "CPF Inválido"));
        }

        public User()
        {
        }

        public async Task<List<User>> ListUsersAsync()
        {
            var listUser = await _context.Users.Include(x => x.Address).Include(x => x.Document).
                Include(x => x.Email).AsNoTracking().ToListAsync();
           
            return listUser;
        }

        public async Task<User> GetByIdAsync(string id)
        {
            var user = await _context.Users.Include(x => x.Document).Include(x => x.Email).
                Include(x => x.Address).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
               
            return user;
        }

        public List<Notification> IsActive(string id)
        {
            var notifications = new List<Notification>();

            /*if (_context.Users.Any(x => x.Username == model.Username && x.Id != model.Id))
            {
                notifications.Add(new Notification("UserName", "Userame já Cadastrado"));
            }*/

            var user = _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == id);

            if (user.Active == false)
            {
                notifications.Add(new Notification("Active", "Usuário inativo"));
            }

            return notifications;
        }

        public void Disable(User user)
        {
            user.Active = false;
        }

        public void Activate(User user)
        {
            user.Active = true;
        }
    }
}


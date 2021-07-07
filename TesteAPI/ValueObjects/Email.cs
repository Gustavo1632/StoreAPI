using FluentValidator;
using FluentValidator.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TesteAPI.Data;
using TesteAPI.Entity;

namespace TesteAPI.ValueObjects
{
    public class Email : Notifiable
    {
        private readonly DataContext _context;

        public Email(DataContext context)
        {
            _context = context;
        }

        [Key]
        public string Id { get; set; }
        public string Address { get; set; }
        public string PersonId { get; set; }

        public Email(string address, string personId)
        {
            Id = Guid.NewGuid().ToString().Replace("-", "").ToUpper().Substring(0, 8);
            Address = address;
            PersonId = personId;

            AddNotifications(new ValidationContract().Requires().IsEmail(address, "Email", "Email inválido"));
        }

        public Email()
        {
        }

        public List<Email> ListEmail()
        {
            var listEmails = _context.Emails.AsNoTracking().ToList();

            return listEmails;

        }

        public void EditEmail(Person person, Person model)
        {
            //var personEmail = _context.Emails.FirstOrDefault(x => x.PartnerId == person.Id);

            var email = new Email(model.Email.Address, model.Id);
            _context.Emails.AddAsync(email);
            model.Email = email;


        }

    }
}

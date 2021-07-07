using FluentValidator;
using FluentValidator.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TesteAPI.Data;
using TesteAPI.ValueObjects;

namespace TesteAPI.Entity
{
    public class Person : Notifiable
    {
        private readonly DataContext _context;
        private readonly Email _email;

        public Person(DataContext context, Email email)
        {
            _context = context;
            _email = email;
        }

        [Key]
        public string Id { get; set; }

        [Required(ErrorMessage = "Documento é obrigatório.")]
        public Document Document { get; set; }

        [Required(ErrorMessage = "Email é obrigatório.")]
        public Email Email { get; set; }

        [Required(ErrorMessage = "Endereço é obrigatório.")]
        public Address Address { get; set; }

        [Required(ErrorMessage = "Primeiro nome é obrigatório.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Sobrenome é obrigatório.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Telefone é obrigatório.")]
        public string Phone { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "Data de nascimento é obrigatório.")]
        public DateTime BirthDate { get; set; }

        [MaxLength(20, ErrorMessage = "Este campo deve conter enre 3 e 20 caracteres.")]
        [MinLength(3, ErrorMessage = "Este campo deve conter enre 3 e 20 caracteres.")]
        public string Username { get; set; }

        [MaxLength(20, ErrorMessage = "Este campo deve conter enre 3 e 20 caracteres.")]
        [MinLength(3, ErrorMessage = "Este campo deve conter enre 3 e 20 caracteres.")]
        public string Password { get; set; }
        public bool Active { get; set; }
        public string Role { get; set; }

        public Person(string id, Document document, Email email, Address address, string firstName, string lastName,
        string phone, DateTime birthDate, string userName, string password)
        {
            Id = id;
            Document = document;
            Email = email;
            Address = address;
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            BirthDate = birthDate;
            Username = userName;
            Password = password;
            Active = true;

            AddNotifications(new ValidationContract().Requires().IsEmail(Email.Address, "Email", "Email inválido"));
            AddNotifications(new ValidationContract().IsTrue(Document.IsCpf(Document.Number), "Document", "CPF inválido"));
        }

        public Person()
        {
        }

        public void GenerateId(Person person)
        {
            person.Id = Guid.NewGuid().ToString().Replace("-", "").ToUpper().Substring(0, 8);
        }

        public List<Notification> VerifyPersonCreation(Person model)
        {
            var listNotification = new List<Notification>();

            var listDocuments = _context.Documents.AsNoTracking().ToList();
            //_document.ListDocument();


            if (listDocuments.Any(x => x.Number == model.Document.Number))
                listNotification.Add(new Notification("Document", "Documento já cadastrado"));

            var listEmail = _email.ListEmail();
            //await _context.Emails.AsNoTracking().ToListAsync();

            if (listEmail.Any(x => x.Address == model.Email.Address))
                listNotification.Add(new Notification("Email", "Email já cadastrado"));

            if (_context.Users.Any(x => x.Username == model.Username && x.Id != model.Id))
            {
                listNotification.Add(new Notification("UserName", "Userame já Cadastrado"));
            }

            return listNotification;

        }
    }
}

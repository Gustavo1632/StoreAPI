using FluentValidator.Validation;
using System;
using System.Collections.Generic;
using TesteAPI.ValueObjects;

namespace TesteAPI.Entity
{
    public class Customer : Person
    {
        public List<Order> Orders { get; set; } = new List<Order>();
        public decimal Debits { get; set; }

        public Customer(string id, Document document, Email email, Address address, string firstName, 
            string lastName, string phone, DateTime birthDate, string userName, string password) 
            : base(id, document, email, address, firstName, lastName, phone, birthDate, 
                userName, password)
        {
            Debits = 0;
            Orders = new List<Order>();
            Role = "customer";

            AddNotifications(new ValidationContract().Requires().IsEmail(Email.Address, "Email", "Email inválido"));
            AddNotifications(new ValidationContract().IsTrue(Document.IsCpf(Document.Number), "Document", "CPF Inválido"));
        }

        public Customer()
        {
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TesteAPI.Data;
using TesteAPI.Entity;

namespace TesteAPI.ValueObjects
{
    public class Address
    {
        private readonly DataContext _context;

        public Address(DataContext context)
        {
            _context = context;
        }

        [Key]
        public string Id { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string PartnerId { get; set; }

        public Address(string street, string number, string district, string city,
            string state, string country, string zipCode, string partnerId)
        {
            Id = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8).ToUpper();
            Street = street;
            Number = number;
            District = district;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipCode;
            PartnerId = partnerId;
        }

        public Address()
        {
            
        }

        public void EditAddress(Person person, Person model)
        {
            //var personAddress = _context.Addresses.FirstOrDefault(x=>x.PartnerId==person.Id);

            var personAddress = person.Address.Street + person.Address.Number + person.Address.District +
            person.Address.City + person.Address.State + person.Address.Country + person.Address.ZipCode;

            var modelAddress = model.Address.Street + model.Address.Number + model.Address.District +
            model.Address.City + model.Address.State + model.Address.Country + model.Address.ZipCode;

            if (personAddress != modelAddress)
            {
                var address = new Address(model.Address.Street, model.Address.Number, model.Address.District,
            model.Address.City, model.Address.State, model.Address.Country, model.Address.ZipCode, model.Id);
                _context.Addresses.AddAsync(address);
                model.Address = address;
            }
        }

    }
}

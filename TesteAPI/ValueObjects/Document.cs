using FluentValidator;
using FluentValidator.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TesteAPI.Data;
using TesteAPI.Entity;

namespace TesteAPI.ValueObjects
{
    public class Document : Notifiable
    {
        private readonly DataContext _context;

        public Document(DataContext context)
        {
            _context = context;
        }

        [Key]
        public string Id { get; set; }
        public string Number { get; set; }
        public string PersonId { get; set; }

        public Document(string number, string personId)
        {
            Id = Guid.NewGuid().ToString().Replace("-", "").ToUpper().Substring(0, 8);
            Number = number;
            PersonId = personId;

            AddNotifications(new ValidationContract().IsTrue(IsCpf(number), "Document", "CPF Inválido"));
        }

        public Document()
        {
        }

        public static bool IsCpf(string cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;
            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;
            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);
        }

        public void EditDocument(Person person, Person model)
        {
            //var personDocument = _context.Documents.FirstOrDefault(x => x.PartnerId == person.Id);

            if (person.Document.Number != model.Document.Number)
            {
                var document = new Document(model.Document.Number, model.Id);
                _context.Documents.AddAsync(document);
                person.Document = document;
                model.Document = document;
            }
        }

    }
}

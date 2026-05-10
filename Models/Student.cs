using System;
using System.ComponentModel.DataAnnotations;

namespace Registrar.Models
{
    public class Student : DAL.Record
    {
        [Required(ErrorMessage = "Requis")]
        public string Number { get; set; } // Numéro d'admission

        [Required(ErrorMessage = "Requis")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Requis")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Requis")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Requis")]
        [EmailAddress(ErrorMessage = "Invalide")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Requis")]
        [RegularExpression(@"^\(\d{3}\) \d{3}-\d{4}$", ErrorMessage = "Format: (000) 000-0000")]
        public string Phone { get; set; }
    }
}
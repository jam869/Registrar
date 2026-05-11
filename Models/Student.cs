using System;
using System.ComponentModel.DataAnnotations;

namespace Registrar.Models
{
    public class Student : DAL.Record
    {
        [Required(ErrorMessage = "Numéro manquant")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Prénom manquant")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Nom manquant")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Date manquante")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Courriel manquant")]
        [EmailAddress(ErrorMessage = "Invalide")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Téléphone manquant")]
        [RegularExpression(@"^\(\d{3}\) \d{3}-\d{4}$", ErrorMessage = "Format: (000) 000-0000")]
        public string Phone { get; set; }
    }
}
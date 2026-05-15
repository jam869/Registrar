using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Registrar.Models
{
    public class Course : DAL.Record
    {
        [Required(ErrorMessage = "Le code est requis")]
        [RegularExpression(@"^[A-Z0-9]{3}-[A-Z0-9]{3}-[A-Z0-9]{2}$", ErrorMessage = "Format requis: XXX-XXX-XX")]
        public string Code { get; set; } 

        [Required(ErrorMessage = "Le titre est requis")]
        public string Title { get; set; }

        [Required(ErrorMessage = "La session est requise")]
        [Range(1, 6, ErrorMessage = "La session doit être entre 1 et 6")]
        public int Session { get; set; }

        public List<int> Inscriptions { get; set; } = new List<int>();

        public Dictionary<int, int> Allocations { get; set; } = new Dictionary<int, int>();

        public string Name => $"[{Session}] {Code} {Title}";
    }
}
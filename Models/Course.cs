using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Registrar.Models
{
    public class Course : DAL.Record
    {
        [Required(ErrorMessage = "Le code est requis")]
        // Le format correspond aux codes du JSON (ex: 420-KB1-LG)
        [RegularExpression(@"^[A-Z0-9]{3}-[A-Z0-9]{3}-[A-Z0-9]{2}$", ErrorMessage = "Format requis: XXX-XXX-XX")]
        public string Code { get; set; } // Était "Sigle"

        [Required(ErrorMessage = "Le titre est requis")]
        public string Title { get; set; } // Était "Titre"

        [Required(ErrorMessage = "La session est requise")]
        [Range(1, 6, ErrorMessage = "La session doit être entre 1 et 6")]
        public int Session { get; set; }

        public List<int> Inscriptions { get; set; } = new List<int>();
        // Cette propriété génère exactement l'affichage demandé par le prof : "[Session] Sigle Titre"
        public string Name => $"[{Session}] {Code} {Title}";
    }
}
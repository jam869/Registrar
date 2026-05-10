using System;
using System.Collections.Generic;

namespace Registrar.Models
{
    public class Course : DAL.Record
    {
        public string Sigle { get; set; }
        public string Titre { get; set; }
        public int Session { get; set; } // 1 à 6
        // Liste des IDs des étudiants inscrits
        public List<string> Inscriptions { get; set; } = new List<string>();
    }
}
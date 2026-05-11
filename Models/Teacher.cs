using System;
using System.ComponentModel.DataAnnotations;
using DAL; // Requis pour utiliser [ImageAsset] et Record

namespace Registrar.Models
{
    public class Teacher : Record
    {
        [Required(ErrorMessage = "Le code est requis")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Le prénom est requis")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Le nom est requis")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Le téléphone est requis")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "La date est requise")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        // --- LA MAGIE DE L'IMAGE EST ICI ---
        const string Avatars_Folder = @"/App_Assets/teachers/";
        const string Default_Avatar = @"no_avatar.png";

        // L'attribut dit au système "Gère cette chaîne de caractères comme une image"
        [ImageAsset(Avatars_Folder, Default_Avatar)]
        public string Avatar { get; set; } = Avatars_Folder + Default_Avatar;
    }
}
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
namespace A.Models
{
    public class EmpresaRating
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Comente a sua experiencia")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "Max 5000 chars min 10")]
        public string Comentario { get; set; }

        [Required]
        [Display(Name = "Avaliacao de 0-10")]
        [Range(0,10)]
        public float Avaliacao  { get; set; }

        /*
         * Uma avaliação pertence a uma Empresa
         */
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; }

        /*
         * uma avaliação é dada por um User
        */
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }
}





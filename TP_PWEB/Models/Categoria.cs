using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace A.Models
{
	public class Categoria
    {
		public int Id { get; set; }
        [Required]
        [Display(Name = "Nome Categoria")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Max 250 digitos min 3")]
        public string? Nome { get; set; }

		// Cada categoria tem uma coleção de Veiculos
        public  ICollection<Veiculo>? Veiculos { get; set; }

    }
}




using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace A.Models
{
	public class Empresa
	{
		public int Id { get; set; }
		public string? Nome { get; set; }

        public string? Subscricao { get; set; }
        [Display(Name = "Disponivel")]
        public bool Disponivel { get; set; }


        public ICollection<ApplicationUser>? Users { get; set; }

        // A empresa anuncia diversos veiculos
        public ICollection<Veiculo>? Veiculos { get; set; }

        // A empresa tem diversas avaliações

        public ICollection<EmpresaRating>? EmpresaRatings { get; set; }

        // A empresa tem diversas reservas

        public ICollection<Reserva>? Reservas { get; set; }

    }
}


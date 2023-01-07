using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
namespace A.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        [Display(Name = "Preco")]
        public float? Preco { get; set; }

        public string Estado { get; set; }
        [Required]
        [Display(Name = "DataEntrega")]
        public DateTime? DataEntrega { get; set; }
        [Required]
        [Display(Name = "DataLevantamento")]
        public DateTime? DataLevantamento { get; set; }

        // Uma Reserva é de um e só um Veiculo
        public Veiculo Veiculo { get; set; }
        public int? VeiculoId { get; set; }


        // Uma Reserva é feita com uma Empresa
        public Empresa Empresa { get; set; }
        public int? EmpresaId { get; set; }
        
        // Uma Reserva tem uma entrega
        public Entrega Entrega { get; set; }
        public int? EntregaId { get; set; }

        // Uma Reserva tem um levantamento
        public Levantamento Levantamento { get; set; }
        public int? LevantamentoId { get; set; }
        
        // Uma reserva é de um Cliente 
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}


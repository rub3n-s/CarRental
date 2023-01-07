using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace A.Models
{
    public class Entrega
    {
        [ForeignKey("Reserva")]
        public int Id { get; set; }
        public int NumKM { get; set; }
        public bool Danos { get; set; }
        public string? Observacoes { get; set; }
        public string FuncionarioEmail { get; set; }
        
        public Reserva Reserva { get; set; }
        public int ReservaId { get; set; }
    }
}

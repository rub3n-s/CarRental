using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;


namespace A.Models
{
    public class Venda
    {
        public int VendaId { get; set; }

        [Display(Name = "Quantidade", Prompt = "Introduza a Quantidade anual vendida deste curso!")]
        public int QtVendida { get; set; }

        [Display(Name = "Empresa")]
        public int EmpresaId { get; set; }

        [Display(Name = "Empresa")]
        public Empresa Empresa { get; set; }
        
        [Display(Name = "Curso")]
        public int ReservaId { get; set; }
        
        [Display(Name = "Curso")]
        public Reserva Reserva { get; set; }
    }
}

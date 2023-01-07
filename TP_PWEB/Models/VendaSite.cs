using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;


namespace A.Models
{
    public class VendaSite
    {
        public int VendaSiteId { get; set; }
        public string? Cliente { get; set; }
        public DateTime Datavenda { get; set; }
        public int QtVendida { get; set; }
       
        [Display(Name = "Curso")]
        public int ReservaId { get; set; }
        
        [Display(Name = "Curso")]
        public Reserva? reserva { get; set; }

    }
}

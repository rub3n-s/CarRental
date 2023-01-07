using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using A.Models;

namespace A.Models
{
    public class GravaVendasSite
    {
        public string? Cliente { get; set; }
        public DateTime Datavenda { get; set; }
        public int QtVendida { get; set; }

        [Display(Name = "Reserva")]
        public int CursoID { get; set; }

        [Display(Name = "Reserva")]
        public Reserva? Reserva { get; set; }

        public VendaSite ToRegisto() => new VendaSite()
        {
            Cliente = this.Cliente,
            Datavenda = this.Datavenda,
            QtVendida = this.QtVendida,
            ReservaId = this.CursoID
        };

    }
}


    

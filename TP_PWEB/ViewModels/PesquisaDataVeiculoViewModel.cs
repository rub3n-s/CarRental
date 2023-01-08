using A.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace A.ViewModels
{
    public class PesquisaDataVeiculoViewModel
    {
        public List<Veiculo> ListaDeVeiculos { get; set; }
        public List <Reserva> ListaDeReservas { get; set; }
        public int NumResultados { get; set; }
        
        [Required]
        [Display(Name = "Localidade", Prompt = "Escolha o local de entrega...")]

        public string Local { get; set; }
        
        [Required]
        [Display(Name = "Data de Entrega")]
        public DateTime DataEntrega { get; set; }
        
        [Required]
        [Display(Name = "Data de Levantamento")]
        public DateTime DataLevanta { get; set; }
        
        public double Preco { get; set; }
    }
}

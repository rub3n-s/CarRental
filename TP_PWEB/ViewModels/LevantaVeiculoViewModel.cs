using A.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace A.ViewModels
{
    public class LevantaVeiculoViewModel
    {
        public int NumKM { get; set; }
        public bool Danos { get; set; }
        public string? Observacoes { get; set; }
        public string FuncionarioEmail { get; set; }

     

    }
}

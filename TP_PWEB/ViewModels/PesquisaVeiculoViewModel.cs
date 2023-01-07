using A.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace A.ViewModels
{
    public class PesquisaVeiculoViewModel
    {
        public List<Veiculo> ListaDeVeiculos { get; set; }
        public int NumResultados { get; set; }
        [Display(Name = "Pesquisa", Prompt = "Pesquise por categoria,modelo...")]
        public string TextoAPesquisar { get; set; }

    }
}

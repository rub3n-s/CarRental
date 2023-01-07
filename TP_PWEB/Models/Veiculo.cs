using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using A.Models;

namespace A.Models
{
    public class Veiculo
    {
        public int Id { get; set; }
        [Display(Name = "Marca")]
        public string Marca { get; set; }
        [Display(Name = "Modelo")]
        public string Modelo { get; set; }
        
        [Display(Name = "Disponivel")]
        public bool Disponivel { get; set; }
        [Display(Name = "Descricao")]
        public string Descricao { get; set; }
        [Display(Name = "Resumo")]
        
        public string DescricaoResumida { get; set; }
        [Display(Name = "Localidade")]
        public string Local { get; set; }

        [Required]
        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
        [Required]
        [Display(Name = "Empresa")]
        public int? EmpresaId { get; set; }
        public Empresa Empresa { get; set; }

        [Required]
        [Display(Name = "Preço Diário")]
        public double PrecoDiario { get; set; }
        
        [Display(Name = "")]
        public byte[]? Foto { get; set; }
        [NotMapped]
        public IFormFile? FotoFile { get; set; }


        public ICollection<Reserva>? Reservas { get; set; }
    }
}


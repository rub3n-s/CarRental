using Microsoft.AspNetCore.Identity;

namespace A.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string PrimeiroNome { get; set; }

        public string UltimoNome { get; set; }
        public DateTime DataNascimento { get; set; }                
        public int NIF { get; set; }

        public byte[]? Avatar { get; set; }

        public int? EmpresaId { get; set; }
        public bool Estado { get; set; }

        public Empresa? Empresa;

        //No caso sugerido pelo professor todos os AppUsers pertencem a uma empresa.
    }
}

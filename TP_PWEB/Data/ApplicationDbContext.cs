using A.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace A.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<Empresa> Empresa { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Veiculo> Veiculo { get; set; }
        public DbSet<EmpresaRating> EmpresaRating { get; set; }
        public DbSet<Reserva> Reserva { get; set; } 
        
        public DbSet<Entrega> Entrega { get; set; }


        public DbSet<Venda> VendasAnuais { get; set; }
        public DbSet<VendaSite> VendasMensais { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<A.Models.Levantamento>? Levantamento { get; set; }

      
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using A.Data;
using A.ViewModels;
using A.Models;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace A.Controllers
{
    public class VeiculosController : Controller
    {
        private readonly ApplicationDbContext _context;
      
        public VeiculosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [Authorize(Roles = "Cliente,Gestor,Funcionario")]
        public async Task<IActionResult> ListVeiculosOrder()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria.ToList(), "Id", "Nome");
            ViewData["EmpresaId"] = new SelectList(_context.Empresa.ToList(), "Id", "Nome");
            var veiculos = _context.Veiculo
                .Where(v => v.Disponivel == true)
                .Include(v => v.Categoria)
                .Include(v => v.Empresa)
                .OrderBy(v => v.PrecoDiario);
            return View(await veiculos.ToListAsync());
        }

        [AllowAnonymous]
        [Authorize(Roles = "Cliente,Gestor,Funcionario")]
        public async Task<IActionResult> ListVeiculosOrderDesc()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria.ToList(), "Id", "Nome");
            ViewData["EmpresaId"] = new SelectList(_context.Empresa.ToList(), "Id", "Nome");
            var veiculos = _context.Veiculo
                .Where(v => v.Disponivel == true)
                .Include(v => v.Categoria)
                .Include(v => v.Empresa)
                .OrderByDescending(v => v.PrecoDiario);
            return View(await veiculos.ToListAsync());
        }
        
        /*[Authorize(Roles = "Cliente,Gestor,Funcionario")]
        public async Task<IActionResult> ListVeiculosOrderEmp()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria.ToList(), "Id", "Nome");
            ViewData["EmpresaId"] = new SelectList(_context.Empresa.ToList(), "Id", "Nome");
            var veiculos = _context.Veiculo
                .Where(v => v.Disponivel == true)
                .Include(v => v.Categoria)
                .Include(v => v.Empresa)
                .OrderByDescending(v => v.PrecoDiario);
            return View(await veiculos.ToListAsync());
        } */
                
        // GET: Veiculos
        [Authorize(Roles = "Gestor,Funcionario")]
        public async Task<IActionResult> Index()
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == applicationUserId).First();
            var applicationDbContext = _context.Veiculo.Include(v => v.Categoria).Include(v => v.Empresa).Where(p => p.EmpresaId == user.EmpresaId);
            return View(await applicationDbContext.ToListAsync());
        }

        [AllowAnonymous]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> ListVeiculos()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Nome");
            ViewData["EmpresaId"] = new SelectList(_context.Empresa.ToList(), "Id", "Nome");
            var veiculos = _context.Veiculo
                .Where(v => v.Disponivel == true)
                .Include(p => p.Categoria)
                .Include(p => p.Empresa);
            return View(await veiculos.ToListAsync());
        }
        
        [AllowAnonymous]
        [Authorize(Roles = "Cliente")]
        [HttpPost]
        public async Task<IActionResult> ListVeiculos(string TextoAPesquisar, int CategoriaId)
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria.ToList(), "Id", "Nome");
            ViewData["EmpresaId"] = new SelectList(_context.Empresa.ToList(), "Id", "Nome");
            
            if (string.IsNullOrWhiteSpace(TextoAPesquisar))
                return View(_context.Veiculo.Where(v => v.Disponivel == true).Where(c => c.CategoriaId == CategoriaId).ToList());
            else
            {
                var resultado = from c in _context.Veiculo.Include(p => p.Empresa)
                    where (c.Marca.Contains(TextoAPesquisar) 
                           || c.Local.Contains(TextoAPesquisar) 
                           || c.Empresa.Nome.Contains(TextoAPesquisar))
                    select c;
                return View(resultado.Where(v => v.Disponivel == true).ToList());
            }
        }
   
        [AllowAnonymous]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Search(string? TextoAPesquisar)
        {
            PesquisaVeiculoViewModel pesquisaVM = new PesquisaVeiculoViewModel();

            if (string.IsNullOrWhiteSpace(TextoAPesquisar))
                pesquisaVM.ListaDeVeiculos = await _context.Veiculo.Include(v=> v.Categoria).Include(v=>v.Empresa).ToListAsync();
            else
            {
                pesquisaVM.ListaDeVeiculos = await _context.Veiculo.Include(c => c.Categoria).Include(c => c.Empresa).
                    Where(c => c.Modelo.Contains(TextoAPesquisar) ||
                               c.DescricaoResumida.Contains(TextoAPesquisar) ||
                               c.Descricao.Contains(TextoAPesquisar) ||
                               c.Marca.Contains(TextoAPesquisar) ||
                               c.Categoria.Nome.Contains(TextoAPesquisar) ||
                               c.Local.Contains(TextoAPesquisar)

                    ).ToListAsync();

                pesquisaVM.TextoAPesquisar = TextoAPesquisar;
            }

            pesquisaVM.NumResultados = pesquisaVM.ListaDeVeiculos.Count();
            
            return View(pesquisaVM);
        }
        
        [AllowAnonymous]
        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search( [Bind("TextoAPesquisar")] PesquisaVeiculoViewModel pesquisaVeiculo)
        {
            if (string.IsNullOrEmpty(pesquisaVeiculo.TextoAPesquisar))
            {
                pesquisaVeiculo.ListaDeVeiculos =
                    await _context.Veiculo.Include(p => p.Categoria).Include(p => p.Empresa).ToListAsync();

                pesquisaVeiculo.NumResultados = pesquisaVeiculo.ListaDeVeiculos.Count();
            }
            else
            {
                pesquisaVeiculo.ListaDeVeiculos =
                    await _context.Veiculo.Include(c => c.Categoria).Include(p => p.Empresa).Where(
                            c => c.Marca.Contains(pesquisaVeiculo.TextoAPesquisar) ||
                                 c.Modelo.Contains(pesquisaVeiculo.TextoAPesquisar))
                        .ToListAsync();

                pesquisaVeiculo.NumResultados = pesquisaVeiculo.ListaDeVeiculos.Count();
            }

            return View(pesquisaVeiculo);
        }

        [AllowAnonymous]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> SearchDate(string? Local, DateTime DataLevanta, DateTime DataEntrega)
        {
            PesquisaDataVeiculoViewModel pesquisaDVM = new PesquisaDataVeiculoViewModel();

            pesquisaDVM.ListaDeVeiculos = await _context.Veiculo.Include(c => c.Categoria).Include(c => c.Empresa).
                Where(c => c.Local.Contains(Local)).ToListAsync();
            //  pesquisaDVM.ListaDeReservas = 

            pesquisaDVM.Local = Local;
            pesquisaDVM.NumResultados = pesquisaDVM.ListaDeVeiculos.Count();

            return View(pesquisaDVM);
        }

        [AllowAnonymous]
        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchDate([Bind("DataLevanta,DataEntrega,Local")] PesquisaDataVeiculoViewModel pesquisaDataVeiculo)
        {
            pesquisaDataVeiculo.ListaDeVeiculos = await _context.Veiculo.Include(p => p.Categoria).Include(p => p.Empresa).Where(p=> p.Local == pesquisaDataVeiculo.Local).ToListAsync();
            pesquisaDataVeiculo.NumResultados = pesquisaDataVeiculo.ListaDeVeiculos.Count();

            var days = (pesquisaDataVeiculo.DataLevanta - pesquisaDataVeiculo.DataEntrega).TotalDays;
            if (days > 0)
            {
                //Mudar o Preco para várias categorias
                //if(pesquisaDataVeiculo.ListaDeVeiculos.Contains()
                pesquisaDataVeiculo.Preco = days * 5;
            }
            return View(pesquisaDataVeiculo);
        }

        [AllowAnonymous]
        [Authorize(Roles = "Gestor,Funcionario,Cliente")]
        // GET: Veiculos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Veiculo == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculo
                .Include(v => v.Categoria)
                .Include(v=>v.Empresa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (veiculo == null)
            {
                return NotFound();
            }

            return View(veiculo);
        }

        bool isValidFileType(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            switch (ext.ToLower())
            {
                case ".jpg": return true;
                case ".jpeg": return true;
                case ".png": return true;
                default: return false;
            }
        }


        // GET: Veiculos/Create
        [Authorize(Roles = "Gestor,Funcionario")]
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Nome");
            return View();
        }

        // POST: Veiculos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Gestor,Funcionario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Marca,Modelo,Disponivel,Descricao,DescricaoResumida,Local,PrecoDiario,CategoriaId,EmpresaId")] Veiculo veiculo, IFormFile FotoFile)
        {
            ModelState.Remove(nameof(veiculo.Categoria));
            ModelState.Remove(nameof(veiculo.Empresa));
            ModelState.Remove(nameof(veiculo.EmpresaId));
            ModelState.Remove(nameof(veiculo.Reservas));
            
            using (var dataStream = new MemoryStream())
            {
                await FotoFile.CopyToAsync(dataStream);
                veiculo.Foto = dataStream.ToArray();
            }
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var manager = _context.Users.Where(x => x.Id == applicationUserId).First();

            veiculo.EmpresaId = manager.EmpresaId;
            
            if (ModelState.IsValid)

            {
                _context.Add(veiculo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Nome", veiculo.CategoriaId);
          
            return View(veiculo);
        }

        // GET: Veiculos/Edit/5
        [Authorize(Roles = "Gestor,Funcionario")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Veiculo == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculo.FindAsync(id);
            if (veiculo == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Nome", veiculo.CategoriaId);
            ViewData["EmpresaId"] = new SelectList(_context.Empresa, "Id", "Nome", veiculo.EmpresaId);
            return View(veiculo);
        }

        // POST: Veiculos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Gestor,Funcionario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Marca,Modelo,Disponivel,Local,Descricao,DescricaoResumida,PrecoDiario,Foto,CategoriaId,EmpresaId")] Veiculo veiculo)
        {
            if (id != veiculo.Id)
            {
                return NotFound();
            }
            ModelState.Remove(nameof(veiculo.Categoria));
            ModelState.Remove(nameof(veiculo.Empresa));
            ModelState.Remove(nameof(veiculo.EmpresaId));
            ModelState.Remove(nameof(veiculo.Reservas));

            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var manager = _context.Users.Where(x => x.Id == applicationUserId).First();
                    
            veiculo.EmpresaId = manager.EmpresaId;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(veiculo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VeiculoExists(veiculo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Nome", veiculo.CategoriaId);
            ViewData["EmpresaId"] = new SelectList(_context.Empresa, "Id", "Nome", veiculo.EmpresaId);
            //  ViewData["GestorId"] = new SelectList(_context.Gestor, "Id", "Nome", veiculo.GestorId);
            //  ViewData["FuncionarioId"] = new SelectList(_context.Funcionario, "Id", "Nome", veiculo.FuncionarioId);
            return View(veiculo);
        }

        // GET: Veiculos/Delete/5
        [Authorize(Roles = "Gestor,Funcionario")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Veiculo == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculo
                .Include(v => v.Categoria)
                .Include(v=> v.Empresa)
                //   .Include(v=>v.Gestor)
                //  .Include(v=>v.Funcionario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (veiculo == null)
            {
                return NotFound();
            }

            var reserva = _context.Reserva.Where(r => r.VeiculoId == id);
            if (reserva != null)
            {
                TempData["msg"] = "<script>alert('Este veículo já tem uma reserva associada.');</script>";
                return RedirectToAction(nameof(Index));
            }

            return View(veiculo);
        }

        // POST: Veiculos/Delete/5
        [Authorize(Roles = "Gestor,Funcionario")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Veiculo == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Veiculo'  is null.");
            }
            var veiculo = await _context.Veiculo.FindAsync(id);
            if (veiculo != null)
            {
                _context.Veiculo.Remove(veiculo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VeiculoExists(int id)
        {
            return _context.Veiculo.Any(e => e.Id == id);
        }
    }
}
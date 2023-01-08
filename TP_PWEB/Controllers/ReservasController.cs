using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using A.Data;
using A.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using A.CartModels;
using A.Helpers;
using Microsoft.AspNetCore.Identity;

namespace A.Controllers
{
    public class ReservasController : Controller
    {

        public const string SessionAcao = "1";
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public ReservasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservas
        [Authorize(Roles = "Gestor,Funcionario")]
        public async Task<IActionResult> Index()
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var customer = _context.Users.Where(x => x.Id == applicationUserId).First();

            var reservas = _context.Reserva.Include(r => r.Empresa).Include(r => r.Veiculo).Include(r=>r.ApplicationUser).Where(p => p.EmpresaId == (customer.EmpresaId));
            return View(await reservas.ToListAsync());
        }
        
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> ListReservas()
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var customer = _context.Users.Where(x => x.Id == applicationUserId).First();

            var reservas = _context.Reserva.Where(p => p.ApplicationUserId == customer.Id).Include(v => v.Veiculo).Include(a => a.ApplicationUser);

            return View(await reservas.ToListAsync());
        }
        
        // GET: Reservas/Details/5
        [Authorize(Roles = "Gestor,Funcionario,Cliente")]
        public async Task<IActionResult> Details(int? id)
        {
            
            if (id == null || _context.Reserva == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva
                .Include(r => r.Empresa)
                .Include(r => r.Veiculo)
                .Include(r => r.Entrega)
                .Include(r => r.Levantamento)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }
        
        [Authorize(Roles = "Cliente")]
        public IActionResult CreateFromDetails()
        {
            ViewData["VeiculoId"] = new SelectList(_context.Veiculo, "Id", "Modelo");
            return View();
        }

        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromDetails(int id, [Bind("DataLevantamento,DataEntrega,VeiculoId,EmpresaId")] Reserva reserva, int qualact)
        {
            ViewData["VeiculoId"] = new SelectList(_context.Veiculo, "Id", "Modelo", reserva.VeiculoId);

            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = _context.Users.Where(x => x.Id == applicationUserId).First();
            var veiculo = _context.Veiculo.Where(x => x.Id == id).First();
            
            reserva.VeiculoId = id;
            reserva.EmpresaId = veiculo.EmpresaId;
        
            reserva.ApplicationUserId = customer.Id;
            reserva.Estado = "Pendente";

            TimeSpan ts = (TimeSpan)(reserva.DataLevantamento - reserva.DataEntrega);
            int days = Math.Abs(ts.Days);
            reserva.Preco = (float?)(veiculo.PrecoDiario * days);

            _context.Add(reserva);
            await _context.SaveChangesAsync();

            var CarrinhoDeCompras = HttpContext.Session.GetJson<Carrinho>("CarrinhoDeCompras") ?? new Carrinho();
            CarrinhoDeCompras.AddItem(new CarrinhoItem { ReservaId = reserva.Id, PrecoUnit = (decimal)reserva.Preco }, 1);
            HttpContext.Session.SetJson("CarrinhoDeCompras", CarrinhoDeCompras);

            HttpContext.Session.SetInt32(SessionAcao, qualact);
                
            return RedirectToAction("Carrinho");
        }
        
        [Authorize(Roles = "Gestor,Funcionario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReservaAccept([Bind("Id,DataLevantamento,DataEntrega,VeiculoId,EmpresaId,ApplicationUserId,Preco")] Reserva reserva)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var funcionario = _context.Users.Where(x => x.Id == applicationUserId).FirstOrDefault();

            ViewData["VeiculoId"] = new SelectList(_context.Veiculo, "Id", "Modelo", reserva.VeiculoId);

            if (String.Compare(reserva.Estado, "Aceite") != 0)
            {
                reserva.Estado = "Aceite";
                reserva.EmpresaId = funcionario.EmpresaId;
                _context.Update(reserva);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Gestor,Funcionario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReservaDecline([Bind("Id,DataLevantamento,DataEntrega,VeiculoId,EmpresaId,ApplicationUserId,Preco")] Reserva reserva)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var funcionario = _context.Users.Where(x => x.Id == applicationUserId).FirstOrDefault();

            ViewData["VeiculoId"] = new SelectList(_context.Veiculo, "Id", "Modelo", reserva.VeiculoId);

            if (String.Compare(reserva.Estado, "Recusado") != 0)
            {
                reserva.Estado = "Recusado";
                reserva.EmpresaId = funcionario.EmpresaId;
                _context.Update(reserva);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        
        // GET: Reservas/Create
        public IActionResult Create()
        {
            ViewData["EmpresaId"] = new SelectList(_context.Empresa, "Id", "Id");
            ViewData["VeiculoId"] = new SelectList(_context.Veiculo, "Id", "Id");
            return View();
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Preco,Estado,DataEntrega,DataLevantamento,VeiculoId,EmpresaId,ApplicationUserId")] Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmpresaId"] = new SelectList(_context.Empresa, "Id", "Id", reserva.EmpresaId);
            ViewData["VeiculoId"] = new SelectList(_context.Veiculo, "Id", "Id", reserva.VeiculoId);
            return View(reserva);
        }

        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reserva == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewData["EmpresaId"] = new SelectList(_context.Empresa, "Id", "Id", reserva.EmpresaId);
            ViewData["VeiculoId"] = new SelectList(_context.Veiculo, "Id", "Id", reserva.VeiculoId);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Preco,Estado,DataEntrega,DataLevantamento,VeiculoId,EmpresaId,ApplicationUserId")] Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
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
            ViewData["EmpresaId"] = new SelectList(_context.Empresa, "Id", "Id", reserva.EmpresaId);
            ViewData["VeiculoId"] = new SelectList(_context.Veiculo, "Id", "Id", reserva.VeiculoId);
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reserva == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva
                .Include(r => r.Empresa)
                .Include(r => r.Veiculo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [Authorize(Roles = "Gestor,Funcionario")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reserva == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reserva'  is null.");
            }
            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva != null)
            {
                _context.Reserva.Remove(reserva);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reserva.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Comprar(int? id, int qualact)
        {
            if (id == null || _context.Reserva == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            var CarrinhoDeCompras = HttpContext.Session.GetJson<Carrinho>("CarrinhoDeCompras") ?? new Carrinho();
            CarrinhoDeCompras.AddItem(new CarrinhoItem { ReservaId = reserva.Id, PrecoUnit = (decimal)reserva.Preco }, 1);
            HttpContext.Session.SetJson("CarrinhoDeCompras", CarrinhoDeCompras);

            HttpContext.Session.SetInt32(SessionAcao, qualact);

            return RedirectToAction("Carrinho");
        }
        
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Carrinho()
        {
            var CarrinhoDeCompras = HttpContext.Session.GetJson<Carrinho>("CarrinhoDeCompras") ?? new Carrinho();

            return View(CarrinhoDeCompras);
        }
        
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> PagarCompras()
        {
            GravaVendasSite gravaVenda = new GravaVendasSite();

            gravaVenda.Cliente = _userManager.GetUserId(User);

            var RegistaCompras = HttpContext.Session.GetJson<Carrinho>("CarrinhoDeCompras");

            foreach (var item in RegistaCompras.items)
            {
                gravaVenda.CursoID = item.ReservaId;
                gravaVenda.QtVendida = item.Quantidade;
                gravaVenda.Datavenda = DateTime.Now.Date;

                await _context.VendasMensais.AddAsync(gravaVenda.ToRegisto());
                await _context.SaveChangesAsync();
            }

            return View();
        }
        
        public class SelectEmpresa
        {
            public string Id { get; set; }
        }
    }
}
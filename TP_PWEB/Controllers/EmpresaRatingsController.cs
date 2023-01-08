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

namespace A.Controllers
{
    public class EmpresaRatingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmpresaRatingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EmpresaRatings
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.EmpresaRating.Include(e => e.Empresa);
            return View(await applicationDbContext.ToListAsync());
        }
        
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> ListAval()
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = _context.Users.Where(x => x.Id == applicationUserId).First();

            var ratings = _context.EmpresaRating.Where(p => p.ApplicationUserId == customer.Id);

            return View(await ratings.ToListAsync());
        }

        [Authorize(Roles = "Cliente")]
        public IActionResult CreateFromDetails()
        {
            ViewData["EmpresaId"] = new SelectList(_context.Veiculo, "Id", "Nome");
            return View();
        }

        /*[Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromDetails(int id, [Bind("DataLevantamento,DataEntrega,VeiculoId,EmpresaId")] Reserva reserva)
        {
            ViewData["VeiculoId"] = new SelectList(_context.Veiculo, "Id", "Modelo", reserva.VeiculoId);

            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = _context.Users.Where(x => x.Id == applicationUserId).First();
            var veiculo = _context.Veiculo.Where(x => x.Id == id).First();

            reserva.VeiculoId = id;
            reserva.EmpresaId = veiculo.EmpresaId;
        
            reserva.ApplicationUserId = customer.Id;
            reserva.Estado = "Pendente";

            _context.Add(reserva);
            await _context.SaveChangesAsync();
            return RedirectToAction("ListReservas", "Reservas");
        } */

        // GET: EmpresaRatings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EmpresaRating == null)
            {
                return NotFound();
            }

            var empresaRating = await _context.EmpresaRating
                .Include(e => e.Empresa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empresaRating == null)
            {
                return NotFound();
            }

            return View(empresaRating);
        }

        // GET: EmpresaRatings/Create
        public IActionResult Create(int? id)
        {
            ViewData["EmpresaId"] = new SelectList(_context.Empresa, "Id", "Id");
            return View();
        }

        // POST: EmpresaRatings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Comentario,Avaliacao,EmpresaId,ApplicationUserId")] EmpresaRating empresaRating, int id)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            empresaRating.ApplicationUserId = applicationUserId;
            
            var reserva = _context.Reserva.Where(v => v.Id == id).First();
            empresaRating.EmpresaId = (int) reserva.EmpresaId;
            
            ModelState.Remove(nameof(empresaRating.EmpresaId));
            ModelState.Remove(nameof(empresaRating.Empresa));
            ModelState.Remove(nameof(empresaRating.ApplicationUser));
            ModelState.Remove(nameof(empresaRating.ApplicationUserId));
            
            if (ModelState.IsValid)
            {
                _context.Add(empresaRating);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmpresaId"] = new SelectList(_context.Empresa, "Id", "Id", empresaRating.EmpresaId);
            return View(empresaRating);
        }

        // GET: EmpresaRatings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EmpresaRating == null)
            {
                return NotFound();
            }

            var empresaRating = await _context.EmpresaRating.FindAsync(id);
            if (empresaRating == null)
            {
                return NotFound();
            }
            ViewData["EmpresaId"] = new SelectList(_context.Empresa, "Id", "Id", empresaRating.EmpresaId);
            return View(empresaRating);
        }

        // POST: EmpresaRatings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Comentario,Avaliacao,EmpresaId,ApplicationUserId")] EmpresaRating empresaRating)
        {
            if (id != empresaRating.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empresaRating);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpresaRatingExists(empresaRating.Id))
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
            ViewData["EmpresaId"] = new SelectList(_context.Empresa, "Id", "Id", empresaRating.EmpresaId);
            return View(empresaRating);
        }

        // GET: EmpresaRatings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EmpresaRating == null)
            {
                return NotFound();
            }

            var empresaRating = await _context.EmpresaRating
                .Include(e => e.Empresa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empresaRating == null)
            {
                return NotFound();
            }

            return View(empresaRating);
        }

        // POST: EmpresaRatings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EmpresaRating == null)
            {
                return Problem("Entity set 'ApplicationDbContext.EmpresaRating'  is null.");
            }
            var empresaRating = await _context.EmpresaRating.FindAsync(id);
            if (empresaRating != null)
            {
                _context.EmpresaRating.Remove(empresaRating);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpresaRatingExists(int id)
        {
          return _context.EmpresaRating.Any(e => e.Id == id);
        }
    }
}

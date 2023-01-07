using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using A.Data;
using A.Models;
using Microsoft.AspNetCore.Authorization;

namespace A.Controllers
{
    public class EntregasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EntregasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Entregas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Entrega.Include(e => e.Reserva);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Entregas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Entrega == null)
            {
                return NotFound();
            }

            var entrega = await _context.Entrega
                .Include(e => e.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entrega == null)
            {
                return NotFound();
            }

            return View(entrega);
        }

        // GET: Entregas/Create
        public IActionResult Create()
        {
            ViewData["Id"] = new SelectList(_context.Reserva, "Id", "Id");
            return View();
        }

        // POST: Entregas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Gestor,Funcionario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NumKM,Danos,Observacoes,FuncionarioEmail,ReservaId")] Entrega entrega, int id)
        {
            entrega.ReservaId = id;
            
            if (ModelState.IsValid)
            {
                _context.Add(entrega);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id"] = new SelectList(_context.Reserva, "Id", "Id", entrega.Id);
            return View(entrega);
        }

        // GET: Entregas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Entrega == null)
            {
                return NotFound();
            }

            var entrega = await _context.Entrega.FindAsync(id);
            if (entrega == null)
            {
                return NotFound();
            }
            ViewData["Id"] = new SelectList(_context.Reserva, "Id", "Id", entrega.Id);
            return View(entrega);
        }

        // POST: Entregas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumKM,Danos,Observacoes,FuncionarioEmail,ReservaId")] Entrega entrega)
        {
            if (id != entrega.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entrega);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntregaExists(entrega.Id))
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
            ViewData["Id"] = new SelectList(_context.Reserva, "Id", "Id", entrega.Id);
            return View(entrega);
        }

        // GET: Entregas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Entrega == null)
            {
                return NotFound();
            }

            var entrega = await _context.Entrega
                .Include(e => e.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entrega == null)
            {
                return NotFound();
            }

            return View(entrega);
        }

        // POST: Entregas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Entrega == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Entrega'  is null.");
            }
            var entrega = await _context.Entrega.FindAsync(id);
            if (entrega != null)
            {
                _context.Entrega.Remove(entrega);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EntregaExists(int id)
        {
          return (_context.Entrega?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

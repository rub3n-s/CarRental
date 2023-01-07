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
    public class LevantamentosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LevantamentosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Levantamentos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Levantamento.Include(l => l.Reserva);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Levantamentos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Levantamento == null)
            {
                return NotFound();
            }

            var levantamento = await _context.Levantamento
                .Include(l => l.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (levantamento == null)
            {
                return NotFound();
            }

            return View(levantamento);
        }

        // GET: Levantamentos/Create
        public IActionResult Create()
        {
            ViewData["Id"] = new SelectList(_context.Reserva, "Id", "Id");
            return View();
        }

        // POST: Levantamentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Gestor,Funcionario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NumKM,Danos,Observacoes,FuncionarioEmail,ReservaId")] Levantamento levantamento, int id)
        {
            levantamento.ReservaId = id;
            
            ModelState.Remove(nameof(levantamento.Reserva));
            ModelState.Remove(nameof(levantamento.ReservaId));
            
            if (ModelState.IsValid)
            {
                _context.Add(levantamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id"] = new SelectList(_context.Reserva, "Id", "Id", levantamento.Id);
            return View(levantamento);
        }

        // GET: Levantamentos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Levantamento == null)
            {
                return NotFound();
            }

            var levantamento = await _context.Levantamento.FindAsync(id);
            if (levantamento == null)
            {
                return NotFound();
            }
            ViewData["Id"] = new SelectList(_context.Reserva, "Id", "Id", levantamento.Id);
            return View(levantamento);
        }

        // POST: Levantamentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumKM,Danos,Observacoes,FuncionarioEmail,ReservaId")] Levantamento levantamento)
        {
            if (id != levantamento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(levantamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LevantamentoExists(levantamento.Id))
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
            ViewData["Id"] = new SelectList(_context.Reserva, "Id", "Id", levantamento.Id);
            return View(levantamento);
        }

        // GET: Levantamentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Levantamento == null)
            {
                return NotFound();
            }

            var levantamento = await _context.Levantamento
                .Include(l => l.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (levantamento == null)
            {
                return NotFound();
            }

            return View(levantamento);
        }

        // POST: Levantamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Levantamento == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Levantamento'  is null.");
            }
            var levantamento = await _context.Levantamento.FindAsync(id);
            if (levantamento != null)
            {
                _context.Levantamento.Remove(levantamento);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LevantamentoExists(int id)
        {
          return (_context.Levantamento?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

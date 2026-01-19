using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FilmCatalog.Web.Data;
using FilmCatalog.Web.Models;

namespace FilmCatalog.Web.Controllers
{
    public class DirectorsController : Controller
    {
        private readonly FilmCatalogContext _context;

        public DirectorsController(FilmCatalogContext context)
        {
            _context = context;
        }

        // GET: Directors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Directors.ToListAsync());
        }

        // GET: Directors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var director = await _context.Directors
                .Include(d => d.Films)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (director == null)
            {
                return NotFound();
            }

            return View(director);
        }

        // GET: Directors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Directors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Country")] Director director)
        {
            if (ModelState.IsValid)
            {
                _context.Add(director);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Режисера успішно створено!";
                return RedirectToAction(nameof(Index));
            }
            return View(director);
        }

        // GET: Directors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var director = await _context.Directors.FindAsync(id);
            if (director == null)
            {
                return NotFound();
            }
            return View(director);
        }

        // POST: Directors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Country")] Director director)
        {
            if (id != director.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(director);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Режисера успішно оновлено!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DirectorExists(director.Id))
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
            return View(director);
        }

        // GET: Directors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var director = await _context.Directors
                .FirstOrDefaultAsync(m => m.Id == id);

            if (director == null)
            {
                return NotFound();
            }

            return View(director);
        }

        // POST: Directors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var director = await _context.Directors.FindAsync(id);
            if (director != null)
            {
                // Перевірка чи є фільми цього режисера
                var hasFilms = await _context.Films.AnyAsync(f => f.DirectorId == id);
                if (hasFilms)
                {
                    TempData["ErrorMessage"] = "Неможливо видалити режисера, який має пов'язані фільми!";
                    return RedirectToAction(nameof(Index));
                }

                _context.Directors.Remove(director);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Режисера успішно видалено!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DirectorExists(int id)
        {
            return _context.Directors.Any(e => e.Id == id);
        }
    }
}

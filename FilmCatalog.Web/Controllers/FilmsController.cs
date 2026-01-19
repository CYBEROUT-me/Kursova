using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FilmCatalog.Web.Data;
using FilmCatalog.Web.Models;

namespace FilmCatalog.Web.Controllers
{
    public class FilmsController : Controller
    {
        private readonly FilmCatalogContext _context;

        public FilmsController(FilmCatalogContext context)
        {
            _context = context;
        }

        // GET: Films
        public async Task<IActionResult> Index(string searchString, int? genreId)
        {
            var films = _context.Films
                .Include(f => f.Genre)
                .Include(f => f.Director)
                .AsQueryable();

            // Пошук за назвою
            if (!string.IsNullOrEmpty(searchString))
            {
                films = films.Where(f => f.Title.Contains(searchString));
            }

            // Фільтрація за жанром
            if (genreId.HasValue)
            {
                films = films.Where(f => f.GenreId == genreId.Value);
            }

            ViewBag.Genres = new SelectList(await _context.Genres.ToListAsync(), "Id", "Name");
            ViewBag.SearchString = searchString;
            ViewBag.GenreId = genreId;

            // Завантажуємо, потім сортуємо на клієнті (SQLite не підтримує ORDER BY decimal)
            var filmsList = await films.ToListAsync();
            return View(filmsList.OrderByDescending(f => f.Rating ?? 0).ToList());
        }

        // GET: Films/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .Include(f => f.Genre)
                .Include(f => f.Director)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // GET: Films/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Genres = new SelectList(await _context.Genres.ToListAsync(), "Id", "Name");
            ViewBag.Directors = new SelectList(await _context.Directors.ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: Films/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Year,Description,Rating,GenreId,DirectorId")] Film film)
        {
            if (ModelState.IsValid)
            {
                _context.Add(film);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Фільм успішно створено!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Genres = new SelectList(await _context.Genres.ToListAsync(), "Id", "Name", film.GenreId);
            ViewBag.Directors = new SelectList(await _context.Directors.ToListAsync(), "Id", "Name", film.DirectorId);
            return View(film);
        }

        // GET: Films/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films.FindAsync(id);
            if (film == null)
            {
                return NotFound();
            }

            ViewBag.Genres = new SelectList(await _context.Genres.ToListAsync(), "Id", "Name", film.GenreId);
            ViewBag.Directors = new SelectList(await _context.Directors.ToListAsync(), "Id", "Name", film.DirectorId);
            return View(film);
        }

        // POST: Films/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Year,Description,Rating,GenreId,DirectorId")] Film film)
        {
            if (id != film.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(film);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Фільм успішно оновлено!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmExists(film.Id))
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

            ViewBag.Genres = new SelectList(await _context.Genres.ToListAsync(), "Id", "Name", film.GenreId);
            ViewBag.Directors = new SelectList(await _context.Directors.ToListAsync(), "Id", "Name", film.DirectorId);
            return View(film);
        }

        // GET: Films/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .Include(f => f.Genre)
                .Include(f => f.Director)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // POST: Films/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var film = await _context.Films.FindAsync(id);
            if (film != null)
            {
                _context.Films.Remove(film);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Фільм успішно видалено!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool FilmExists(int id)
        {
            return _context.Films.Any(e => e.Id == id);
        }
    }
}

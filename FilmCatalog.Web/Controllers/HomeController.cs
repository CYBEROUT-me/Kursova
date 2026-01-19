using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FilmCatalog.Web.Data;

namespace FilmCatalog.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly FilmCatalogContext _context;

        public HomeController(FilmCatalogContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.FilmsCount = await _context.Films.CountAsync();
            ViewBag.GenresCount = await _context.Genres.CountAsync();
            ViewBag.DirectorsCount = await _context.Directors.CountAsync();
            
            // Завантажуємо фільми, потім сортуємо на клієнті (SQLite не підтримує ORDER BY decimal)
            var films = await _context.Films
                .Include(f => f.Genre)
                .Include(f => f.Director)
                .ToListAsync();
            
            ViewBag.TopFilms = films
                .OrderByDescending(f => f.Rating ?? 0)
                .Take(5)
                .ToList();

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}

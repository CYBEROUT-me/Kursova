using Microsoft.EntityFrameworkCore;
using FilmCatalog.Web.Models;

namespace FilmCatalog.Web.Data
{
    public class FilmCatalogContext : DbContext
    {
        public FilmCatalogContext(DbContextOptions<FilmCatalogContext> options)
            : base(options)
        {
        }

        public DbSet<Film> Films { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Director> Directors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Налаштування зв'язків
            modelBuilder.Entity<Film>()
                .HasOne(f => f.Genre)
                .WithMany(g => g.Films)
                .HasForeignKey(f => f.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Film>()
                .HasOne(f => f.Director)
                .WithMany(d => d.Films)
                .HasForeignKey(f => f.DirectorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Початкові дані - Жанри
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Драма" },
                new Genre { Id = 2, Name = "Комедія" },
                new Genre { Id = 3, Name = "Бойовик" },
                new Genre { Id = 4, Name = "Фантастика" },
                new Genre { Id = 5, Name = "Трилер" },
                new Genre { Id = 6, Name = "Жахи" }
            );

            // Початкові дані - Режисери
            modelBuilder.Entity<Director>().HasData(
                new Director { Id = 1, Name = "Крістофер Нолан", Country = "Велика Британія" },
                new Director { Id = 2, Name = "Квентін Тарантіно", Country = "США" },
                new Director { Id = 3, Name = "Стівен Спілберг", Country = "США" },
                new Director { Id = 4, Name = "Мартін Скорсезе", Country = "США" },
                new Director { Id = 5, Name = "Дені Вільньов", Country = "Канада" }
            );

            // Початкові дані - Фільми
            modelBuilder.Entity<Film>().HasData(
                new Film { Id = 1, Title = "Інтерстеллар", Year = 2014, Description = "Науково-фантастичний фільм про подорож крізь чорну діру", Rating = 8.7m, GenreId = 4, DirectorId = 1 },
                new Film { Id = 2, Title = "Кримінальне чтиво", Year = 1994, Description = "Культовий кримінальний фільм", Rating = 8.9m, GenreId = 5, DirectorId = 2 },
                new Film { Id = 3, Title = "Список Шіндлера", Year = 1993, Description = "Історична драма про Голокост", Rating = 9.0m, GenreId = 1, DirectorId = 3 },
                new Film { Id = 4, Title = "Початок", Year = 2010, Description = "Фільм про сни всередині снів", Rating = 8.8m, GenreId = 4, DirectorId = 1 },
                new Film { Id = 5, Title = "Дюна", Year = 2021, Description = "Екранізація знаменитого роману", Rating = 8.0m, GenreId = 4, DirectorId = 5 }
            );
        }
    }
}

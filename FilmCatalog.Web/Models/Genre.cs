using System.ComponentModel.DataAnnotations;

namespace FilmCatalog.Web.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва жанру є обов'язковою")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Назва має бути від 2 до 100 символів")]
        [Display(Name = "Назва жанру")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Film>? Films { get; set; }
    }
}

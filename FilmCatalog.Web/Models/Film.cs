using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmCatalog.Web.Models
{
    public class Film
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва фільму є обов'язковою")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Назва має бути від 1 до 200 символів")]
        [Display(Name = "Назва")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Рік випуску є обов'язковим")]
        [Range(1895, 2030, ErrorMessage = "Рік має бути від 1895 до 2030")]
        [Display(Name = "Рік випуску")]
        public int Year { get; set; }

        [StringLength(2000, ErrorMessage = "Опис не може перевищувати 2000 символів")]
        [Display(Name = "Опис")]
        public string? Description { get; set; }

        [Range(1, 10, ErrorMessage = "Рейтинг має бути від 1 до 10")]
        [Display(Name = "Рейтинг")]
        public decimal? Rating { get; set; }

        [Required(ErrorMessage = "Жанр є обов'язковим")]
        [Display(Name = "Жанр")]
        public int GenreId { get; set; }

        [ForeignKey("GenreId")]
        public Genre? Genre { get; set; }

        [Required(ErrorMessage = "Режисер є обов'язковим")]
        [Display(Name = "Режисер")]
        public int DirectorId { get; set; }

        [ForeignKey("DirectorId")]
        public Director? Director { get; set; }
    }
}

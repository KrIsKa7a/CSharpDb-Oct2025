namespace MoviesApp.Models
{
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidation;

    public class Movie
    {
        [Key]
        public int Id { get; set; }
        
        // Required by default, because nullable reference types are enabled
        [MaxLength(MovieTitleMaxLength)]
        public string Title { get; set; } = null!;

        [MaxLength(MovieGenreMaxLength)]
        public string Genre { get; set; } = null!;
        
        public DateOnly ReleaseDate { get; set; }

        [MaxLength(MovieDirectorMaxLength)]
        public string Director { get; set; } = null!;

        public int Duration { get; set; }

        [MaxLength(MovieDescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [MaxLength(MovieImageUrlMaxLength)]
        public string? ImageUrl { get; set; }
    }
}

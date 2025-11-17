namespace MoviesApp.ViewModels.Movies
{
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidation;

    public class AddMovieFormModel
    {
        [Required]
        [MinLength(MovieTitleMinLength)]
        [MaxLength(MovieTitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MinLength(MovieGenreMinLength)]
        [MaxLength(MovieGenreMaxLength)]
        public string Genre { get; set; } = null!;

        [Required]
        [MinLength(MovieDirectorMinLength)]
        [MaxLength(MovieDirectorMaxLength)]
        public string Director { get; set; } = null!;

        [Range(MovieDurationMinValue, MovieDurationMaxValue)]
        public int Duration { get; set; }

        public DateTime ReleaseDate { get; set; }

        [Required]
        [MinLength(MovieDescriptionMinLength)]
        [MaxLength(MovieDescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [MaxLength(MovieImageUrlMaxLength)]
        public string? ImageUrl { get; set; }
    }
}

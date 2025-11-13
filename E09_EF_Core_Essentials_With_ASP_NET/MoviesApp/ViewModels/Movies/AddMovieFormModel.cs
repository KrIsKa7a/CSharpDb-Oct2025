namespace MoviesApp.ViewModels.Movies
{
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidation;

    public class AddMovieFormModel
    {
        [MaxLength(MovieTitleMaxLength)]
        public string Title { get; set; } = null!;

        [MaxLength(MovieGenreMaxLength)]
        public string Genre { get; set; } = null!;

        [MaxLength(MovieDirectorMaxLength)]
        public string Director { get; set; } = null!;

        [Range(MovieDurationMinLength, MovieDurationMaxLength)]
        public int Duration { get; set; }

        public DateTime ReleaseDate { get; set; }

        [MaxLength(MovieDescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [MaxLength(MovieImageUrlMaxLength)]
        public string? ImageUrl { get; set; }
    }
}

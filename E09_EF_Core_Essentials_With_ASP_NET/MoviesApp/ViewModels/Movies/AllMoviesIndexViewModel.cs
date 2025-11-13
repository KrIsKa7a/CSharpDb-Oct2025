namespace MoviesApp.ViewModels.Movies
{
    public class AllMoviesIndexViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Genre { get; set; } = null!;

        public string Director { get; set; } = null!;

        public string ReleaseDate { get; set; } = null!;

        public int Duration { get; set; }

        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }
    }
}

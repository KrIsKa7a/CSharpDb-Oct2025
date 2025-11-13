namespace MoviesApp.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Watchlist
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Movie))]
        public int MovieId { get; set; }

        public Movie Movie { get; set; } = null!;
    }
}

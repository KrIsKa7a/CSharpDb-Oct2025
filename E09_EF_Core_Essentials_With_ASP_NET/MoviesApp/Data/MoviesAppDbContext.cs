namespace MoviesApp.Data
{
    using Microsoft.EntityFrameworkCore;

    using Models;

    public class MoviesAppDbContext : DbContext
    {
        public MoviesAppDbContext(DbContextOptions<MoviesAppDbContext> options)
            : base(options)
        {

        }

        public virtual DbSet<Movie> Movies { get; set; } = null!;

        public virtual DbSet<Watchlist> Watchlists { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // If you later want Fluent API config, put it here.
        }
    }
}

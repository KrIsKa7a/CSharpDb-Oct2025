namespace MoviesApp.Services
{
    using Microsoft.EntityFrameworkCore;

    using Data;
    using Interfaces;
    using Models;

    public class MoviesService : IMoviesService
    {
        private readonly MoviesAppDbContext dbContext;

        public MoviesService(MoviesAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(Movie movie)
        {
            await this.dbContext.Movies.AddAsync(movie);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Movie? movieToDelete = await this.dbContext.Movies.FindAsync(id);

            // The Watchlist-Movie relationship is configured with Cascade Delete
            // => All Watchlist entries referencing this movie will be deleted automatically
            this.dbContext.Movies.Remove(movieToDelete!);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            bool movieExists = await this.dbContext
                .Movies
                .AnyAsync(m => m.Id == id);

            return movieExists;
        }

        // This is example for bad architectural design -> We can't perform optimization of the query payload
        // TODO: Optimize the design to return the specific ViewModel/DTO
        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            IEnumerable<Movie> allMovies = await this.dbContext
                .Movies
                .AsNoTracking()
                .ToArrayAsync();

            return allMovies;
        }

        public async Task<Movie?> GetByIdAsync(int id)
        {
            Movie? movie = await this.dbContext.Movies.FindAsync(id);

            return movie;
        }
    }
}

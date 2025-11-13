namespace MoviesApp.Services
{
    using Microsoft.EntityFrameworkCore;

    using Data;
    using Interfaces;
    using Models;

    public class WatchlistService : IWatchlistService
    {
        private readonly MoviesAppDbContext dbContext;

        public WatchlistService(MoviesAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(int movieId)
        {
            Watchlist newWatchlistEntry = new Watchlist
            {
                MovieId = movieId
            };

            await this.dbContext.Watchlists.AddAsync(newWatchlistEntry);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            IEnumerable<Movie> allMoviesInWatchlists = await this.dbContext
                .Watchlists
                .Include(w => w.Movie)
                .AsNoTracking()
                .Select(w => w.Movie)
                .ToArrayAsync();

            return allMoviesInWatchlists;
        }

        public async Task RemoveAsync(int movieId)
        {
            // We assume that we will have a collection with single element in case of correct add logic
            IEnumerable<Watchlist> watchlistEntriesToRemove = await this.dbContext
                .Watchlists
                .AsNoTracking()
                .Where(w => w.MovieId == movieId)
                .ToArrayAsync();

            this.dbContext.Watchlists.RemoveRange(watchlistEntriesToRemove);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<bool> MovieExistsInWatchlistAsync(int movieId)
        {
            bool movieExists = await this.dbContext
                .Watchlists
                .AnyAsync(w => w.MovieId == movieId);
            
            return movieExists;
        }
    }
}

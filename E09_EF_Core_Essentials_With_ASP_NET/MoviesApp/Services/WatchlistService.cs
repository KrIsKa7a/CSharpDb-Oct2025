namespace MoviesApp.Services
{
    using System.Globalization;

    using Data;
    using Interfaces;
    using Models;
    using ViewModels.Movies;

    using Microsoft.EntityFrameworkCore;

    public class WatchlistService : IWatchlistService
    {
        private readonly MoviesAppDbContext dbContext;

        public WatchlistService(MoviesAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> AddMovieToWatchlistAsync(int movieId)
        {
            bool movieExists = await this.dbContext
                .Movies
                .AnyAsync(m => m.Id == movieId);
            if (!movieExists)
            {
                return false;
            }

            bool movieInWatchlistExists = await this.dbContext
                .Watchlists
                .AnyAsync(w => w.MovieId == movieId);
            if (movieInWatchlistExists)
            {
                return false;
            }

            Watchlist newWatchlistEntry = new Watchlist
            {
                MovieId = movieId
            };

            await this.dbContext.Watchlists.AddAsync(newWatchlistEntry);
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<AllMoviesIndexViewModel>> GetAllMoviesInWatchlistAsync()
        {
            IEnumerable<AllMoviesIndexViewModel> allMoviesInWatchlists = await this.dbContext
                .Watchlists
                .Include(w => w.Movie)
                .AsNoTracking()
                .Select(w => w.Movie)
                .Select(m => new AllMoviesIndexViewModel()
                {
                    Id = m.Id,
                    Title = m.Title,
                    Genre = m.Genre,
                    ReleaseDate = m.ReleaseDate
                        .ToString(CultureInfo.CurrentCulture),
                    Director = m.Director,
                    Duration = m.Duration,
                    Description = m.Description,
                    ImageUrl = m.ImageUrl,
                })
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

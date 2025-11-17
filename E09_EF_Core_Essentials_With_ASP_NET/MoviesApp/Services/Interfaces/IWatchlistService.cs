namespace MoviesApp.Services.Interfaces
{
    using Models;
    using ViewModels.Movies;

    public interface IWatchlistService
    {
        Task<IEnumerable<AllMoviesIndexViewModel>> GetAllMoviesInWatchlistAsync();

        Task<bool> AddMovieToWatchlistAsync(int movieId);

        Task RemoveAsync(int movieId);

        Task<bool> MovieExistsInWatchlistAsync(int movieId);
    }
}

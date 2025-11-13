namespace MoviesApp.Services.Interfaces
{
    using Models;

    public interface IWatchlistService
    {
        Task<IEnumerable<Movie>> GetAllAsync();

        Task AddAsync(int movieId);

        Task RemoveAsync(int movieId);

        Task<bool> MovieExistsInWatchlistAsync(int movieId);
    }
}

namespace MoviesApp.Services.Interfaces
{
    using Models;

    public interface IMoviesService
    {
        Task<IEnumerable<Movie>> GetAllAsync();

        Task<Movie?> GetByIdAsync(int id);

        Task AddAsync(Movie movie);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}

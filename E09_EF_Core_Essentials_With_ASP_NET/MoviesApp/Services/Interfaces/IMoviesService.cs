namespace MoviesApp.Services.Interfaces
{
    using Models;
    using ViewModels.Movies;

    public interface IMoviesService
    {
        Task<IEnumerable<AllMoviesIndexViewModel>> GetAllMoviesForListingAsync();

        Task<MovieDetailsViewModel?> GetMovieDetailsByIdAsync(int id);

        Task<AllMoviesIndexViewModel?> GetMoviePrepareDeleteViewModelByIdAsync(int id);

        Task CreateAsync(AddMovieFormModel inputModel);

        Task<bool> DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}

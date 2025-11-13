namespace MoviesApp.Controllers
{
    using System.Globalization;

    using Microsoft.AspNetCore.Mvc;

    using Models;
    using Services.Interfaces;
    using ViewModels.Movies;

    public class MoviesController : Controller
    {
        private const string DefaultImageUrl =
            "https://img.freepik.com/free-vector/cinema-film-production-realistic-transparent-composition-with-isolated-image-clapper-with-empty-fields-vector-illustration_1284-66163.jpg?semt=ais_incoming&w=740&q=80";
        
        private readonly IMoviesService moviesService;

        public MoviesController(IMoviesService moviesService)
        {
            this.moviesService = moviesService;
        }

        public async Task<IActionResult> Index()
        {
            // Data direction is Export -> from DB to UI -> no need of data validation
            IEnumerable<Movie> allMovies = await this.moviesService
                .GetAllAsync();
            IEnumerable<AllMoviesIndexViewModel> viewModels = allMovies
                .Select(m => new AllMoviesIndexViewModel()
                {
                    Id = m.Id,
                    Title = m.Title,
                    Genre = m.Genre,
                    ReleaseDate = m.ReleaseDate.ToString(DateTimeFormatInfo.CurrentInfo),
                    Director = m.Director,
                    Duration = m.Duration,
                    Description = m.Description,
                    ImageUrl = m.ImageUrl
                });

            return View(viewModels);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new AddMovieFormModel
            {
                ReleaseDate = DateTime.Today
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddMovieFormModel model)
        {
            // Data direction is Import -> from UI to DB -> we need to validate the data
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // We have already performed all the validation using attributes!!!
            Movie newMovie = new Movie()
            {
                Title = model.Title,
                Genre = model.Genre,
                ReleaseDate = DateOnly.FromDateTime(model.ReleaseDate),
                Director = model.Director,
                Duration = model.Duration,
                Description = model.Description,
                ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? 
                    DefaultImageUrl : model.ImageUrl
            };
            await this.moviesService.AddAsync(newMovie);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            Movie? movie = await this.moviesService.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            MovieDetailsViewModel viewModel = new MovieDetailsViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Genre = movie.Genre,
                Director = movie.Director,
                Duration = movie.Duration,
                ReleaseDate = movie.ReleaseDate.ToDateTime(TimeOnly.MinValue),
                Description = movie.Description,
                ImageUrl = movie.ImageUrl
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Movie? movie = await this.moviesService.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            AllMoviesIndexViewModel viewModel = new AllMoviesIndexViewModel()
            {
                Id = movie.Id,
                Title = movie.Title,
                Genre = movie.Genre,
                ReleaseDate = movie.ReleaseDate.ToString(DateTimeFormatInfo.CurrentInfo),
                Director = movie.Director,
                Duration = movie.Duration,
                Description = movie.Description,
                ImageUrl = movie.ImageUrl
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool movieExists = await this.moviesService.ExistsAsync(id);
            if (movieExists)
            {
                await this.moviesService.DeleteAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

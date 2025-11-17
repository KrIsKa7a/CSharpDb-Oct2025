namespace MoviesApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    
    using Services.Interfaces;
    using ViewModels.Movies;

    public class MoviesController : Controller
    {
        private readonly IMoviesService moviesService;

        public MoviesController(IMoviesService moviesService)
        {
            this.moviesService = moviesService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<AllMoviesIndexViewModel> viewModels = await this.moviesService
                .GetAllMoviesForListingAsync();

            return View(viewModels);
        }

        [HttpGet]
        public IActionResult Create()
        {
            AddMovieFormModel model = new AddMovieFormModel
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

            await this.moviesService.CreateAsync(model);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            MovieDetailsViewModel? viewModel = await this.moviesService
                .GetMovieDetailsByIdAsync(id);
            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            AllMoviesIndexViewModel? viewModel = await this.moviesService
                .GetMoviePrepareDeleteViewModelByIdAsync(id);
            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool movieDeleted = await this.moviesService.DeleteAsync(id);
            if (!movieDeleted)
            {
                ModelState.AddModelError(string.Empty, "There was unexpected error while deleting entity! Try again later!");
                return RedirectToAction(nameof(Delete), new { id });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

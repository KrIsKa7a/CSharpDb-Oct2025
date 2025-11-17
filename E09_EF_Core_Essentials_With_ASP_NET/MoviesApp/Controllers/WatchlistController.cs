namespace MoviesApp.Controllers
{
    using Services.Interfaces;
    using ViewModels.Movies;

    using Microsoft.AspNetCore.Mvc;

    public class WatchlistController : Controller
    {
        private readonly IMoviesService moviesService;
        private readonly IWatchlistService watchlistService;

        public WatchlistController(IWatchlistService watchlistService, 
            IMoviesService moviesService)
        {
            this.watchlistService = watchlistService;
            this.moviesService = moviesService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<AllMoviesIndexViewModel> movieViewModels = await this.watchlistService
                .GetAllMoviesInWatchlistAsync();

            return View(movieViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int id)
        {
            bool movieExists = await this.moviesService.ExistsAsync(id);
            if (!movieExists)
            {
                return NotFound();
            }

            bool movieAddedToWatchlist = await this.watchlistService
                .AddMovieToWatchlistAsync(id);
            if (movieAddedToWatchlist)
            {
                return RedirectToAction("Index", "Watchlist");
            }

            return RedirectToAction("Index", "Movies");
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            await this.watchlistService.RemoveAsync(id);
            
            return RedirectToAction(nameof(Index));
        }
    }
}

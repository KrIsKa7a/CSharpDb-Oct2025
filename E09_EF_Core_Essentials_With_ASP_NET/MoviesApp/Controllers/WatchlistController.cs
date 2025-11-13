namespace MoviesApp.Controllers
{
    using System.Globalization;
    using Microsoft.AspNetCore.Mvc;

    using Models;
    using Services.Interfaces;
    using ViewModels.Movies;

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
            IEnumerable<Movie> movies = await this.watchlistService
                .GetAllAsync();
            IEnumerable<AllMoviesIndexViewModel> movieViewModels = movies
                .Select(m => new AllMoviesIndexViewModel
                {
                    Id = m.Id,
                    Title = m.Title,
                    Genre = m.Genre,
                    ReleaseDate = m.ReleaseDate.ToString(CultureInfo.CurrentCulture),
                    Director = m.Director,
                    Duration = m.Duration,
                    Description = m.Description,
                    ImageUrl = m.ImageUrl,
                });

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

            bool movieInWatchlistExists = await this.watchlistService
                .MovieExistsInWatchlistAsync(id);
            if (!movieInWatchlistExists)
            {
                await this.watchlistService.AddAsync(id);

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

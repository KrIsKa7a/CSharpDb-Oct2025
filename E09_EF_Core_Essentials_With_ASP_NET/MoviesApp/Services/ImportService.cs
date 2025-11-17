namespace MoviesApp.Services
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    using Data;
    using DTOs.Json;
    using Interfaces;
    using Models;

    using Newtonsoft.Json;

    public class ImportService : IImportService
    {
        private readonly MoviesAppDbContext dbContext;

        public ImportService(MoviesAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<int> ImportFromJsonAsync(string fileName)
        {
            string jsonFileContent = this.ReadDatasetFileContents(fileName);

            ICollection<Movie> moviesToImport = new List<Movie>();
            IEnumerable<ImportJsonMovieDto>? importedMovieDtos = JsonConvert
                .DeserializeObject<ImportJsonMovieDto[]>(jsonFileContent);
            if (importedMovieDtos != null)
            {
                foreach (ImportJsonMovieDto movieDto in importedMovieDtos)
                {
                    if (!this.IsValid(movieDto))
                    {
                        continue;
                    }

                    bool isReleaseDateValid = DateOnly
                        .TryParseExact(movieDto.ReleaseDate, "yyyy-MM-dd",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly releaseDate);
                    if (!isReleaseDateValid)
                    {
                        continue;
                    }

                    if (this.dbContext.Movies.Any(m => m.Title == movieDto.Title))
                    {
                        /* Based on assumption that the Movie Title is unique */
                        /* This will prevent double import even if the application is restarted */
                        continue;
                    }

                    Movie newMovie = new Movie()
                    {
                        Title = movieDto.Title,
                        Genre = movieDto.Genre,
                        ReleaseDate = releaseDate,
                        Director = movieDto.Director,
                        Duration = movieDto.Duration,
                        Description = movieDto.Description,
                        ImageUrl = movieDto.ImageUrl
                    };
                    moviesToImport.Add(newMovie);
                }

                await this.dbContext.Movies.AddRangeAsync(moviesToImport);
                await this.dbContext.SaveChangesAsync();
            }

            return moviesToImport.Count;
        }

        public async Task<int> ImportFromXmlAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        private string ReadDatasetFileContents(string fileName)
        {
            string fileDirPath = Path
                .Combine(Directory.GetCurrentDirectory(), "./Datasets/");
            string fileText = File
                .ReadAllText(fileDirPath + fileName);

            return fileText;
        }

        private bool IsValid(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);
            ICollection<ValidationResult> validationResults
                = new List<ValidationResult>();

            return Validator
                .TryValidateObject(obj, validationContext, validationResults);
        }
    }
}

namespace MoviesApp.Services.Interfaces
{
    public interface IImportService
    {
        /// <summary>
        /// Imports movies from a JSON file and returns the number of successfully imported movies.
        /// </summary>
        Task<int> ImportFromJsonAsync(string fileName);

        /// <summary>
        /// Imports movies from an XML file and returns the number of successfully imported movies.
        /// </summary>
        Task<int> ImportFromXmlAsync(string fileName);
    }
}

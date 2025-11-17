namespace MoviesApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using Services.Interfaces;
    using ViewModels.Import;

    public class ImportController : Controller
    {
        private readonly IImportService importService;

        public ImportController(IImportService importService)
        {
            this.importService = importService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new ImportIndexViewModel();

            if (TempData.ContainsKey("JsonImportedCount"))
            {
                model.JsonImportedCount = (int)TempData["JsonImportedCount"]!;
            }

            if (TempData.ContainsKey("XmlImportedCount"))
            {
                model.XmlImportedCount = (int)TempData["XmlImportedCount"]!;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ImportJson()
        {
            const string jsonDataSetFileName = "moviesJSONFile.json";
            int importedCount = await this.importService
                .ImportFromJsonAsync(jsonDataSetFileName);

            TempData["JsonImportedCount"] = importedCount;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ImportXml()
        {
            const string xmlDataSetFileName = "MoviesXMLFile.xml";
            int importedCount = await this.importService
                .ImportFromXmlAsync(xmlDataSetFileName);

            TempData["XmlImportedCount"] = importedCount;

            return RedirectToAction(nameof(Index));
        }
    }
}

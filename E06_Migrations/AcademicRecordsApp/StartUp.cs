namespace AcademicRecordsApp
{
    using Data;

    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            /* Often used during development before initial application release */
            if (Environment.GetEnvironmentVariables().Contains("DEV"))
            {
                /* Never use with PROD database */
                AcademicRecordsDbContext dbContext = new AcademicRecordsDbContext();
                dbContext.Database.Migrate();
            }
        }
    }
}

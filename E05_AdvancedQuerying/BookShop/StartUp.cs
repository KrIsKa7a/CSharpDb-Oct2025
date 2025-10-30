namespace BookShop
{
    using System.Diagnostics;
    using System.Text;

    using Microsoft.EntityFrameworkCore;
    using Z.EntityFramework.Plus;

    using Data;
    using Initializer;
    using Models;
    using Models.Enums;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //string? command = Console.ReadLine();
            //if (command != null)
            //{
            //    //string result = GetAuthorNamesEndingIn(db, command);
            //    Console.WriteLine(result);
            //}

            Stopwatch sw = Stopwatch.StartNew();
            IncreasePrices_EF_8(db);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        // Problem 02 -> Shows that Enumerations are optimized for search in DB
        // Used as a parameterized query -> EF Cache is working properly
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder sb = new StringBuilder();

            bool isCommandValid = Enum
                .TryParse(command, true, out AgeRestriction ageRestriction);
            if (isCommandValid)
            {
                IEnumerable<string> bookTitles = context
                    .Books
                    .AsNoTracking()
                    .Where(b => b.AgeRestriction == ageRestriction)
                    .OrderBy(b => b.Title)
                    .Select(b => b.Title)
                    .ToArray();

                foreach (string title in bookTitles)
                {
                    sb.AppendLine(title);
                }
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 06 -> Shows that searching inside collections is NOT optimized for DB
        // Used as non-parameterized query -> EF Cache is NOT working properly
        // Note: Fixed as of EF 9.0
        // Alt: Use pre-compiled query
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            string[] categoriesArr = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLowerInvariant())
                .ToArray();
            if (categoriesArr.Any())
            {
                IEnumerable<string> bookTitles = context
                    .Books
                    .AsNoTracking()
                    .Where(b => b.BookCategories
                        .Select(bc => bc.Category)
                        .Any(c => categoriesArr.Contains(c.Name.ToLower())))
                    .OrderBy(b => b.Title)
                    .Select(b => b.Title)
                    .ToArray();

                foreach (string title in bookTitles)
                {
                    sb.AppendLine(title);
                }
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 08 -> Shows that .StartsWith() and .EndsWith() are optimized for use in DB
        // EF Core generates parameterized queries for these methods
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var authorsNames = context
                .Authors
                .AsNoTracking()
                .Where(a => a.FirstName != null &&
                            a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName,
                })
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .ToArray();
            foreach (var author in authorsNames)
            {
                sb.AppendLine($"{author.FirstName} {author.LastName}");
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 12 -> Shows using of aggregation functions in DB queries is optimal
        // EF Core translates aggregation functions to SQL properly, but we should be careful
        // Keep in mind to use .AsSplitQuery() if required
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            
            var authorCopies = context
                .Authors
                .AsNoTracking()
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName,
                    TotalCopies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.TotalCopies)
                .ToArray();

            foreach (var author in authorCopies)
            {
                sb.AppendLine($"{author.FirstName} {author.LastName} - {author.TotalCopies}");
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 13 -> Shows that aggregate functions with navigation properties are optimized for DB
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            
            var categories = context
                .Categories
                .Include(c => c.CategoryBooks)
                .ThenInclude(cb => cb.Book)
                .AsNoTracking()
                .Select(c => new
                {
                    c.Name,
                    TotalProfit = c.CategoryBooks
                        .Select(cb => cb.Book)
                        .Sum(b => b.Price * b.Copies)
                })
                .OrderByDescending(c => c.TotalProfit)
                .ThenBy(c => c.Name)
                .ToArray();
            foreach (var category in categories)
            {
                sb.AppendLine($"{category.Name} ${category.TotalProfit:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 15
        // Older versions of EF Core < 8.0 can't handle buck update/delete
        // They are performing multiple atomic updates/deletes -> inefficient query
        public static void IncreasePrices(BookShopContext context)
        {
            IQueryable<Book> booksToUpdate = context
                .Books
                .Where(b => b.ReleaseDate != null &&
                            b.ReleaseDate.Value.Year < 2010);
            foreach (Book book in booksToUpdate)
            {
                book.Price += 5;
            }
            
            context.SaveChanges();
        }

        // For older EF Core versions < 8.0, we can use EntityFrameworkCorePlus NuGet packet for bulk operations
        // Third party NuGet packet for EF 6.0 -> Z.EntityFramework.Plus.EFCore
        public static void IncreasePrices_Bulk(BookShopContext context)
        {
            context
                .Books
                .Where(b => b.ReleaseDate != null &&
                            b.ReleaseDate.Value.Year < 2010)
                .Update(b => new Book
                {
                    Price = b.Price + 5
                });
        }

        // EF Core 8.0+ has built-in support for bulk updates
        public static void IncreasePrices_EF_8(BookShopContext context)
        {
            context
                .Books
                .Where(b => b.ReleaseDate != null &&
                            b.ReleaseDate.Value.Year < 2010)
                .ExecuteUpdate(e => e.SetProperty(b => b.Price, b => b.Price + 5));
        }
    }
}



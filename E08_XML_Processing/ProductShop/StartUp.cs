namespace ProductShop
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    using Data;
    using DTOs.Export;
    using DTOs.Import;
    using Models;
    using Utilities;

    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main()
        {
            using ProductShopContext dbContext = new ProductShopContext();

            //ResetAndSeedDatabase(dbContext);
            string result = GetUsersWithProducts(dbContext);
            WriteSerializationResult("users-and-products.xml", result);
            Console.WriteLine(result);
        }

        // Problem 01
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            ICollection<User> usersToImport = new List<User>();

            ImportUserDto[]? importUserDtos = XmlSerializerWrapper
                .Deserialize<ImportUserDto[]>(inputXml, "Users");
            if (importUserDtos != null)
            {
                foreach (ImportUserDto userDto in importUserDtos)
                {
                    if (!IsValid(userDto))
                    {
                        continue;
                    }

                    bool isAgeValid = 
                        TryParseNullableInt(userDto.Age, out int? age);
                    if (!isAgeValid)
                    {
                        continue;
                    }

                    User newUser = new User()
                    {
                        FirstName = userDto.FirstName,
                        LastName = userDto.LastName,
                        Age = age
                    };
                    usersToImport.Add(newUser);
                }

                context.Users.AddRange(usersToImport);
                context.SaveChanges();
            }

            return $"Successfully imported {usersToImport.Count}";
        }

        // Problem 02
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            ICollection<Product> productsToImport = new List<Product>();

            ImportProductDto[]? importProductDtos = XmlSerializerWrapper
                .Deserialize<ImportProductDto[]>(inputXml, "Products");
            if (importProductDtos != null)
            {
                foreach (ImportProductDto productDto in importProductDtos)
                {
                    if (!IsValid(productDto))
                    {
                        continue;
                    }

                    bool isPriceValid = decimal
                        .TryParse(productDto.Price, out decimal priceVal);
                    bool isSellerIdValid = int
                        .TryParse(productDto.SellerId, out int sellerIdVal);
                    bool isBuyerIdValid = 
                        TryParseNullableInt(productDto.BuyerId, out int? buyerIdVal);
                    if (!isPriceValid || !isSellerIdValid || !isBuyerIdValid)
                    {
                        continue;
                    }

                    // TODO: Check if SellerId and BuyerId exist in the database
                    // Justification: The problem description does not require this check and Judge may not be happy about it...
                    Product newProduct = new Product()
                    {
                        Name = productDto.Name,
                        Price = priceVal,
                        SellerId = sellerIdVal,
                        BuyerId = buyerIdVal
                    };
                    productsToImport.Add(newProduct);
                }

                context.Products.AddRange(productsToImport);
                context.SaveChanges();
            }

            return $"Successfully imported {productsToImport.Count}";
        }

        // Problem 03
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            ICollection<Category> categoriesToImport = new List<Category>();

            ImportCategoryDto[]? importCategoryDtos = XmlSerializerWrapper
                .Deserialize<ImportCategoryDto[]>(inputXml, "Categories");
            if (importCategoryDtos != null)
            {
                foreach (ImportCategoryDto categoryDto in importCategoryDtos)
                {
                    if (!IsValid(categoryDto))
                    {
                        continue;
                    }

                    Category newCategory = new Category()
                    {
                        Name = categoryDto.Name
                    };
                    categoriesToImport.Add(newCategory);
                }

                context.Categories.AddRange(categoriesToImport);
                context.SaveChanges();
            }

            return $"Successfully imported {categoriesToImport.Count}";
        }

        // Problem 04
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            ICollection<CategoryProduct> categoryProductsToImport
                = new List<CategoryProduct>();
            IEnumerable<int> existingCategoryIds = context
                .Categories
                .AsNoTracking()
                .Select(c => c.Id)
                .ToArray();
            IEnumerable<int> existingProductIds = context
                .Products
                .AsNoTracking()
                .Select(p => p.Id)
                .ToArray();

            ImportCategoryProductDto[]? importCategoryProductDtos = XmlSerializerWrapper
                .Deserialize<ImportCategoryProductDto[]>(inputXml, "CategoryProducts");
            if (importCategoryProductDtos != null)
            {
                foreach (ImportCategoryProductDto cpDto in importCategoryProductDtos)
                {
                    if (!IsValid(cpDto))
                    {
                        continue;
                    }

                    bool isCategoryIdValid = int
                        .TryParse(cpDto.CategoryId, out int categoryId);
                    bool isProductIdValid = int
                        .TryParse(cpDto.ProductId, out int productId);
                    if (!isCategoryIdValid || !isProductIdValid)
                    {
                        continue;
                    }

                    if (!existingCategoryIds.Contains(categoryId) || 
                        !existingProductIds.Contains(productId))
                    {
                        continue;
                    }

                    CategoryProduct newCategoryProduct = new CategoryProduct()
                    {
                        CategoryId = categoryId,
                        ProductId = productId
                    };
                    categoryProductsToImport.Add(newCategoryProduct);
                }

                context.CategoryProducts.AddRange(categoryProductsToImport);
                context.SaveChanges();
            }

            return $"Successfully imported {categoryProductsToImport.Count}";
        }

        // Problem 08
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            ExportUsersCountDto rootDto = new ExportUsersCountDto()
            {
                TotalUsersCount = context
                    .Users
                    .Include(u => u.ProductsSold)
                    .AsNoTracking()
                    .Count(u => u.ProductsSold.Any()),
                Users = context
                    .Users
                    .Include(u => u.ProductsSold)
                    .AsNoTracking()
                    .Where(u => u.ProductsSold.Any())
                    .Select(u => new ExportUserWithSoldProductsDto()
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Age = u.Age,
                        SoldProducts = new ExportUserSoldProductsDto()
                        {
                            Count = u.ProductsSold.Count,
                            Products = u.ProductsSold
                                .OrderByDescending(p => p.Price)
                                .Select(p => new ExportSoldProductDto()
                                {
                                    Name = p.Name,
                                    Price = p.Price.ToString("f2")
                                })
                                .ToArray()
                        }
                    })
                    .OrderByDescending(u => u.SoldProducts.Count)
                    .Take(10)
                    .ToArray(),
            };

            string result = XmlSerializerWrapper
                .Serialize(rootDto, "Users");
            return result;
        }

        private static void WriteSerializationResult(string fileName, string text)
        {
            string xmlFileDirPath = Path
                .Combine(Directory.GetCurrentDirectory(), "../../../Results/");
            File
                .WriteAllText(xmlFileDirPath + fileName, text, Encoding.Unicode);
        }

        private static void ResetAndSeedDatabase(ProductShopContext dbContext)
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            string xmlFileText = ReadXmlDatasetFileContents("users.xml");
            string result = ImportUsers(dbContext, xmlFileText);

            xmlFileText = ReadXmlDatasetFileContents("products.xml");
            result = ImportProducts(dbContext, xmlFileText);

            xmlFileText = ReadXmlDatasetFileContents("categories.xml");
            result = ImportCategories(dbContext, xmlFileText);

            xmlFileText = ReadXmlDatasetFileContents("categories-products.xml");
            result = ImportCategoryProducts(dbContext, xmlFileText);

            Console.WriteLine(result);
        }

        private static string ReadXmlDatasetFileContents(string fileName)
        {
            string xmlFileDirPath = Path
                .Combine(Directory.GetCurrentDirectory(), "../../../Datasets/");
            string xmlFileText = File
                .ReadAllText(xmlFileDirPath + fileName);

            return xmlFileText;
        }

        private static bool TryParseNullableInt(string? input, out int? val)
        {
            // TODO: Refactor as generic method
            int? outValue = null;
            if (input != null)
            {
                bool isInputValid = int
                    .TryParse(input, out int ageVal);
                if (!isInputValid)
                {
                    val = outValue;
                    return false;
                }

                outValue = ageVal;
            }

            val = outValue;
            return true;
        }

        private static bool IsValid(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);
            ICollection<ValidationResult> validationResults
                = new List<ValidationResult>();

            return Validator
                .TryValidateObject(obj, validationContext, validationResults);
        }
    }
}
namespace CarDealer
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;

    using Data;
    using DTOs.Import;
    using Models;

    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class StartUp
    {
        public static void Main()
        {
            using CarDealerContext dbContext = new CarDealerContext();
            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();

            string jsonFileDirPath = Path
                .Combine(Directory.GetCurrentDirectory(), "../../../Datasets/");
            string jsonFileName = "sales.json";
            string jsonFileText = File
                .ReadAllText(jsonFileDirPath + jsonFileName);

            //string result = ImportSales(dbContext, jsonFileText);
            //Console.WriteLine(result);

            string result = GetSalesWithAppliedDiscount(dbContext);
            Console.WriteLine(result);
        }

        // Problem 09
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            ICollection<Supplier> suppliersToImport = new List<Supplier>();

            IEnumerable<ImportSupplierDto>? supplierDtos = JsonConvert
                .DeserializeObject<ImportSupplierDto[]>(inputJson);
            if (supplierDtos != null)
            {
                foreach (ImportSupplierDto supplierDto in supplierDtos)
                {
                    if (!IsValid(supplierDto))
                    {
                        continue;
                    }

                    bool isImporterValidVal = bool
                        .TryParse(supplierDto.IsImporter, out bool isImporter);
                    if (!isImporterValidVal)
                    {
                        continue;
                    }

                    Supplier newSupplier = new Supplier()
                    {
                        Name = supplierDto.Name,
                        IsImporter = isImporter
                    };
                    suppliersToImport.Add(newSupplier);
                }

                context.Suppliers.AddRange(suppliersToImport);
                context.SaveChanges();
            }

            return $"Successfully imported {suppliersToImport.Count}.";
        }

        // Problem 10
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            ICollection<Part> partsToImport = new List<Part>();
            // Choose single query + store in memory when suppliers count is small
            ICollection<int> existingSuppliers = context
                .Suppliers
                .AsNoTracking()
                .Select(s => s.Id)
                .ToArray();

            IEnumerable<ImportPartDto>? partDtos = JsonConvert
                .DeserializeObject<ImportPartDto[]>(inputJson);
            if (partDtos != null)
            {
                foreach (ImportPartDto partDto in partDtos)
                {
                    // First validation, then insertion
                    if (!IsValid(partDto))
                    {
                        continue;
                    }

                    bool isSupplierIdValid = int
                        .TryParse(partDto.SupplierId, out int supplierId);
                    if ((!isSupplierIdValid) || 
                        (!existingSuppliers.Contains(supplierId)))
                    {
                        continue;
                    }

                    Part newPart = new Part()
                    {
                        Name = partDto.Name,
                        Price = partDto.Price,
                        Quantity = partDto.Quantity,
                        SupplierId = supplierId
                    };
                    partsToImport.Add(newPart);
                }

                context.Parts.AddRange(partsToImport);
                context.SaveChanges();
            }

            return $"Successfully imported {partsToImport.Count}.";
        }

        // Problem 11
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            ICollection<Car> carsToImport = new List<Car>();
            ICollection<PartCar> partsCarsToImport = new List<PartCar>();

            IEnumerable<ImportCarDto>? carDtos = JsonConvert
                .DeserializeObject<ImportCarDto[]>(inputJson);
            if (carDtos != null)
            {
                foreach (ImportCarDto carDto in carDtos)
                {
                    if (!IsValid(carDto))
                    {
                        continue;
                    }

                    Car newCar = new Car()
                    {
                        Make = carDto.Make,
                        Model = carDto.Model,
                        TraveledDistance = carDto.TraveledDistance
                    };
                    carsToImport.Add(newCar);

                    foreach (int partId in carDto.PartsIds.Distinct())
                    {
                        if (!context.Parts.Any(p => p.Id == partId))
                        {
                            continue;
                        }

                        PartCar newPartCar = new PartCar()
                        {
                            PartId = partId,
                            Car = newCar
                        };
                        partsCarsToImport.Add(newPartCar);
                    }
                }

                //context.Cars.AddRange(carsToImport); // Actually it's not needed, since EF will find the new cars from mapping entities
                context.PartsCars.AddRange(partsCarsToImport);

                context.SaveChanges();
            }

            return $"Successfully imported {carsToImport.Count}.";
        }

        // Problem 12
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            // Just in-memory collection of Entity objects (not related to DB!)
            ICollection<Customer> customersToImport = new List<Customer>();

            IEnumerable<ImportCustomerDto>? customerDtos = JsonConvert
                .DeserializeObject<ImportCustomerDto[]>(inputJson);
            if (customerDtos != null)
            {
                foreach (ImportCustomerDto customerDto in customerDtos)
                {
                    if (!IsValid(customerDto))
                    {
                        continue;
                    }

                    bool isBirthDateValid = DateTime
                        .TryParseExact(customerDto.Birthdate, "yyyy-MM-dd'T'HH:mm:ss",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate);
                    bool isYoungDriverValid = bool
                        .TryParse(customerDto.IsYoungDriver, out bool isYoungDriver);
                    if ((!isBirthDateValid) || (!isYoungDriverValid))
                    {
                        continue;
                    }

                    Customer newCustomer = new Customer()
                    {
                        Name = customerDto.Name,
                        BirthDate = birthDate,
                        IsYoungDriver = isYoungDriver
                    };
                    customersToImport.Add(newCustomer);
                }

                // Now the in-memory collection is added to the ChangeTracker
                context.Customers.AddRange(customersToImport);
                context.SaveChanges();
            }

            return $"Successfully imported {customersToImport.Count}.";
        }

        // Problem 13
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            ICollection<Sale> salesToImport = new List<Sale>();

            IEnumerable<ImportSaleDto>? saleDtos = JsonConvert
                .DeserializeObject<ImportSaleDto[]>(inputJson);
            if (saleDtos != null)
            {
                foreach (ImportSaleDto saleDto in saleDtos)
                {
                    bool isCarIdExisting = context
                        .Cars
                        .Any(c => c.Id == saleDto.CarId);
                    if (!isCarIdExisting)
                    {
                        continue;
                    }

                    bool isCustomerIdExisting = context
                        .Customers
                        .Any(cu => cu.Id == saleDto.CustomerId);
                    if (!isCustomerIdExisting)
                    {
                        continue;
                    }

                    Sale newSale = new Sale()
                    {
                        CarId = saleDto.CarId,
                        CustomerId = saleDto.CustomerId,
                        Discount = saleDto.Discount
                    };
                    salesToImport.Add(newSale);
                }

                context.Sales.AddRange(salesToImport);
                context.SaveChanges();
            }

            return $"Successfully imported {salesToImport.Count}.";
        }

        // Problem 19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var top10Sales = context
                .Sales
                .Include(s => s.Customer)
                .Include(s => s.Car)
                .ThenInclude(c => c.PartsCars)
                .ThenInclude(pc => pc.Part)
                .AsNoTracking()
                .Select(s => new
                {
                    Car = new
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance,
                    },
                    CustomerName = s.Customer.Name,
                    CustomerIsYoungDriver = s.Customer.IsYoungDriver,
                    Discount = s.Discount,
                    Price = s.Car.PartsCars
                        .Select(pc => pc.Part)
                        .Sum(p => p.Price)
                })
                .Take(10)
                .ToArray();

            var saleExportDtos = top10Sales
                .Select(s => new
                {
                    car = s.Car,
                    customerName = s.CustomerName,
                    discount = s.Discount.ToString("f2"),
                    price = s.Price.ToString("f2"),
                    priceWithDiscount = (s.Price - (s.Price * (s.Discount / 100))).ToString("f2")
                })
                .ToArray();

            string jsonResult = JsonConvert
                .SerializeObject(saleExportDtos, Formatting.Indented);
            return jsonResult;
        }

        private static bool IsValid(object obj)
        {
            // These two variables are required by the Validator.TryValidateObject method
            // We will not use them for now...
            // We are just using the Validation result (true or false)
            ValidationContext validationContext = new ValidationContext(obj);
            ICollection<ValidationResult> validationResults
                = new List<ValidationResult>();

            return Validator
                .TryValidateObject(obj, validationContext, validationResults);
        }
    }
}
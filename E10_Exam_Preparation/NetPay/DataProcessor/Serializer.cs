namespace NetPay.DataProcessor
{
    using Data;
    using Data.Models.Enums;
    using ExportDtos;
    using Utilities;

    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportHouseholdsWhichHaveExpensesToPay(NetPayContext context)
        {
            const string xmlRootName = "Households";
            ExportHouseholdDto[] householdUnpaidExpenses = context
                .Households
                .AsNoTracking()
                .Include(h => h.Expenses)
                .ThenInclude(e => e.Service)
                .Where(h => h.Expenses.Any(e => e.PaymentStatus != PaymentStatus.Paid))
                .OrderBy(h => h.ContactPerson)
                .ToArray()
                .Select(h => new ExportHouseholdDto()
                {
                    ContactPerson = h.ContactPerson,
                    Email = h.Email,
                    PhoneNumber = h.PhoneNumber,
                    Expenses = h.Expenses
                        .Where(e => e.PaymentStatus != PaymentStatus.Paid)
                        .Select(e => new ExportHouseholdUnpaidExpenseDto()
                        {
                            ExpenseName = e.ExpenseName,
                            Amount = e.Amount.ToString("F2"),
                            PaymentDate = e.DueDate.ToString("yyyy-MM-dd"),
                            ServiceName = e.Service.ServiceName
                        })
                        .OrderBy(e => e.PaymentDate)
                        .ThenBy(e => e.Amount)
                        .ToArray(),
                })
                .ToArray();

            string xmlResult = XmlSerializerWrapper
                .Serialize(householdUnpaidExpenses, xmlRootName);

            return xmlResult;
        }

        public static string ExportAllServicesWithSuppliers(NetPayContext context)
        {
            var servicesWithSuppliers = context
                .Services
                .Include(s => s.SuppliersServices)
                .ThenInclude(ss => ss.Supplier)
                .AsNoTracking()
                .Select(s => new
                {
                    s.ServiceName,
                    Suppliers = s.SuppliersServices
                        .Select(ss => ss.Supplier)
                        .OrderBy(sup => sup.SupplierName)
                        .Select(sup => new
                        {
                            sup.SupplierName,
                        })
                        .ToArray()
                })
                .OrderBy(s => s.ServiceName)
                .ToArray();

            string jsonResult = JsonConvert
                .SerializeObject(servicesWithSuppliers, Formatting.Indented);
            
            return jsonResult;
        }
    }
}

namespace SoftUni
{
    using System.Text;

    using Data;
    using Models;

    public class StartUp
    {
        static void Main(string[] args)
        {
            try
            {
                using SoftUniContext context = new SoftUniContext();

                string result = AddNewAddressToEmployee(context);

                Console.WriteLine(result);
            }
            catch (Exception e)
            {
                Console.WriteLine("## Unhandled exception occurred! ##");
                Console.WriteLine(e.Message);
            }
        }

        /* Problem 03 */
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            /*
             *  SQL Server Query...
             *  .ToArray()/.ToList() -> Materialization Point
             *  Client In-Memory Processing...
             */
            var employees = context
                .Employees
                .OrderBy(e => e.EmployeeId)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary,
                })
                .ToArray();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        /* Problem 05 */
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employeesRnD = context
                .Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    DepartmentName = e.Department.Name,
                    e.Salary,
                })
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToArray();
            
            foreach (var e in employeesRnD)
            {
                sb
                    .AppendLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary.ToString("f2")}");
            }

            return sb.ToString().TrimEnd();
        }

        /* Problem 06 */
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Employee nakovEmployee = context
                .Employees
                .First(e => e.LastName == "Nakov");

            /* Create new row in Addresses and set it to Nakov Employee row */
            /* These changes are still locally in ChangeTracker of Employee, Address */
            Address newAddress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4, /* Must be ensured that a valid FK value is passed */
            };
            nakovEmployee.Address = newAddress;

            /* Persist local changes in the server's DB */
            context.SaveChanges();

            string[] employeesAddresses = context
                .Employees
                .OrderByDescending(e => e.AddressId)
                .Select(e => e.Address.AddressText)
                .Take(10)
                .ToArray();

            return string.Join(Environment.NewLine, employeesAddresses);
        }
    }
}

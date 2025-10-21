namespace SoftUni.Models
{
    public class Address
    {
        public int AddressId { get; set; }

        public string AddressText { get; set; } = null!;

        /* This is the property corresponding to FK TownID */
        public int? TownId { get; set; }

        /* This is the navigation property corresponding to FK TownID */
        /* virtual keyword is used to enable 'Lazy' loading of data -> memory reduction */
        /* This can lead to SQL network overhead */
        public virtual Town? Town { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
            = new List<Employee>();
    }
}
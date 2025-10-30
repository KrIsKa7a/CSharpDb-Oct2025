namespace BookShop.Models
{
    using System.Collections.Generic;

    public class Category
    {
        public int CategoryId { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<BookCategory> CategoryBooks { get; set; }
            = new HashSet<BookCategory>();
    }
}
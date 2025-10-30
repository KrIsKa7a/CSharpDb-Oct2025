namespace BookShop.Models
{
    using System.Collections.Generic;

    public class Author
    {
        public int AuthorId { get; set; }

        public string? FirstName { get; set; }

        public string LastName { get; set; } = null!;

        public virtual ICollection<Book> Books { get; set; }
            = new HashSet<Book>();
    }
}
namespace BookShop.Models
{
    using System;
    using System.Collections.Generic;

    using Enums;

    public class Book
    {
        public int BookId { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public EditionType EditionType { get; set; }

        public decimal Price { get; set; }

        public int Copies { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public AgeRestriction AgeRestriction { get; set; }

        public int AuthorId { get; set; }

        public virtual Author Author { get; set; } = null!;

        public virtual ICollection<BookCategory> BookCategories { get; set; }
            = new HashSet<BookCategory>();
    }
}

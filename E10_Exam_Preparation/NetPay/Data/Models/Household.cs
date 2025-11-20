namespace NetPay.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidation.Household;

    public class Household
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ContactPersonMaxLength)]
        public string ContactPerson { get; set; } = null!;

        [MaxLength(EmailMaxLength)]
        public string? Email { get; set; }

        [Required]
        [MaxLength(PhoneNumberLength)]
        public string PhoneNumber { get; set; } = null!;

        public virtual ICollection<Expense> Expenses { get; set; }
            = new HashSet<Expense>();
    }
}

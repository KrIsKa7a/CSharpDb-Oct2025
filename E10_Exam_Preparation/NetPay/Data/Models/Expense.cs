namespace NetPay.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Enums;
    using static Common.EntityValidation.Expense;

    public class Expense
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ExpenseNameMaxLength)]
        public string ExpenseName { get; set; } = null!;

        [Column(TypeName = AmountSqlType)]
        public decimal Amount { get; set; }

        public DateTime DueDate { get; set; }

        // Enum will be stored in the DB as INT
        public PaymentStatus PaymentStatus { get; set; }

        [ForeignKey(nameof(Household))]
        public int HouseholdId { get; set; }

        public virtual Household Household { get; set; } = null!;

        [ForeignKey(nameof(Service))]
        public int ServiceId { get; set; }

        public virtual Service Service { get; set; } = null!;
    }
}

namespace NetPay.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;

    using Newtonsoft.Json;

    using static Common.EntityValidation.Expense;

    public class ImportExpenseDto
    {
        [JsonProperty("ExpenseName")]
        [Required]
        [MinLength(ExpenseNameMinLength)]
        [MaxLength(ExpenseNameMaxLength)]
        public string ExpenseName { get; set; } = null!;

        [JsonProperty("Amount")]
        [Required]
        [Range(typeof(decimal), AmountMinValue, AmountMaxValue)]
        public decimal? Amount { get; set; }

        [JsonProperty("DueDate")]
        [Required]
        public string DueDate { get; set; } = null!;

        [JsonProperty("PaymentStatus")]
        [Required]
        public string PaymentStatus { get; set; } = null!;

        [JsonProperty("HouseholdId")]
        [Required]
        public int? HouseholdId { get; set; }

        [JsonProperty("ServiceId")]
        [Required]
        public int? ServiceId { get; set; }
    }
}

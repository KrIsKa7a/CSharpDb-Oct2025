namespace CarDealer.DTOs.Import
{
    using System.ComponentModel.DataAnnotations;

    using Newtonsoft.Json;

    public class ImportPartDto
    {
        [Required]
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        // We should avoid direct usage of decimal without TryParse()
        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        // We are using string, because SupplierId could be invalid
        [Required]
        [JsonProperty("supplierId")]
        public string SupplierId { get; set; } = null!;
    }
}

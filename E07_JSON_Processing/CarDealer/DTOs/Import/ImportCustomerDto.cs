namespace CarDealer.DTOs.Import
{
    using System.ComponentModel.DataAnnotations;

    using Newtonsoft.Json;

    public class ImportCustomerDto
    {
        [Required]
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [Required]
        [JsonProperty("birthDate")]
        public string Birthdate { get; set; } = null!;

        [Required]
        [JsonProperty("isYoungDriver")]
        public string IsYoungDriver { get; set; } = null!;
    }
}

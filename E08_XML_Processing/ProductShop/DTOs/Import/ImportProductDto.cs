namespace ProductShop.DTOs.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Product")]
    public class ImportProductDto
    {
        [Required]
        [XmlElement("name")]
        public string Name { get; set; } = null!;

        [Required]
        [XmlElement("price")]
        public string Price { get; set; } = null!;

        [Required]
        [XmlElement("sellerId")]
        public string SellerId { get; set; } = null!;

        [XmlElement("buyerId")]
        public string? BuyerId { get; set; }
    }
}

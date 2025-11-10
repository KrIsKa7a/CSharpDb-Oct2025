namespace ProductShop.DTOs.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("CategoryProduct")]
    public class ImportCategoryProductDto
    {
        [Required]
        [XmlElement("CategoryId")]
        public string CategoryId { get; set; } = null!;

        [Required]
        [XmlElement("ProductId")]
        public string ProductId { get; set; } = null!;
    }
}

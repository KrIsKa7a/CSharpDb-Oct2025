namespace ProductShop.DTOs.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("User")]
    public class ImportUserDto
    {
        [XmlElement("firstName")]
        public string? FirstName { get; set; }

        [Required]
        [XmlElement("lastName")]
        public string LastName { get; set; } = null!;

        [XmlElement("age")]
        public string? Age { get; set; }
    }
}

namespace ProductShop.DTOs.Export
{
    using System.Xml.Serialization;

    [XmlRoot("Users")]
    public class ExportUsersCountDto
    {
        [XmlElement("count")]
        public int TotalUsersCount { get; set; }

        [XmlArray("users")]
        public ExportUserWithSoldProductsDto[] Users { get; set; } = null!;
    }
}

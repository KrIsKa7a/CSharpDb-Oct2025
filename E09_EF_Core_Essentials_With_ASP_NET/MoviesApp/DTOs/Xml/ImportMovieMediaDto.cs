namespace MoviesApp.DTOs.Xml
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    using static Common.EntityValidation;

    [XmlType("Media")]
    public class ImportMovieMediaDto
    {
        [XmlElement("ImageUrl")]
        [MaxLength(MovieImageUrlMaxLength)]
        public string? ImageUrl { get; set; }
    }
}

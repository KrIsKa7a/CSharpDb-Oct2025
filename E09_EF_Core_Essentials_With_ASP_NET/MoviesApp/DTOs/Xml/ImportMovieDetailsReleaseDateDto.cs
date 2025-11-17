namespace MoviesApp.DTOs.Xml
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    using static Common.EntityValidation;

    [XmlType("Release")]
    public class ImportMovieDetailsReleaseDateDto
    {
        [XmlAttribute("date")]
        [Required]
        [RegularExpression(MovieReleaseDateRegExprPattern)]
        public string Date { get; set; } = null!;
    }
}

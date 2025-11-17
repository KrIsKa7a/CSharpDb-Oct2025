namespace MoviesApp.DTOs.Xml
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    using static Common.EntityValidation;

    [XmlType("Rating")]
    public class ImportMovieRatingDto
    {
        [XmlAttribute("source")]
        [MaxLength(MovieRatingSourceMaxLength)]
        public string? RatingSource { get; set; }
    }
}

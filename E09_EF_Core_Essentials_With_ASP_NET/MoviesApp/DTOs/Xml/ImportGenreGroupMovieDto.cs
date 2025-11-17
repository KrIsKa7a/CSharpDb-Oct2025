namespace MoviesApp.DTOs.Xml
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    using static Common.EntityValidation;

    [XmlType("Movie")]
    public class ImportGenreGroupMovieDto
    {
        [XmlAttribute("duration")]
        [Required]
        [MaxLength(MovieDurationTextMaxLength)]
        public string Duration { get; set; } = null!;

        [XmlElement("Title")]
        [Required]
        [MinLength(MovieTitleMinLength)]
        [MaxLength(MovieTitleMaxLength)]
        public string Title { get; set; } = null!;

        [XmlElement("Details")]
        public ImportMovieDetailsDto Details { get; set; } = null!;

        [XmlElement("Description")]
        [Required]
        [MinLength(MovieDescriptionMinLength)]
        [MaxLength(MovieDescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [XmlElement("Media")]
        public ImportMovieMediaDto? Media { get; set; }

        [XmlElement("Rating")]
        public ImportMovieRatingDto? Rating { get; set; }
    }
}

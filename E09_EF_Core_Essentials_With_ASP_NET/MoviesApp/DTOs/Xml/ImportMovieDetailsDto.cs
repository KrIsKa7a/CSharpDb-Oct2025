namespace MoviesApp.DTOs.Xml
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    using static Common.EntityValidation;

    [XmlType("Details")]
    public class ImportMovieDetailsDto
    {
        [XmlElement("Genre")]
        [Required]
        [MinLength(MovieGenreMinLength)]
        [MaxLength(MovieGenreMaxLength)]
        public string Genre { get; set; } = null!;

        [XmlElement("Director")]
        [Required]
        [MinLength(MovieDirectorMinLength)]
        [MaxLength(MovieDirectorMaxLength)]
        public string Director { get; set; } = null!;

        [XmlElement("Release")]
        public ImportMovieDetailsReleaseDateDto Release { get; set; } = null!;
    }
}

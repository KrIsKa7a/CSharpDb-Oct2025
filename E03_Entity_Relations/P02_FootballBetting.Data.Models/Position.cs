namespace P02_FootballBetting.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidation.Position;

    public class Position
    {
        [Key]
        public int PositionId { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Player> Players { get; set; }
            = new HashSet<Player>();
    }
}

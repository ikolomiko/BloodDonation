using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodDonation.Types.Entity
{
    [Table("Hospitals")]
    public class Hospital
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(75)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Address { get; set; }

        [StringLength(15)]
        public string? Phone { get; set; }
    }
}

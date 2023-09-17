using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodDonation.Types.Entity
{
    [Table("NeedForBlood")]
    public class NeedForBlood
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int HospitalId { get; set; }

        [Required]
        public byte BloodGroupId { get; set; }

        [Required]
        public int AmountNeeded { get; set; }

        [NotMapped]
        public BloodGroup BloodGroup
        {
            get { return (BloodGroup)BloodGroupId; }
            set { BloodGroupId = (byte)value; }
        }
    }
}

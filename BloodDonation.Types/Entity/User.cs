using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodDonation.Types.Entity
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public byte UserTypeId { get; set; } = 0;

        [Required]
        [StringLength (50)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public byte BloodGroupId { get; set; } = 0;

        [StringLength (50)]
        public string? FirstName { get; set; }

        [StringLength (50)]
        public string? LastName { get; set; }

        public int? HospitalId { get; set; }

        [NotMapped]
        public UserType UserType
        {
            get { return (UserType)UserTypeId; }
            set { UserTypeId = (byte)value; }
        }

        [NotMapped]
        public BloodGroup BloodGroup
        {
            get { return (BloodGroup)BloodGroupId; }
            set { BloodGroupId = (byte)value; }
        }
    }
}

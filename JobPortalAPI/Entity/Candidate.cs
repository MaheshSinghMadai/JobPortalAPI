using System.ComponentModel.DataAnnotations;

namespace JobPortalAPI.Entity
{
    public class Candidate
    {
        [Key]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }
        
        public string PhoneNumber { get; set; }

        [Required]
        public string LastName { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string LinkedInProfileUrl { get; set; }
        public string GithubProfileUrl { get; set; }

        [Required]
        public string Comment { get; set; }

    }
}

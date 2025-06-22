//using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EventEaseManagementSystem.Models
{
    public class VenueView
    {
        public int VenueId { get; set; }
        [Required]
        public string VenueName { get; set; } = null!;

        [Required]
        public string Location { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
        public int Capacity { get; set; }
        
        public string? ImageUrl { get; set; }

        [Display(Name = "Upload Picture")]
        public IFormFile? UploadImage { get; set; } // <-- used in form upload
    }
}

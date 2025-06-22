using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventEaseManagementSystem.Models
{
    public class EventView
    {
        public int EventId { get; set; }

        [Required]
        public string EventName { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateOnly EventDate { get; set; }

        public string Description { get; set; } = null!;

        [Required]
        [Display(Name = "Venue")]
        public int VenueId { get; set; }

        public string? ImageUrl { get; set; } // for viewing existing image

        [Display(Name = "Upload Event Image")]
        public IFormFile? UploadImage { get; set; } // for uploading

        // Optional: for dropdown list display
        public IEnumerable<SelectListItem>? VenueList { get; set; }
    }
}

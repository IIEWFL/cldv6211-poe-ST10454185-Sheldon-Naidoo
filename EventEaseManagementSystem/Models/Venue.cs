using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace EventEaseManagementSystem.Models;

public partial class Venue
{
    public int VenueId { get; set; }

    [Required(ErrorMessage = "Venue Name is required")]
    public string VenueName { get; set; } = null!;
    [Required(ErrorMessage = "Location is required")]
    public string Location { get; set; } = null!;

    [Required(ErrorMessage = "Capacity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
    public int Capacity { get; set; }

    [Required(ErrorMessage = "Venue Image Picture is required")]
    public string ImageUrl { get; set; } = null!;

    public bool IsAvailable { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}

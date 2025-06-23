using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventEaseManagementSystem.Models;

public partial class BookingView
{
    public int BookingId { get; set; }

    public DateOnly BookingDate { get; set; }

    public int VenueId { get; set; }

    public string VenueName { get; set; } = null!;

    public string VenueLocation { get; set; } = null!;

    public int Capacity { get; set; }

    public string Image { get; set; } = null!;

    public bool IsAvailable { get; set; }

    public int EventId { get; set; }

    public string EventName { get; set; } = null!;

    public DateOnly EventDate { get; set; }

    public string Details { get; set; } = null!;

    public string? EventType { get; set; }
    public int? EventTypeId { get; set; }
    
}

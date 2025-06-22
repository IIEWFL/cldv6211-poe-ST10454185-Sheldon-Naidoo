using System;
using System.Collections.Generic;

namespace EventEaseManagementSystem.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? VenueId { get; set; }

    public int? EventId { get; set; }

    public DateOnly BookingDate { get; set; }

    public virtual Event? Event { get; set; }

    public virtual Venue? Venue { get; set; }
}

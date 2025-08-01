﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EventEaseManagementSystem.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string EventName { get; set; } = null!;

    public DateOnly EventDate { get; set; }

    public string Description { get; set; } = null!;

    public int? VenueId { get; set; }

    public int? EventTypeId { get; set; }

    public virtual EventType? EventType { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Venue? Venue { get; set; }
}

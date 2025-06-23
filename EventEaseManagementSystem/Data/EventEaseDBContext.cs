using System;
using System.Collections.Generic;
using EventEaseManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventEaseManagementSystem.Data;

public partial class EventEaseDBContext : DbContext
{
    public EventEaseDBContext()
    {
    }

    public EventEaseDBContext(DbContextOptions<EventEaseDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingView> BookingViews { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Venue> Venues { get; set; }

    public virtual DbSet<EventType> EventTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__73951ACD896D0D81");

            entity.ToTable("Booking");

            entity.HasIndex(e => new { e.VenueId, e.EventId, e.BookingDate }, "UniqueBooking").IsUnique();

            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.VenueId).HasColumnName("VenueID");

            entity.HasOne(d => d.Event).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Booking_Event");

            entity.HasOne(d => d.Venue).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.VenueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Booking_Venue");
        });

        modelBuilder.Entity<BookingView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("BookingView");

            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.Details)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.EventName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Image).IsUnicode(false);
            entity.Property(e => e.VenueId).HasColumnName("VenueID");
            entity.Property(e => e.VenueLocation)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.VenueName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.EventTypeId).HasColumnName("EventTypeID");
            entity.Property(e => e.EventType)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Event__7944C87006A11589");

            entity.ToTable("Event");

            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.EventName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.VenueId).HasColumnName("VenueID");

            entity.Property(e => e.EventTypeId).HasColumnName("EventTypeID");

            entity.HasOne(d => d.Venue).WithMany(p => p.Events)
                .HasForeignKey(d => d.VenueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Event_Venue");

            entity.HasOne(d => d.EventType)
                .WithMany(et => et.Events)
                .HasForeignKey(d => d.EventTypeId)
                .OnDelete(DeleteBehavior.SetNull) // You can change this behavior
                .HasConstraintName("FK_Event_EventType");
        });

        modelBuilder.Entity<EventType>(entity =>
        {
            entity.HasKey(e => e.EventTypeId);
            entity.Property(e => e.EventTypeId).HasColumnName("EventTypeID");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(e => e.VenueId).HasName("PK__Venue__3C57E5D2B348A4AD");

            entity.ToTable("Venue");

            entity.HasIndex(e => e.VenueName, "UQ__Venue__A40F8D12023FA950").IsUnique();

            entity.Property(e => e.VenueId).HasColumnName("VenueID");
            entity.Property(e => e.ImageUrl)
                .IsUnicode(false)
                .HasColumnName("ImageURL");
            entity.Property(e => e.Location)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.VenueName)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEaseManagementSystem.Data; 
using EventEaseManagementSystem.Models;
using Azure.Storage.Sas;
using Azure.Storage;
using Microsoft.AspNetCore.Mvc.Rendering;
using static NuGet.Packaging.PackagingConstants;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EventEaseManagementSystem.Controllers
{
    public class BookingViewController : Controller
    {
        private readonly EventEaseDBContext _context;

        // Add Azure Blob Storage configuration
        private readonly string storageAccountName;
        private readonly string storageAccountKey;
        private readonly string containerName;

        public BookingViewController(EventEaseDBContext context, IConfiguration config)
        {
            _context = context;
            storageAccountName = config["AzureBlob:storageAccountName"] ?? throw new ArgumentNullException("AzureBlob:storageAccountName");
            storageAccountKey = config["AzureBlob:storageAccountKey"] ?? throw new ArgumentNullException("AzureBlob:storageAccountKey");
            containerName = config["AzureBlob:containerName"] ?? "venue-pictures";
        }
        private string GenerateSasUrl(string blobName)
        {
            if (string.IsNullOrEmpty(blobName))
                return "";

            var blobUri = new Uri($"https://{storageAccountName}.blob.core.windows.net/{containerName}/{blobName}");
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(30)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(storageAccountName, storageAccountKey)).ToString();
            return $"{blobUri}?{sasToken}";
        }

        // GET: BookingView
        public async Task<IActionResult> Index(string searchQuery, int? eventTypeId, DateTime? startDate, DateTime? endDate, bool? isAvailable)
        {
            var bookingsQuery = _context.BookingViews.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                bookingsQuery = bookingsQuery.Where(b =>
                    b.BookingId.ToString().Contains(searchQuery) ||
                    b.BookingDate.ToString().Contains(searchQuery) ||
                    b.VenueName.ToLower().Contains(searchQuery) ||
                    b.VenueLocation.ToLower().Contains(searchQuery) ||
                    b.EventName.ToLower().Contains(searchQuery) ||
                    b.Details.ToLower().Contains(searchQuery));
            }

            if (eventTypeId.HasValue)
            {
                bookingsQuery = bookingsQuery.Where(b => b.EventTypeId == eventTypeId);
            }

            // 3.Apply Date Range filters
            if (startDate.HasValue)
            {
                // Filter where BookingDate is on or after startDate
                bookingsQuery = bookingsQuery.Where(b => b.BookingDate >= DateOnly.FromDateTime(startDate.Value));
            }

            if (endDate.HasValue)
            {
                // Filter where BookingDate is on or before endDate
                bookingsQuery = bookingsQuery.Where(b => b.BookingDate <= DateOnly.FromDateTime(endDate.Value));
            }


            if (isAvailable.HasValue)
            {
                bookingsQuery = bookingsQuery.Where(b => b.IsAvailable == isAvailable.Value);
            }

            var bookings = await bookingsQuery.ToListAsync();

            // Attach SAS URLs for Images
            foreach (var booking in bookings)
            {
                // Handle potential null Image from DB correctly before passing to GenerateSasUri
                // This ensures 'booking.Image' is always a string (empty if null from DB)
                booking.Image = booking.Image ?? ""; // If dbBooking.Image is null, set to ""

                // Only call GenerateSasUri if there's an actual image path
                if (!string.IsNullOrEmpty(booking.Image))
                {
                    // Assuming GenerateSasUri returns the SAS URL string and you want to assign it back
                    booking.Image = GenerateSasUrl(booking.Image);
                }
                // If booking.Image was null or empty, it will remain an empty string from the ?? "" operation.
            }

            //ViewData["EventTypes"] = new SelectList(_context.EventTypes.ToList(), "EventTypeId", "Name");

            // Populate ViewData for the Event Type dropdown filter
            // Order by Name for better display
            ViewData["EventTypes"] = new SelectList(_context.EventTypes.OrderBy(et => et.Name), "EventTypeId", "Name", eventTypeId);

            // Pass current filter values back to the view to pre-fill the form inputs
            ViewData["CurrentSearchQuery"] = searchQuery;
            ViewData["CurrentEventTypeId"] = eventTypeId; // Keep for dropdown selection
            ViewData["CurrentStartDate"] = startDate?.ToString("yyyy-MM-dd"); // Format for HTML date input
            ViewData["CurrentEndDate"] = endDate?.ToString("yyyy-MM-dd");     // Format for HTML date input
            ViewData["CurrentIsAvailable"] = isAvailable; // For checkbox state



            return View(bookings);
        }

        // GET: BookingView Details
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.BookingViews
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            booking.Image = string.IsNullOrEmpty(booking.Image)
                ? ""
                : GenerateSasUrl(booking.Image);

            return View(booking);
        }
    }
}


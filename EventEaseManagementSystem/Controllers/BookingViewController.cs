using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEaseManagementSystem.Data; 
using EventEaseManagementSystem.Models;
using Azure.Storage.Sas;
using Azure.Storage;

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
        public async Task<IActionResult> Index(string searchQuery)
        {
            var bookingsList = await _context.BookingViews.ToListAsync();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();

                bookingsList = bookingsList.Where(B =>
                B.BookingId.ToString().Contains(searchQuery) ||
                B.VenueId.ToString().Contains(searchQuery) ||
                B.VenueName.ToLower().Contains(searchQuery) ||
                B.VenueLocation.ToLower().Contains(searchQuery) ||
                B.EventId.ToString().Contains(searchQuery) ||
                B.EventName.ToLower().Contains(searchQuery) ||
                B.Details.ToLower().Contains(searchQuery)).ToList();
            }

            // Fetch the data from the database
            //var bookings = await _context.BookingViews.ToListAsync();

            // Generate SAS URLs for each image
            foreach (var booking in bookingsList)
            {
                booking.Image = string.IsNullOrEmpty(booking.Image)
                    ? ""
                    : GenerateSasUrl(booking.Image);
            }

            return View(bookingsList);
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


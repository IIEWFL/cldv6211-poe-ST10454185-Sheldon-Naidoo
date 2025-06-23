using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EventEaseManagementSystem.Data;
using EventEaseManagementSystem.Models;

namespace EventEaseManagementSystem.Controllers
{
    public class VenueController : Controller
    {
        private readonly EventEaseDBContext _context;

        // Integrate azure blob storage

        private readonly string storageAccountName;
        private readonly string storageAccountKey;
        private readonly string containerName;

        public VenueController(EventEaseDBContext context, IConfiguration config)
        {
            _context = context;
            storageAccountName = config["AzureBlob:storageAccountName"] ?? throw new ArgumentNullException("AzureBlob:storageAccountName");
            storageAccountKey = config["AzureBlob:storageAccountKey"] ?? throw new ArgumentNullException("AzureBlob:storageAccountKey");
            containerName = config["AzureBlob:containerName"] ?? "venue-pictures";
        }

        // Set up Blob container client
        private BlobContainerClient GetContainerClient()
        {
            var serviceUri = new Uri($"https://{storageAccountName}.blob.core.windows.net");
            var serviceClient = new BlobServiceClient(serviceUri, new StorageSharedKeyCredential(storageAccountName, storageAccountKey));
            return serviceClient.GetBlobContainerClient(containerName);
        }

        // Set up image upload method
        private async Task<string> UploadImageToBlobAsync(IFormFile file)
        {
            // Check if there is a valid image
            if (file == null || file.Length == 0) return null;
               // throw new ArgumentException("No file provided for upload.");

            var containerClient = GetContainerClient();
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            // Assign the image a unique ID and name

            var blobName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var blobClient = containerClient.GetBlobClient(blobName);

            //Send image to container using blobClient

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

            return blobName;
        }
        
        // Set up our shared access signture token method
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
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(15)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(storageAccountName, storageAccountKey)).ToString();
            return $"{blobUri}?{sasToken}";
        }

        // GET: Lecturers List
        [HttpGet]
        public async Task<IActionResult> Index(string searchQuery)
        {
            var Venue = _context.Venues.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                Venue = Venue.Where(v => 
                v.VenueId.ToString().Contains(searchQuery) ||
                v.VenueName.Contains(searchQuery) ||
                v.Location.Contains(searchQuery));
            }
            
            // Fetch the data from the database
            var venuesList = await Venue.ToListAsync();
            var venueWithUrls = venuesList.Select(v => new VenueView
            {
                VenueId = v.VenueId,
                VenueName = v.VenueName,
                Location = v.Location,
                Capacity = v.Capacity,
                IsAvailable = v.IsAvailable,
                ImageUrl = string.IsNullOrEmpty(v.ImageUrl) ? "" : GenerateSasUrl(v.ImageUrl)
            }).ToList();

            ViewData["CurrentSearchQuery"] = searchQuery;

            return View(venueWithUrls);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);

            if (venue == null)
            {
                return NotFound();
            }

            venue.ImageUrl = GenerateSasUrl(venue.ImageUrl);

            return View(venue);
        }

        // Create a new venue
        // GET: Create venue form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create a new venue
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VenueView model)
        {
            if (model.UploadImage == null || model.UploadImage.Length == 0)
            {
                ModelState.AddModelError("UploadImage", "Please upload a valid image.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var imageUrl = await UploadImageToBlobAsync(model.UploadImage);  // Fix: Pass the correct IFormFile

                var venue = new Venue
                {
                    VenueName = model.VenueName,
                    Location = model.Location,
                    Capacity = model.Capacity,
                    ImageUrl = imageUrl,
                    IsAvailable = model.IsAvailable
                };

                _context.Add(venue);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Venue {venue.VenueName} was added successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Image upload failed: {ex.Message}");
                return View(model);
            }

            /*
            _context.Add(venue);
            await _context.SaveChangesAsync();

            // Set temporary message
            TempData["SuccessMessage"] = $"Venue {venue.VenueName} was added successfully!";

            return RedirectToAction(nameof(Index));
            */

        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null)
                return NotFound();

            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName,Location,Capacity,ImageUrl,IsAvailable")] Venue venue, IFormFile? VenuePicture)
        {
            if (id != venue.VenueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var venueToUpdate = await _context.Venues.AsNoTracking().FirstOrDefaultAsync(v => v.VenueId == id);
                    if (venueToUpdate == null)
                    {
                        return NotFound();
                    }

                    string? oldBlobName = venueToUpdate.ImageUrl;
                    string? newBlobName = null;

                    if (VenuePicture != null && VenuePicture.Length > 0)
                    {
                        // 1. Upload the new image FIRST
                        newBlobName = await UploadImageToBlobAsync(VenuePicture);

                        // 2. Update the venue's ImageUrl with the new blob name
                        venue.ImageUrl = newBlobName; 
                    }

                    venueToUpdate.VenueName = venue.VenueName;
                    venueToUpdate.Location = venue.Location;
                    venueToUpdate.Capacity = venue.Capacity;
                    venueToUpdate.IsAvailable = venue.IsAvailable;
                    venueToUpdate.ImageUrl = venue.ImageUrl;

                    _context.Update(venueToUpdate);

                    if (!string.IsNullOrEmpty(oldBlobName) && oldBlobName != newBlobName)
                    {
                        try
                        {
                            var containerClient = GetContainerClient();
                            var blobClient = containerClient.GetBlobClient(oldBlobName);
                            await blobClient.DeleteIfExistsAsync();
                        }
                        catch (Exception ex)
                        {
                            // Log the error but don't prevent the venue update from saving
                            Console.WriteLine($"Error deleting old blob '{oldBlobName}': {ex.Message}");
                            // Optionally, add a TempData error or use ILogger
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                catch (Exception ex) // Catch-all for other potential errors, e.g., during upload
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                    // Log the full exception details here
                    return View(venue);
                }

                // Set temporary message
                TempData["UpdateMessage"] = $"Venue {venue.VenueName} was updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(v => v.VenueId == id);
            if (venue == null)
                return NotFound();

            venue.ImageUrl = GenerateSasUrl(venue.ImageUrl);

            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                // Delete image blob
                try
                {
                    var containerClient = GetContainerClient();
                    var blobClient = containerClient.GetBlobClient(venue.ImageUrl);
                    await blobClient.DeleteIfExistsAsync();
                }
                catch
                {
                    // Log error if needed
                }

                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
            }

            // Set temporary message
            TempData["DeleteMessage"] = $"Venue {venue.VenueName} was deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(v => v.VenueId == id);
        }
    }
}

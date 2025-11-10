using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MunicipalServicesMVC.Models;
using MunicipalServicesMVC.Services;

namespace MunicipalServicesMVC.Controllers
{
    public class IssuesController : Controller
    {
        private readonly IssueStore _store;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<IssuesController> _logger;

        public IssuesController(
            IssueStore store,
            IWebHostEnvironment env,
            ILogger<IssuesController> logger)
        {
            _store = store;
            _env = env;
            _logger = logger;
        }

        // GET: /Issues/Create
        [HttpGet]
        public IActionResult Create()
        {
            // Fresh, empty view model – Category and Priority will be null
            var vm = new IssueCreateVm();
            return View(vm);
        }

        // POST: /Issues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IssueCreateVm vm)
        {
            if (!ModelState.IsValid)
            {
                // Light encouragement message for the user
                TempData["Engagement"] = "Please fix the highlighted fields and try again.";
                return View(vm);
            }

            string? virtualPath = null;

            // Handle optional attachment
            if (vm.Attachment != null && vm.Attachment.Length > 0)
            {
                try
                {
                    var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsRoot);

                    var originalName = Path.GetFileNameWithoutExtension(vm.Attachment.FileName);
                    var ext = Path.GetExtension(vm.Attachment.FileName);
                    var safeName = string.IsNullOrWhiteSpace(originalName)
                        ? "attachment"
                        : originalName.Replace(" ", "_");

                    var fileName = $"{safeName}_{Guid.NewGuid():N}{ext}";
                    var fullPath = Path.Combine(uploadsRoot, fileName);

                    using (var stream = System.IO.File.Create(fullPath))
                    {
                        await vm.Attachment.CopyToAsync(stream);
                    }

                    // This is what we store on the Issue so it can be served from wwwroot
                    virtualPath = $"/uploads/{fileName}";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving attachment");
                    // We don’t fail the whole request, just log it
                }
            }

            // Generate the short 8-character reference ID that the user sees & searches with
            var referenceId = Guid.NewGuid()
                                  .ToString("N")
                                  .Substring(0, 8)
                                  .ToUpperInvariant();

            // Map from view model to domain model
            var issue = new Issue
            {
                Id = referenceId,
                Location = vm.Location,
                Category = vm.Category ?? IssueCategory.Other, // fallback if somehow null
                Description = vm.Description,
                AttachmentVirtualPath = virtualPath,
                CreatedAt = DateTime.UtcNow,
                Status = IssueStatus.Pending,                  // default when created
                Priority = vm.Priority ?? 3                    // fallback to normal priority
            };

            _store.Add(issue);

            // Small friendly message for the next page
            TempData["Engagement"] = "Thank you for reporting this issue.";

            // Redirect to thank-you page with the same reference id
            return RedirectToAction(nameof(ThankYou), new { id = issue.Id });
        }

        // GET: /Issues/ThankYou/{id}
        [HttpGet]
        public IActionResult ThankYou(string id)
        {
            // The view shows this id directly (already 8 characters, upper-case)
            ViewBag.IssueId = id;
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using MunicipalServicesMVC.Models;
using MunicipalServicesMVC.Models;

namespace MunicipalServicesMVC.Controllers
{
    public class IssuesController : Controller
    {
        private readonly IssueStore _store;
        private readonly IWebHostEnvironment _env;

        public IssuesController(IssueStore store, IWebHostEnvironment env)
        {
            _store = store;
            _env = env;
        }

        // GET: /Issues/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new IssueCreateVm());
        }

        // POST: /Issues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IssueCreateVm vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Engagement"] = "Almost there! Please fix the highlighted fields 👀";
                return View(vm);
            }

            string? virtualPath = null;

            if (vm.Attachment is not null && vm.Attachment.Length > 0)
            {
                // Allow only common image/PDF types
                var allowed = new[] { ".png", ".jpg", ".jpeg", ".gif", ".pdf" };
                var ext = Path.GetExtension(vm.Attachment.FileName).ToLowerInvariant();
                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError(nameof(vm.Attachment), "Only PNG, JPG, GIF or PDF allowed.");
                    TempData["Engagement"] = "Oops! Unsupported file type. Try PNG/JPG/GIF/PDF.";
                    return View(vm);
                }

                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);

                var safeName = $"{Guid.NewGuid():N}{ext}";
                var fullPath = Path.Combine(uploads, safeName);
                using (var fs = new FileStream(fullPath, FileMode.CreateNew))
                {
                    await vm.Attachment.CopyToAsync(fs);
                }

                virtualPath = $"/uploads/{safeName}";
            }

            var issue = new Issue
            {
                Location = vm.Location.Trim(),
                Category = vm.Category!.Value,
                Description = vm.Description.Trim(),
                AttachmentVirtualPath = virtualPath
            };

            _store.Add(issue);

            TempData["Success"] = $"Your report has been logged. Ref: {issue.Id.Substring(0, 8).ToUpper()}";
            TempData["Engagement"] = "Thank you for taking action! 🌟";
            return RedirectToAction(nameof(ThankYou), new { id = issue.Id });
        }

        // GET: /Issues/ThankYou/{id}
        [HttpGet]
        public IActionResult ThankYou(string id)
        {
            ViewBag.IssueId = id;
            return View();
        }
    }
}

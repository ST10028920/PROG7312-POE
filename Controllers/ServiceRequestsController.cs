using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MunicipalServicesMVC.Models;
using MunicipalServicesMVC.Services;

namespace MunicipalServicesMVC.Controllers
{
    /// <summary>
    /// Shows and searches the status of service requests (reported issues).
    /// </summary>
    public class ServiceRequestsController : Controller
    {
        private readonly IssueStore _store;

        public ServiceRequestsController(IssueStore store)
        {
            _store = store;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var allIssues = _store.ToArray();
            var index = new ServiceRequestIndex(allIssues);
            return View(index);
        }

        [HttpPost]
        public IActionResult Search(string referenceId)
        {
            var allIssues = _store.ToArray();
            var index = new ServiceRequestIndex(allIssues);

            if (string.IsNullOrWhiteSpace(referenceId))
            {
                ViewBag.Message = "Please enter a reference ID.";
                return View("Index", index);
            }

            var trimmed = referenceId.Trim();
            var found = index.FindById(trimmed);

            if (found == null)
            {
                ViewBag.Message = $"No issue found with reference ID {trimmed}.";
                return View("Index", index);
            }

            // Show only that one issue in the table
            ViewBag.Message = $"Result for reference ID {trimmed}:";
            var singleIndex = new ServiceRequestIndex(new List<Issue> { found });
            return View("Index", singleIndex);
        }
    }
}

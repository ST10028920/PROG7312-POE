using Microsoft.AspNetCore.Mvc;
using MunicipalServicesMVC.Services;
using System;
using System.Linq;

namespace MunicipalServicesMVC.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventCatalog _catalog;

        public EventsController(IEventCatalog catalog)
        {
            _catalog = catalog;
        }

        // Includes sorting: date_asc (default), date_desc, title_asc, category_asc
        public IActionResult Index(string? category, string? date, string? sort = "date_asc")
        {
            // Parse date safely
            DateOnly? parsed = null;
            if (!string.IsNullOrWhiteSpace(date) && DateOnly.TryParse(date, out var d))
                parsed = d;

            // Base results from the catalog (filtered by category/date)
            var results = _catalog.Search(category, parsed);

            // Key used for date-time ordering
            DateTime KeyTime(MunicipalServicesMVC.Models.Event e) =>
                e.Time.HasValue
                    ? e.Date.ToDateTime(e.Time.Value)
                    : e.Date.ToDateTime(new TimeOnly(0, 0));

            // ---- APPLY SORT (with stable secondary keys to avoid “column jumping”) ----
            results = sort switch
            {
                "date_desc" => results.OrderByDescending(KeyTime)
                                         .ThenBy(e => e.Title)
                                         .ToList(),

                "title_asc" => results.OrderBy(e => e.Title)
                                         .ThenBy(KeyTime)
                                         .ToList(),

                "category_asc" => results.OrderBy(e => e.Category)
                                         .ThenBy(KeyTime)
                                         .ThenBy(e => e.Title)
                                         .ToList(),

                _ => results.OrderBy(KeyTime)      // date_asc (default)
                                         .ThenBy(e => e.Title)
                                         .ToList(),
            };

            // Recommendations (based on last category searched + history)
            var recs = _catalog.Recommend(category);

            // Data for the view (sticky inputs)
            ViewBag.Categories = _catalog.AllEvents
                .Select(e => e.Category)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c)
                .ToList();

            ViewBag.Category = category ?? "";
            ViewBag.Date = parsed?.ToString("yyyy-MM-dd") ?? "";
            ViewBag.Sort = sort ?? "date_asc";
            ViewBag.Recommended = recs;

            return View(results);
        }
    }
}
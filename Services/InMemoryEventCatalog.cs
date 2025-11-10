using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServicesMVC.Models;

namespace MunicipalServicesMVC.Services
{
    public class InMemoryEventCatalog : IEventCatalog
    {
        private readonly List<Event> _events = new();

        // Tracks how many times each category was searched
        private readonly Dictionary<string, int> _searchCounts =
            new(StringComparer.OrdinalIgnoreCase);

        // Data structures for ordering and categories
        public SortedDictionary<DateOnly, List<Event>> EventsByDate { get; } = new();
        public HashSet<string> CategorySet { get; } = new();
        public Stack<Event> RecentlyViewed { get; } = new();

        // Pipelines for upcoming items
        public Queue<Event> UpcomingQueue { get; } = new();
        public PriorityQueue<Event, DateTime> UpcomingPriority { get; } = new();

        public InMemoryEventCatalog()
        {
            Seed();            // ⬅️ 18 realistic events
            BuildStructures(); // build dictionaries/sets/queues
        }

        public IReadOnlyCollection<Event> AllEvents => _events.AsReadOnly();

        // ---------- helpers ----------
        private static DateTime AsDateTime(Event e) =>
            e.Time.HasValue
                ? e.Date.ToDateTime(e.Time.Value)
                : e.Date.ToDateTime(new TimeOnly(0, 0));

        // ---------- search ----------
        public IEnumerable<Event> Search(string? category, DateOnly? date)
        {
            IEnumerable<Event> q = _events;

            if (!string.IsNullOrWhiteSpace(category))
            {
                var key = category.Trim();
                q = q.Where(e => e.Category.Equals(key, StringComparison.OrdinalIgnoreCase));

                // bump counter safely
                _searchCounts[key] = _searchCounts.TryGetValue(key, out var c) ? c + 1 : 1;
            }

            if (date.HasValue)
                q = q.Where(e => e.Date == date.Value);

            return q.OrderBy(AsDateTime).ToList();
        }

        // ---------- recommendations ----------
        public IEnumerable<Event> Recommend(string? lastSearchCategory)
        {
            if (!string.IsNullOrWhiteSpace(lastSearchCategory))
            {
                var key = lastSearchCategory.Trim();
                var rec = _events
                    .Where(e => e.Category.Equals(key, StringComparison.OrdinalIgnoreCase)
                                && AsDateTime(e) >= DateTime.Now)
                    .OrderBy(AsDateTime)
                    .Take(3)
                    .ToList();
                if (rec.Count > 0) return rec;
            }

            if (_searchCounts.Count > 0)
            {
                var topCategory = _searchCounts
                    .OrderByDescending(kv => kv.Value)
                    .First().Key;

                var rec = _events
                    .Where(e => e.Category.Equals(topCategory, StringComparison.OrdinalIgnoreCase)
                                && AsDateTime(e) >= DateTime.Now)
                    .OrderBy(AsDateTime)
                    .Take(3)
                    .ToList();
                if (rec.Count > 0) return rec;
            }

            return _events
                .Where(e => AsDateTime(e) >= DateTime.Now)
                .OrderBy(AsDateTime)
                .Take(3)
                .ToList();
        }

        // ---------- seed data (15+ entries) ----------
        private void Seed()
        {
            // Categories used by your badge map: Utilities, Community, Roads, Sanitation, Other

            // Utilities
            _events.Add(new Event
            {
                Title = "Water Outage Notice",
                Category = "Utilities",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
                Time = null,
                Location = "Ward 32",
                Description = "Planned maintenance between 10:00–14:00.",
                IsAnnouncement = true
            });
            _events.Add(new Event
            {
                Title = "Electricity Substation Upgrade",
                Category = "Utilities",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(6)),
                Time = new TimeOnly(11, 0),
                Location = "Pinetown Central",
                Description = "Temporary power interruptions expected during the upgrade window.",
                IsAnnouncement = true
            });
            _events.Add(new Event
            {
                Title = "Water Tanker Schedule",
                Category = "Utilities",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                Time = new TimeOnly(8, 30),
                Location = "uMlazi V Section",
                Description = "Tanker deliveries every 2 hours at the community hall.",
                IsAnnouncement = true
            });
            _events.Add(new Event
            {
                Title = "Streetlight Fault Reporting Drive",
                Category = "Utilities",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(9)),
                Time = new TimeOnly(9, 0),
                Location = "Online",
                Description = "Log faulty streetlights via the portal for faster turnaround.",
                IsAnnouncement = true
            });

            // Community
            _events.Add(new Event
            {
                Title = "Beach Cleanup",
                Category = "Community",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(4)),
                Time = new TimeOnly(9, 0),
                Location = "North Beach",
                Description = "Join the community cleanup. Bags provided.",
                IsAnnouncement = false
            });
            _events.Add(new Event
            {
                Title = "Community Safety Forum",
                Category = "Community",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
                Time = new TimeOnly(18, 30),
                Location = "Phoenix Civic Centre",
                Description = "Monthly public meeting with SAPS and councillors.",
                IsAnnouncement = false
            });
            _events.Add(new Event
            {
                Title = "Library Story Hour (Kids)",
                Category = "Community",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
                Time = new TimeOnly(10, 0),
                Location = "Glenwood Library",
                Description = "Free reading session for ages 4–8. Parents welcome.",
                IsAnnouncement = false
            });
            _events.Add(new Event
            {
                Title = "Public Wi-Fi Hotspot Launch",
                Category = "Community",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(14)),
                Time = new TimeOnly(12, 0),
                Location = "Verulam Taxi Rank",
                Description = "New hotspot live. Bring a device to test and get help connecting.",
                IsAnnouncement = true
            });

            // Roads
            _events.Add(new Event
            {
                Title = "Road Resurfacing: Umgeni Rd",
                Category = "Roads",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
                Time = null,
                Location = "Umgeni Rd (Argyle → Smiso Nkwanyana)",
                Description = "Single-lane closures; expect delays.",
                IsAnnouncement = true
            });
            _events.Add(new Event
            {
                Title = "Pothole Blitz – Volunteer Day",
                Category = "Roads",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(8)),
                Time = new TimeOnly(8, 0),
                Location = "Newlands East Depot",
                Description = "Assist teams with reporting and traffic control (training provided).",
                IsAnnouncement = false
            });
            _events.Add(new Event
            {
                Title = "Cycle Lane Awareness Ride",
                Category = "Roads",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(10)),
                Time = new TimeOnly(7, 30),
                Location = "Durban CBD",
                Description = "Family-friendly paced ride highlighting new lanes.",
                IsAnnouncement = false
            });

            // Sanitation
            _events.Add(new Event
            {
                Title = "Refuse Collection Shift",
                Category = "Sanitation",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
                Time = null,
                Location = "Westville Wards",
                Description = "Pickup moved one day later due to public holiday.",
                IsAnnouncement = true
            });
            _events.Add(new Event
            {
                Title = "Illegal Dumping Cleanup",
                Category = "Sanitation",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(11)),
                Time = new TimeOnly(8, 30),
                Location = "KwaMashu D-Section",
                Description = "Community-led cleanup with municipal trucks on site.",
                IsAnnouncement = false
            });
            _events.Add(new Event
            {
                Title = "Recycling Workshop",
                Category = "Sanitation",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(12)),
                Time = new TimeOnly(17, 30),
                Location = "Chatsworth Youth Centre",
                Description = "Learn sorting basics and drop-off points near you.",
                IsAnnouncement = false
            });

            // Other
            _events.Add(new Event
            {
                Title = "Heritage Day Concert",
                Category = "Other",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(13)),
                Time = new TimeOnly(16, 0),
                Location = "People’s Park",
                Description = "Local artists, food stalls, and cultural displays.",
                IsAnnouncement = false
            });
            _events.Add(new Event
            {
                Title = "Job Readiness Bootcamp",
                Category = "Other",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(6)),
                Time = new TimeOnly(9, 30),
                Location = "KwaDabeka Hall",
                Description = "CV help, interview skills, and free headshots.",
                IsAnnouncement = false
            });
            _events.Add(new Event
            {
                Title = "Emergency Siren Test",
                Category = "Other",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
                Time = new TimeOnly(12, 0),
                Location = "Municipality-wide",
                Description = "Short siren test across several wards. No action required.",
                IsAnnouncement = true
            });
            _events.Add(new Event
            {
                Title = "Public Transport Survey",
                Category = "Other",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(9)),
                Time = null,
                Location = "Online",
                Description = "Help improve routes and schedules. 5-minute survey.",
                IsAnnouncement = true
            });
        }

        // ---------- structure building ----------
        private void BuildStructures()
        {
            EventsByDate.Clear();
            CategorySet.Clear();
            UpcomingQueue.Clear();
            UpcomingPriority.Clear();

            foreach (var e in _events)
            {
                if (!EventsByDate.TryGetValue(e.Date, out var list))
                {
                    list = new List<Event>();
                    EventsByDate[e.Date] = list;
                }
                list.Add(e);

                if (!string.IsNullOrWhiteSpace(e.Category))
                    CategorySet.Add(e.Category);

                var dt = AsDateTime(e);
                if (dt >= DateTime.Now)
                {
                    UpcomingQueue.Enqueue(e);
                    UpcomingPriority.Enqueue(e, dt);
                }
            }
        }
    }
}

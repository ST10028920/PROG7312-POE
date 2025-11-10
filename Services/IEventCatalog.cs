using System;
using System.Collections.Generic;
using MunicipalServicesMVC.Models;

namespace MunicipalServicesMVC.Services
{
    public interface IEventCatalog
    {
        IReadOnlyCollection<Event> AllEvents { get; }

        // Search (category + date)
        IEnumerable<Event> Search(string? category, DateOnly? date);

        // Recommendations (we'll wire later)
        IEnumerable<Event> Recommend(string? lastSearchCategory);
    }
}

using System;

namespace MunicipalServicesMVC.Models
{
    public class Event
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = "";
        public string Category { get; set; } = "";
        public DateOnly Date { get; set; }
        public TimeOnly? Time { get; set; }
        public string Location { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsAnnouncement { get; set; } = false;
    }
}

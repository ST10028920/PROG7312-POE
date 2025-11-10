namespace MunicipalServicesMVC.Models.Chatbot
{
    public class IssueDraft
    {
        public string SuggestedRef { get; set; } = $"REF-{DateTime.UtcNow:yyyyMMddHHmmss}";
        public string Category { get; set; } = "";
        public string Location { get; set; } = "";
        public string Description { get; set; } = "";
        public string? PhotoPath { get; set; }
    }

    public class IssueStatus
    {
        public string Reference { get; set; } = "";
        public string State { get; set; } = "Pending";
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Notes { get; set; } = "Queued for assessment.";
    }
}

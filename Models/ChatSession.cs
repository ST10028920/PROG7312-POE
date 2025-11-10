namespace MunicipalServicesMVC.Models.Chatbot
{
    public enum ReportState { Idle, AskCategory, AskLocation, AskDescription, AskPhoto, Confirm }

    public class ChatSession
    {
        public ReportState ReportState { get; set; } = ReportState.Idle;
        public string? PendingCategory { get; set; }
        public string? PendingLocation { get; set; }
        public string? PendingDescription { get; set; }
        public string? PendingPhotoPath { get; set; }
    }
}

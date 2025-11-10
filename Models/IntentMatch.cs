namespace MunicipalServicesMVC.Models.Chatbot
{
    public class IntentMatch
    {
        public Intent Intent { get; set; }
        public Dictionary<string, string> Entities { get; set; } = new();
    }
}

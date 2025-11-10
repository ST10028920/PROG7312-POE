namespace MunicipalServicesMVC.Services.Chatbot
{
    public sealed class IssueService
    {
        public string HowToReport() =>
            "To report a problem, go to **Issues** and fill in Location, Category and Description. " +
            "Use the **Attach Media** button if you have a photo. " +
            "When ready, click **Submit** ✅";
    }
}

namespace MunicipalServicesMVC.Services.Chatbot
{
    public sealed class FaqService
    {
        public string Welcome() =>
            "Hi, I’m your municipal assistant 👋<br />" +
            "You can ask me to:<br />" +
            "• <strong>open events</strong><br />" +
            "• <strong>open issues</strong><br />" +
            "• <strong>search community on 2025-10-10</strong><br />" +
            "• <strong>sort events by date</strong><br />" +
            "• <strong>report an issue</strong>";

        public string Help() =>
            "Here’s what I can help with:<br />" +
            "• Open the <strong>events</strong> page<br />" +
            "• Open the <strong>issue reporting</strong> page<br />" +
            "• <strong>Search events</strong> by category and date<br />" +
            "• <strong>Sort events</strong> by date, title, or category<br />" +
            "Try something like: <strong>search community on 10/10/2025</strong>.";

        public string Fallback() =>
            "Sorry, I’m not sure what you mean there 🤔<br />" +
            "Try <strong>help</strong>, or something like:<br />" +
            "<strong>open events</strong>, <strong>open issues</strong>, " +
            "<strong>search community on 2025-10-10</strong>, or <strong>report an issue</strong>.";
    }
}
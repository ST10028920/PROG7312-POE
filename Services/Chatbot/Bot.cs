using System.Collections.Generic;
using System.Web;

namespace MunicipalServicesMVC.Services.Chatbot
{
    public sealed class Bot
    {
        private readonly Nlu _nlu = new();
        private readonly FaqService _faq = new();
        private readonly IssueService _issues = new();

        // Returns HTML so we can show links and bold text
        public (string replyHtml, string? navigateUrl) Respond(string userText)
        {
            var (intent, category, date, sort) = _nlu.Detect(userText);

            switch (intent)
            {
                case Intent.Greet:
                    return (_faq.Welcome(), null);

                case Intent.Help:
                    return (_faq.Help(), null);

                case Intent.Thanks:
                    return ("You’re welcome! 😊", null);

                case Intent.OpenHome:
                    return ("Taking you back to the home page…", "/");

                case Intent.OpenEvents:
                    return ("Opening the events page…", "/Events");

                case Intent.OpenIssues:
                    return ("Opening the issue reporting page…", "/Issues");

                case Intent.SortByDate:
                case Intent.SortByTitle:
                case Intent.SortByCategory:
                    {
                        var sortKey = sort ?? "date_asc";
                        var nice = sortKey switch
                        {
                            "title_asc" => "title",
                            "category_asc" => "category",
                            _ => "date"
                        };

                        var qs = BuildEventQuery(null, null, sortKey);
                        return ($"Sorting events by <strong>{nice}</strong>…", "/Events" + qs);
                    }

                case Intent.SearchEvents:
                    {
                        var qs = BuildEventQuery(category, date, sort);
                        var msg = "Searching events";

                        if (!string.IsNullOrWhiteSpace(category))
                            msg += $" in <strong>{HttpUtility.HtmlEncode(category)}</strong>";

                        if (!string.IsNullOrWhiteSpace(date))
                            msg += $" on <strong>{date}</strong>";

                        msg += "…";
                        return (msg, "/Events" + qs);
                    }

                case Intent.ReportIssue:
                    return (_issues.HowToReport(), "/Issues");

                default:
                    return (_faq.Fallback(), null);
            }
        }

        private static string BuildEventQuery(string? category, string? date, string? sort)
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(category))
                parts.Add("category=" + HttpUtility.UrlEncode(category));

            if (!string.IsNullOrWhiteSpace(date))
                parts.Add("date=" + HttpUtility.UrlEncode(date));

            if (!string.IsNullOrWhiteSpace(sort))
                parts.Add("sort=" + HttpUtility.UrlEncode(sort));

            return parts.Count == 0 ? "" : "?" + string.Join("&", parts);
        }
    }
}

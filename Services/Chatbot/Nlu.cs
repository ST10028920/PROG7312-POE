using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace MunicipalServicesMVC.Services.Chatbot
{
    public enum Intent
    {
        Greet,
        Help,
        Thanks,
        OpenHome,
        OpenEvents,
        OpenIssues,
        SearchEvents,
        SortByDate,
        SortByTitle,
        SortByCategory,
        ReportIssue,
        Unknown
    }

    public sealed class Nlu
    {
        // Returns: intent, category, date(yyyy-MM-dd), sortKey
        public (Intent intent, string? category, string? date, string? sort) Detect(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return (Intent.Help, null, null, null);

            // Normalise: remove bullets and take first line
            var normalised = text
                .Replace("•", " ")
                .Replace("●", " ")
                .Trim();

            var firstLine = normalised
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault() ?? string.Empty;

            var t = firstLine.Trim().ToLowerInvariant();

            // ---- Exact short commands first (so no funny business) ----
            if (t == "open issues" || t == "issues" || t == "issue" || t == "go to issues")
                return (Intent.OpenIssues, null, null, null);

            if (t == "open events" || t == "events" || t == "go to events")
                return (Intent.OpenEvents, null, null, null);

            // ---- Small talk ----
            if (ContainsAny(t, "hi", "hello", "hey", "good morning", "good evening"))
                return (Intent.Greet, null, null, null);

            if (t.Contains("thank"))
                return (Intent.Thanks, null, null, null);

            if (t.Contains("help") || t.Contains("what can you do"))
                return (Intent.Help, null, null, null);

            // ---- Home navigation ----
            if (ContainsAny(t, "home page", "homepage", "go home", "back home", "start page"))
                return (Intent.OpenHome, null, null, null);

            // ---- Issues / reporting ----

            // very direct “report issue” phrases
            if (ContainsAny(t,
                    "report issue",
                    "report an issue",
                    "report a issue",
                    "report a problem",
                    "log issue",
                    "log a issue",
                    "log a problem",
                    "log a fault",
                    "report pothole",
                    "report a pothole"))
            {
                return (Intent.ReportIssue, null, null, null);
            }

            // explicit “open issues” in a longer sentence
            if (ContainsAny(t,
                    "open issues",
                    "open issue",
                    "go to issues",
                    "go to issue",
                    "issues page",
                    "issue page",
                    "report problems page",
                    "view issues",
                    "view issue list"))
            {
                return (Intent.OpenIssues, null, null, null);
            }

            // mentions an issue + the word "report"
            if (ContainsAny(t, "issue", "fault", "complaint", "problem") && t.Contains("report"))
                return (Intent.ReportIssue, null, null, null);

            // generic: any message that talks about “issues” → open issues page
            if (ContainsAny(t, "issues", "issue"))
                return (Intent.OpenIssues, null, null, null);

            // ---- Events navigation / actions ----

            // open events (longer sentences)
            if (ContainsAny(t, "open events", "go to events", "events page", "view events"))
                return (Intent.OpenEvents, null, null, null);

            // sorting
            if (t.Contains("sort") && ContainsAny(t, "date", "soonest", "latest"))
                return (Intent.SortByDate, null, null, "date_asc");

            if (t.Contains("sort") && ContainsAny(t, "title", "name", "alphabetical"))
                return (Intent.SortByTitle, null, null, "title_asc");

            if (t.Contains("sort") && t.Contains("category"))
                return (Intent.SortByCategory, null, null, "category_asc");

            // ---- Event search ----
            if (t.Contains("event") || t.Contains("search"))
            {
                string? category = null;

                if (t.Contains("community"))
                    category = "Community";
                else if (t.Contains("sport") || t.Contains("sports"))
                    category = "Sports";
                else if (t.Contains("health"))
                    category = "Health";
                else if (t.Contains("safety"))
                    category = "Safety";
                else if (t.Contains("utilities") || t.Contains("water") || t.Contains("electricity"))
                    category = "Utilities";
                else if (t.Contains("other"))
                    category = "Other";

                var date = ExtractDate(t);

                return (Intent.SearchEvents, category, date, null);
            }

            // ---- Fallback ----
            return (Intent.Unknown, null, null, null);
        }

        private static bool ContainsAny(string t, params string[] terms)
            => terms.Any(term => t.Contains(term));

        private static string? ExtractDate(string t)
        {
            // yyyy-MM-dd
            var m = Regex.Match(t, @"\b\d{4}-\d{2}-\d{2}\b");
            if (m.Success && DateTime.TryParseExact(m.Value, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt.ToString("yyyy-MM-dd");

            // d/M/yyyy or dd/MM/yyyy
            m = Regex.Match(t, @"\b\d{1,2}/\d{1,2}/\d{4}\b");
            if (m.Success && DateTime.TryParse(m.Value, out dt))
                return dt.ToString("yyyy-MM-dd");

            // 10 Oct 2025 / 10 October 2025 etc.
            m = Regex.Match(t, @"\b\d{1,2}\s+[A-Za-z]{3,}\s+\d{4}\b");
            if (m.Success && DateTime.TryParse(m.Value, out dt))
                return dt.ToString("yyyy-MM-dd");

            return null;
        }
    }
}

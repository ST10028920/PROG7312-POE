using System;
using System.ComponentModel.DataAnnotations;

namespace MunicipalServicesMVC.Models;

/// <summary>
/// Categories of issues that can be reported.
/// </summary>
public enum IssueCategory
{
    Sanitation,
    Roads,
    Utilities,
    Safety,
    Other
}

/// <summary>
/// Represents a municipal service issue reported by a citizen.
/// </summary>
public sealed class Issue
{
    /// <summary>
    /// Unique identifier for the issue.
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// Location where the issue is observed.
    /// </summary>
    [Required, StringLength(120)]
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Category that the issue belongs to.
    /// </summary>
    [Required]
    public IssueCategory Category { get; set; }

    /// <summary>
    /// Detailed description of the issue.
    /// </summary>
    [Required, StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// For this is was Optional: relative path to an uploaded attachment (e.g., under wwwroot/uploads).
    /// </summary>
    public string? AttachmentVirtualPath { get; set; }

    /// <summary>
    /// Date and time when the issue was created (in UTC).
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

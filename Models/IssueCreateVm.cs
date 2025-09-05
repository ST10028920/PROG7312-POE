using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MunicipalServicesMVC.Models;

/// <summary>
/// View model used when creating a new issue report.
/// </summary>
public sealed class IssueCreateVm
{
    /// <summary>
    /// Location where the issue is observed.
    /// </summary>
    [Required, StringLength(120)]
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Category of the issue being reported.
    /// Nullable to allow proper model validation binding.
    /// </summary>
    [Required]
    public IssueCategory? Category { get; set; }

    /// <summary>
    /// Detailed description of the issue.
    /// </summary>
    [Required, StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Optional file upload (e.g., photo evidence).
    /// </summary>
    public IFormFile? Attachment { get; set; }
}

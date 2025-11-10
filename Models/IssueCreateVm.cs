using System.ComponentModel.DataAnnotations;

namespace MunicipalServicesMVC.Models
{
    public sealed class IssueCreateVm
    {
        [Required(ErrorMessage = "Please enter a location.")]
        [StringLength(120)]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a category.")]
        public IssueCategory? Category { get; set; } // nullable now

        [Required(ErrorMessage = "Please select a priority level (1–5).")]
        [Range(1, 5, ErrorMessage = "Priority must be between 1 and 5.")]
        public int? Priority { get; set; } // nullable now

        [Required(ErrorMessage = "Please describe the issue.")]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        public IFormFile? Attachment { get; set; }
    }
}

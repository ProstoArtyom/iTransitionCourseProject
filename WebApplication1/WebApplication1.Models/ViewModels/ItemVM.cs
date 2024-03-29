using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.ViewModels
{
    public class ItemVM
    {
        public Item Item { get; set; }
        [Required]
        [MinLength(5)]
        [MaxLength(20)]
        public string TagName { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string CommentText { get; set; }
        [ValidateNever]
        public IEnumerable<Comment> Comments { get; set; }
        public Dictionary<string, string[]> CustomFields { get; set; }
        [Required]
        [MinLength(5)]
        [MaxLength(20)]
        public string FieldName { get; set; }
        [Required]
        [MinLength(5)]
        [MaxLength(20)]
        public string FieldValueInput { get; set; }
        [Required]
        [MinLength(5)]
        public string FieldValueTextArea { get; set; }
        public string FieldType { get; set; }
        public List<SelectListItem> TypesList { get; } = new()
        {
            new()
            {
                Text = "Numeric",
                Value = "number"
            },
            new()
            {
                Text = "Text",
                Value = "text"
            },
            new()
            {
                Text = "Date / Time",
                Value = "datetime-local"
            },
            new()
            {
                Text = "Logical",
                Value = "checkbox"
            },
            new()
            {
                Text = "Text area",
                Value = "textarea"
            }
        };
    }
}

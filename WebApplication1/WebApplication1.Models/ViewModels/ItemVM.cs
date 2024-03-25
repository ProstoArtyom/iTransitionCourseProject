using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.ViewModels
{
    public class ItemVM
    {
        public Item Item { get; set; }
        public string NewTagName { get; set; }
        public Dictionary<string, object[]> CustomFields { get; set; }

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

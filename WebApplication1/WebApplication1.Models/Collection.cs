using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApplication1.Models
{
    public class Collection
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public int ThemeId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(ThemeId))]
        public Theme Theme { get; set; }
        [NotMapped]
        public IEnumerable<Item> Items { get; set; }
    }
}

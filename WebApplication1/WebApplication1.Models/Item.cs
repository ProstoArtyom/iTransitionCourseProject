using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApplication1.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Column(TypeName = "json")]
        public string CustomFields { get; set; }
        [Required]
        public int CollectionId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(CollectionId))]
        public Collection Collection { get; set; }
        public List<Tag> Tags { get; } = new();
    }
}

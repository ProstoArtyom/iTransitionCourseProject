using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WebApplication1.CustomValidationAttributes;

namespace WebApplication1.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
        [Column(TypeName = "json")]
        public string? CustomFields { get; set; }
        [Required]
        public int CollectionId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(CollectionId))]
        public Collection Collection { get; set; }
        public List<ItemTag> ItemTags { get; set; }
    }
}

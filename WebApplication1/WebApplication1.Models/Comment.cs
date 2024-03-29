using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApplication1.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Text { get; set; }
        [Required]
        public int ItemId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(ItemId))]
        public Item Item { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; }
    }
}

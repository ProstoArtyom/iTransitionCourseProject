using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WebApplication1.CustomValidationAttributes;

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
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        [NotMapped]
        public IEnumerable<Item> Items { get; set; }
        [DataType(DataType.ImageUrl)]
        [Display(Name = "Poster")]
        public string ImageUrl { get; set; }
        [Display(Name = "Image File")]
        [MaxFileSize(1 * 1024 * 1024)]
        [PermittedExtensions(new string[] { ".jpg", ".png", ".gif", ".jpeg" })]
        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }
        public string ImageStorageName { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Like
    {
        [Key]
        public int Id { get; set; }
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

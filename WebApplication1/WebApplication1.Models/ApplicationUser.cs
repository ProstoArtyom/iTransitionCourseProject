using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models;
public class ApplicationUser : IdentityUser
{
    [Required]
    public string Name { get; set; }
    [NotMapped]
    public string Role { get; set; }
    [NotMapped]
    public bool IsBlocked { get; set; }
}

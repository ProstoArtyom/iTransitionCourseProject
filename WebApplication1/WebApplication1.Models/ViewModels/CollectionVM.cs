﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.ViewModels
{
    public class CollectionVM
    {
        public Collection Collection { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ThemesList { get; set; }
    }
}
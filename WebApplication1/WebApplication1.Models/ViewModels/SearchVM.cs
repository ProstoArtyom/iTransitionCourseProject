using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Models.ViewModels
{
    public class SearchVM
    {
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<Collection> Collections { get; set; }
    }
}

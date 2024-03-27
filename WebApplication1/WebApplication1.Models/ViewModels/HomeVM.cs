using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Collection> Collections { get; set; }
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
    }
}

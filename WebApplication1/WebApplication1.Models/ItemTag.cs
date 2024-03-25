using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class ItemTag
    {
        public Item Item { get; set; }
        [ForeignKey(nameof(Item))]
        public int ItemId { get; set; }
        public Tag Tag { get; set; }
        [ForeignKey(nameof(Tag))]
        public int TagId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryManagementSystem.Models
{
    public class OrderedItems
    {
        public int OrderId { get; set; }
        public int AssetId { get; set; }
        public string AssetName { get; set; }
        public DateTime? OrderedOn { get; set; }
        public int? QtyOrdered { get; set; }        
    }
}
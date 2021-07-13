using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryManagementSystem.Models
{
    public class AssetData
    {
        public int Asset_Id { get; set; }
        public string Asset_Name { get; set; }
        public int? Asset_Value { get; set; }
    }
}
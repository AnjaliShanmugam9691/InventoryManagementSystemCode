//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InventoryManagementSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class User_Details
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User_Details()
        {
            this.Order_Details = new HashSet<Order_Details>();
        }
    
        public int UserName_ID { get; set; }
        public string CompanyName_VC { get; set; }
        public string Password_VC { get; set; }
        public Nullable<bool> IsCustomer_BT { get; set; }
        public Nullable<System.DateTime> CreatedOn_DT { get; set; }
        public Nullable<System.DateTime> UpdatedOn_DT { get; set; }
        public Nullable<bool> IsActive_BT { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_Details> Order_Details { get; set; }
    }
}

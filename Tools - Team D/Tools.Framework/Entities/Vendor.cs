namespace Tools.Framework.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Vendor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Vendor()
        {
            PurchaseOrders = new HashSet<PurchaseOrder>();
            StockItems = new HashSet<StockItem>();
        }

        public int VendorID { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The name of the vendor should be 100 characters at most.")]
        public string VendorName { get; set; }

        [Required]
        [StringLength(12, ErrorMessage = "The phone number of the vendor should be 12 characters.")]
        public string Phone { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "The address should be 30 characters at most.")]
        public string Address { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The city's name should be 50 characters at most.")]
        public string City { get; set; }

        [Required]
        [StringLength(2, ErrorMessage = "The ProvinceID should only be 2 characters.")]
        public string ProvinceID { get; set; }

        [Required]
        [StringLength(6, ErrorMessage = "The Postal should be 6 characters.")]
        public string PostalCode { get; set; }

        public virtual Province Province { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockItem> StockItems { get; set; }
    }
}

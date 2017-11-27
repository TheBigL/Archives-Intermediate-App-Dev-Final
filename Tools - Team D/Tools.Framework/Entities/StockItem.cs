namespace Tools.Framework.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StockItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StockItem()
        {
            PurchaseOrderDetails = new HashSet<PurchaseOrderDetail>();
            SaleDetails = new HashSet<SaleDetail>();
            SaleRefundDetails = new HashSet<SaleRefundDetail>();
            ShoppingCartItems = new HashSet<ShoppingCartItem>();
        }

        public int StockItemID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The item description should be 50 characters at most.")]
        public string Description { get; set; }

        [Column(TypeName = "money")]
        public decimal SellingPrice { get; set; }

        [Column(TypeName = "money")]
        public decimal PurchasePrice { get; set; }

        public int QuantityOnHand { get; set; }

        public int QuantityOnOrder { get; set; }

        public int ReOrderLevel { get; set; }

        public bool Discontinued { get; set; }

        public int VendorID { get; set; }

        [StringLength(25, ErrorMessage = "The Stock Number should be 25 characters at most.")]
        public string VendorStockNumber { get; set; }

        public int CategoryID { get; set; }

        public virtual Category Category { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SaleDetail> SaleDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SaleRefundDetail> SaleRefundDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }

        public virtual Vendor Vendor { get; set; }
    }
}

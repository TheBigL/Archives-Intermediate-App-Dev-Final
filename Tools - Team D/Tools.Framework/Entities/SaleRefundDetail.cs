namespace Tools.Framework.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SaleRefundDetail
    {
        public int SaleRefundDetailID { get; set; }

        public int SaleRefundID { get; set; }

        public int StockItemID { get; set; }

        [Column(TypeName = "money")]
        public decimal SellingPrice { get; set; }

        public int Quantity { get; set; }

        public virtual SaleRefund SaleRefund { get; set; }

        public virtual StockItem StockItem { get; set; }
    }
}

namespace Tools.Framework.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SaleDetail
    {
        public int SaleDetailID { get; set; }

        public int SaleID { get; set; }

        public int StockItemID { get; set; }

        [Column(TypeName = "money")]
        public decimal SellingPrice { get; set; }

        public int Quantity { get; set; }

        public bool Backordered { get; set; }

        public DateTime? ShippedDate { get; set; }

        public virtual Sale Sale { get; set; }

        public virtual StockItem StockItem { get; set; }
    }
}

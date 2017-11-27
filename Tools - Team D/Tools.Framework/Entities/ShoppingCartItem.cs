namespace Tools.Framework.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ShoppingCartItem")]
    public partial class ShoppingCartItem
    {
        public int ShoppingCartItemID { get; set; }

        public int ShoppingCartID { get; set; }

        public int StockItemID { get; set; }

        public int Quantity { get; set; }

        public virtual ShoppingCart ShoppingCart { get; set; }

        public virtual StockItem StockItem { get; set; }
    }
}

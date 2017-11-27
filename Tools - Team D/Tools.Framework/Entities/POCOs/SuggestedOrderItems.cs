using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Framework.Entities.POCOs
{
    public class SuggestedOrderItems
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public int QuantityOnHand { get; set; }
        public int ReOrderLevel { get; set; }
        public int QuantityOnOrder { get; set; }
        public int PurchaseOrderQuantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

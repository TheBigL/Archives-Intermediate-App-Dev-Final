using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Framework.Entities.POCOs
{
    public class NonSuggestedOrderItems
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public int QuantityOnHand { get; set; }
        public int ReOrderLevel { get; set; }
        public int QuantityOnOrder { get; set; }
        public int Buffer { get; set; }
        public decimal PurchasePrice { get; set; }
    }
}

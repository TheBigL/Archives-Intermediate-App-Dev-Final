using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Framework.Entities.POCOs
{
    public class ReceivingItems
    {
        public int PurchaseOrderID { get; set; }
        public int StockItemID { get; set; }
        public string StockItemDescription { get; set; }
        public int QuantityOrdered { get; set; }
        public int QuantityOutstanding { get; set; }
        public int QuantityReceived { get; set; }
        public int QuantityReturned { get; set; }
        public string ReturnReason { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Framework.Entities.POCOs
{
    public class OutstandingItem
    {
        public int StockItemID { get; set; }
        public int QuantityOutstanding { get; set; }
    }
}
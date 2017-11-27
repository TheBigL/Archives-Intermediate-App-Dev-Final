using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Framework.DAL;
using Tools.Framework.Entities;
using Tools.Framework.Entities.POCOs;

namespace Tools.Framework.BLL
{
    [DataObject]
    public class ReceivingController
    {
        /// <summary>
        /// Lists outstanding orders (not closed and has a value for the order date)
        /// </summary>
        /// <returns>List of outstanding orders</returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<ReceivingVendorOrder> ListOutstandingOrders()
        {
            using (var context = new ToolsContext())
            {
                var result = from orders in context.PurchaseOrders
                             where orders.Closed == false && orders.OrderDate != null
                             select new ReceivingVendorOrder()
                             {
                                 PurchaseOrderID = orders.PurchaseOrderID,
                                 OrderDate = orders.OrderDate,
                                 VendorName = orders.Vendor.VendorName,
                                 VendorPhone = orders.Vendor.Phone
                             };
                return result.ToList();
            }
        }

        /// <summary>
        /// Retrieves order date, vendor name, and vendor phone from an order ID
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>Order ID, Order Date, Vendor Name, and Vendor Phone</returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public ReceivingVendorOrder FindVendorByOrder(int orderId)
        {
            using (var context = new ToolsContext())
            {
                var result = from order in context.PurchaseOrders
                             where order.PurchaseOrderID == orderId
                             select new ReceivingVendorOrder()
                             {
                                 PurchaseOrderID = order.PurchaseOrderID,
                                 OrderDate = order.OrderDate,
                                 VendorName = order.Vendor.VendorName,
                                 VendorPhone = order.Vendor.Phone
                             };
                return result.SingleOrDefault();
            }
        }

        /// <summary>
        /// Retrieves the order details of an order (received or not) using a POCO class
        /// </summary>
        /// <param name="orderId">The PurchaseOrderID</param>
        /// <returns>ReceivingItems</returns>                        
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<ReceivingItems> FindOrderById(int orderId)
        {
            using (var context = new ToolsContext())
            {
                List<ReceivingItems> outputList = new List<ReceivingItems>();

                // Default values for inputs
                const int defaultNumber = 0;
                const string defaultText = "";

                // List of PurchaseOrderDetails that have ReceiveOrderDetails
                List<PurchaseOrderDetail> receivedOrders = (from puchaseOrderDetail in context.PurchaseOrderDetails
                                                            where puchaseOrderDetail.PurchaseOrderID == orderId &&
                                                                  puchaseOrderDetail.ReceiveOrderDetails.Count() > 0
                                                            select puchaseOrderDetail).ToList();

                // List of PurchaseOrderDetails that do not have ReceiveOrderDetails
                List<PurchaseOrderDetail> notReceivedOrders = (from puchaseOrderDetail in context.PurchaseOrderDetails
                                                               where puchaseOrderDetail.PurchaseOrderID == orderId &&
                                                                     puchaseOrderDetail.ReceiveOrderDetails.Count() == 0
                                                               select puchaseOrderDetail).ToList();

                // Adds all items in the list receivedOrders to the outputList
                foreach (var order in receivedOrders)
                {
                    var item = (from orderDetails in context.PurchaseOrderDetails
                                where orderDetails.PurchaseOrderDetailID == order.PurchaseOrderDetailID
                                select new ReceivingItems()
                                {
                                    PurchaseOrderID = orderDetails.PurchaseOrderID,
                                    StockItemID = orderDetails.StockItem.StockItemID,
                                    StockItemDescription = orderDetails.StockItem.Description,
                                    QuantityOrdered = orderDetails.Quantity,
                                    QuantityOutstanding = orderDetails.Quantity - (from ro in context.ReceiveOrderDetails
                                                                                   where ro.PurchaseOrderDetailID == orderDetails.PurchaseOrderDetailID
                                                                                   select ro.QuantityReceived).Sum(),
                                    QuantityReceived = defaultNumber,
                                    QuantityReturned = defaultNumber,
                                    ReturnReason = defaultText
                                }).SingleOrDefault();

                    outputList.Add(item);
                }

                // Adds all items in the list notReceivedOrders to the outputList
                foreach (var order in notReceivedOrders)
                {
                    var item = (from orderDetails in context.PurchaseOrderDetails
                                where orderDetails.PurchaseOrderDetailID == order.PurchaseOrderDetailID
                                select new ReceivingItems()
                                {
                                    PurchaseOrderID = orderDetails.PurchaseOrderID,
                                    StockItemID = orderDetails.StockItem.StockItemID,
                                    StockItemDescription = orderDetails.StockItem.Description,
                                    QuantityOrdered = orderDetails.Quantity,
                                    QuantityOutstanding = orderDetails.Quantity,
                                    QuantityReceived = defaultNumber,
                                    QuantityReturned = defaultNumber,
                                    ReturnReason = defaultText
                                }).SingleOrDefault();

                    outputList.Add(item);
                }

                return outputList.OrderBy(column => column.StockItemID).ToList(); // sorts the list by Stock Item ID
            }
        }

        /// <summary>
        /// Adds a new ReceiveOrderDetails
        /// </summary>
        /// <param name="receivingOrder"></param>
        public void ReceiveOrder(int orderId, List<ReceivingItems> receivingOrder)
        {
            using (var context = new ToolsContext())
            {
                bool orderCanBeClosed = true;

                // Creates a new Receive Order
                ReceiveOrder receiveOrder = new ReceiveOrder()
                {
                    PurchaseOrderID = orderId,
                    ReceiveDate = DateTime.Now
                };
                context.ReceiveOrders.Add(receiveOrder);

                // Loops through each item to add a ReceiveOrderDetails and/or ReturnedOrderDetails
                foreach (var item in receivingOrder)
                {
                    // Gets StockItemID and PurchaseOrderDetailID
                    int stockItemId = item.StockItemID;
                    int purchaseOrderDetailID = (from purchaseOrderDetail in context.PurchaseOrderDetails
                                                 where purchaseOrderDetail.PurchaseOrderID == orderId &&
                                                       purchaseOrderDetail.StockItemID == stockItemId
                                                 select purchaseOrderDetail.PurchaseOrderDetailID).SingleOrDefault();

                    if (item.QuantityReceived > 0)
                    {
                        // Creates a new ReceiveOrderDetails
                        ReceiveOrderDetail newReceiveOrderDetails = new ReceiveOrderDetail();
                        newReceiveOrderDetails.PurchaseOrderDetailID = purchaseOrderDetailID;
                        newReceiveOrderDetails.QuantityReceived += item.QuantityReceived;

                        newReceiveOrderDetails.ReceiveOrder = receiveOrder;
                        context.ReceiveOrderDetails.Add(newReceiveOrderDetails);

                        // Updates Stock Item information (Quantity on Hand and Quantity on Order)
                        StockItem stockItem = context.StockItems.Attach(context.StockItems.Find(stockItemId));
                        stockItem.QuantityOnHand = stockItem.QuantityOnHand + item.QuantityReceived;
                        stockItem.QuantityOnOrder = stockItem.QuantityOnOrder - item.QuantityReceived;

                        var dbItem = context.Entry(stockItem);
                        dbItem.State = EntityState.Modified;
                    }

                    if (item.QuantityReturned > 0)
                    {
                        // Creates a new ReturnedOrderDetails
                        ReturnedOrderDetail returnedOrderDetails = new ReturnedOrderDetail();
                        returnedOrderDetails.PurchaseOrderDetailID = purchaseOrderDetailID;
                        returnedOrderDetails.ItemDescription = item.StockItemDescription;
                        returnedOrderDetails.Quantity += item.QuantityReturned;
                        returnedOrderDetails.Reason = item.ReturnReason;

                        returnedOrderDetails.ReceiveOrder = receiveOrder;
                        context.ReturnedOrderDetails.Add(returnedOrderDetails);
                    }

                    // Checks if outstanding quantity is zero so the order can be closed
                    int outstandingQuantity = item.QuantityOutstanding - item.QuantityReceived;
                    if (outstandingQuantity > 0)
                    {
                        orderCanBeClosed = false;
                    }
                }

                // Checks if the order can be closed
                if (orderCanBeClosed)
                {
                    PurchaseOrder purchaseOrder = context.PurchaseOrders.Attach(context.PurchaseOrders.Find(orderId));
                    purchaseOrder.Closed = true;
                    var dbItem = context.Entry(purchaseOrder);
                    dbItem.State = EntityState.Modified;
                }

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Closes an order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="ForceCloseReason"></param>
        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public void CloseOrder(int orderId, string ForceCloseReason, List<OutstandingItem> outstandingItems)
        {
            using (var context = new ToolsContext())
            {
                // Updates the purchase order
                PurchaseOrder purchaseOrder = context.PurchaseOrders.Attach(context.PurchaseOrders.Find(orderId));
                purchaseOrder.Closed = true;
                purchaseOrder.Notes = ForceCloseReason;
                var dbItem = context.Entry(purchaseOrder);
                dbItem.State = EntityState.Modified;

                foreach (var outstandingItem in outstandingItems)
                {
                    // Updates Stock Item information (Quantity on Order)
                    StockItem stockItem = context.StockItems.Attach(context.StockItems.Find(outstandingItem.StockItemID));
                    stockItem.QuantityOnOrder = stockItem.QuantityOnOrder - outstandingItem.QuantityOutstanding;

                    var stockDbItem = context.Entry(stockItem);
                    stockDbItem.State = EntityState.Modified;
                }

                context.SaveChanges();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Framework.DAL;
using Tools.Framework.Entities;
using Tools.Framework.Entities.POCOs;

namespace Tools.Framework.BLL
{
    [DataObject]
    public class PurchasingController
    {
        #region Purchasing Page

        #region DropdownLists
        //Retreives the Employee via a Dropdown List
        #region ListEmployees
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<PurchasingEmployee> ListEmployees()
        {
            using (var context = new ToolsContext())
            {
                var data = from employee in context.Employees
                           orderby employee.LastName
                           select new PurchasingEmployee()
                           {
                               ID = employee.EmployeeID,
                               Name = employee.FirstName + ", " + employee.LastName
                           };

                return data.ToList();
            }

            
        }
        #endregion
        
        #region GetVendorByID
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public VendorInfo GetVendorByID(int vendorID)
        {
            using (var context = new ToolsContext())
            {
                var data = from vendor in context.Vendors
                           where vendor.VendorID == vendorID
                           select new VendorInfo()
                           {
                               Name = vendor.VendorName,
                               Province = vendor.Province.Description,
                               Phone = vendor.Phone

                           };

                return data.FirstOrDefault();
                
            }
            
        }


        #endregion

        #region VendorsList
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<PurchasingVendor> ListVendors()
        {
            // Access the database
            using (var context = new ToolsContext())
            {
                var purchaseOrders = from po in context.PurchaseOrders
                                     where po.OrderDate == null
                                     select po;


                var results = from vendor in context.Vendors
                              orderby vendor.VendorID
                              select new
                              {
                                  VendorID = vendor.VendorID,
                                  VendorName = vendor.VendorName,
                                  PurchaseOrderID = from vendorpo in purchaseOrders
                                                    where vendorpo.VendorID == vendor.VendorID
                                                    select vendorpo.PurchaseOrderID
                              };
                var finalResults = from po in results
                                   select new PurchasingVendor()
                                   {
                                       VendorID = po.VendorID,
                                       VendorNameAndOrder = po.PurchaseOrderID.FirstOrDefault() != 0 ? po.VendorName + " - " + po.PurchaseOrderID.FirstOrDefault().ToString() : po.VendorName + " - none"
                                   };
 
                return finalResults.ToList();


            }
        }



        #endregion
        #endregion


        #region Purchase Order CRUD
        #region Start Purchase Order
        public void StartPurchaseOrder(List<SuggestedOrderItems> podetails, int employeeid, int vendorid, decimal tax, decimal sub)
        {
            using (ToolsContext context = new ToolsContext())
            {
                PurchaseOrder newOrder = new PurchaseOrder()
                {
                    EmployeeID = employeeid,
                    VendorID = vendorid,
                    OrderDate = null,
                    TaxAmount = tax,
                    SubTotal = sub
                };

                context.PurchaseOrders.Add(newOrder);

                foreach (var po in podetails)
                {
                    newOrder.PurchaseOrderDetails.Add(new PurchaseOrderDetail() { StockItemID = po.ID, Quantity = po.PurchaseOrderQuantity });
                }

                context.PurchaseOrders.Add(newOrder);

                context.SaveChanges();
            }
        }
        #endregion

        #region Get Suggested Orders
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SuggestedOrderItems> GetSuggestedOrders(int poID)
        {
            using (ToolsContext context = new ToolsContext())
            {
                var results = from item in context.PurchaseOrderDetails
                              where item.PurchaseOrderID == poID
                              select new SuggestedOrderItems()
                              {
                                  ID = item.StockItemID,
                                  Description = item.StockItem.Description,
                                  QuantityOnHand = item.StockItem.QuantityOnHand,
                                  QuantityOnOrder = item.StockItem.QuantityOnOrder,
                                  ReOrderLevel = item.StockItem.ReOrderLevel,
                                  PurchaseOrderQuantity = item.Quantity,
                                  UnitPrice = item.PurchasePrice
                              };
                return results.ToList();
                              
            }

        }


        #endregion

        #region Get NonSuggested Items
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<NonSuggestedOrderItems> GetNonSuggestedItems(int vendorID, int po)
        {
            using (ToolsContext context = new ToolsContext())
            {
                var results = from item in context.StockItems
                              where item.VendorID == vendorID && !(from podetails in context.PurchaseOrderDetails
                                                                   where podetails.PurchaseOrderID == po
                                                                   select podetails.StockItemID).Contains(item.StockItemID)
                              select new NonSuggestedOrderItems()
                              {
                                  ID = item.StockItemID,
                                  Description = item.Description,
                                  QuantityOnHand = item.QuantityOnHand,
                                  ReOrderLevel = item.ReOrderLevel,
                                  QuantityOnOrder = item.QuantityOnOrder,
                                  Buffer = item.QuantityOnHand - item.ReOrderLevel,
                                  PurchasePrice = item.PurchasePrice
                              };
                return results.ToList();
            }
        }
        #endregion

        /// <summary>
        /// Grabs the Non Suggested Items without the Purchase Order ID should the system fail.
        /// </summary>
        /// <param name="vendorid"></param>
        /// <returns></returns>
        #region Get Non Suggested Order Items
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<NonSuggestedOrderItems> GetNonSuggestedOrderItems(int vendorid)
        {
            using (var context = new ToolsContext())
            {
                var results = from item in context.StockItems
                              where item.ReOrderLevel - (item.QuantityOnHand + item.QuantityOnOrder) < 0 && item.VendorID == vendorid
                              select new NonSuggestedOrderItems
                              {
                                  ID = item.StockItemID,
                                  Description = item.Description,
                                  QuantityOnHand = item.QuantityOnHand,
                                  ReOrderLevel = item.ReOrderLevel,
                                  QuantityOnOrder = item.QuantityOnOrder,
                                  Buffer = item.QuantityOnHand - item.ReOrderLevel,
                                  PurchasePrice = item.PurchasePrice
                              };
                return results.ToList();
            }
        }
        #endregion

        #region Update
        public void UpdatePurchaseOrder(List<SuggestedOrderItems> podetails, PurchaseOrder po)
        {
            using (var context = new ToolsContext())
            {
                var purchaseorder = context.PurchaseOrders.Find(po.PurchaseOrderID);

                if (purchaseorder == null) throw new ArgumentException("Invalid PO ID - Does not exist");

                foreach (var item in podetails)
                {
                    var purchaseorderdetail = purchaseorder.PurchaseOrderDetails.Single(x => x.StockItemID == item.ID);
                    purchaseorderdetail.Quantity = item.PurchaseOrderQuantity;
                    purchaseorderdetail.PurchasePrice = item.UnitPrice;
                    context.Entry<PurchaseOrderDetail>(context.PurchaseOrderDetails.Attach(purchaseorderdetail)).State = System.Data.Entity.EntityState.Modified;
                }


                purchaseorder.SubTotal = po.SubTotal;
                purchaseorder.TaxAmount = po.TaxAmount;

                context.Entry<PurchaseOrder>(context.PurchaseOrders.Attach(purchaseorder)).State = System.Data.Entity.EntityState.Modified;

                context.SaveChanges();
            }
        }
        #endregion

        #region Delete Order
        public void DeleteOrder(int purchaseorderid, List<SuggestedOrderItems> stockitemids)
        {
            using (var context = new ToolsContext())
            {
                var purchaseorder = context.PurchaseOrders.Find(purchaseorderid);
                if (purchaseorder == null) throw new ArgumentException("Invalid PO ID - Does not exist");
                List<PurchaseOrderDetail> toRemove = new List<PurchaseOrderDetail>();

                foreach (var item in purchaseorder.PurchaseOrderDetails)
                {
                    bool stockitemidmatch = stockitemids.Any(x => x.ID == item.StockItemID);
                    if (stockitemidmatch)
                    {
                        toRemove.Add(item);
                    }
                }
                foreach (var item in toRemove)
                {
                    context.PurchaseOrderDetails.Remove(item);
                }

                context.PurchaseOrders.Remove(purchaseorder);

                context.SaveChanges();

            }
        }
        #endregion

        #region AddToOrder
        public void AddToOrder(PurchaseOrderDetail purchaseorderdetail)
        {
            using (ToolsContext context = new ToolsContext())
            {
                List<string> errors = new List<string>();

                if (purchaseorderdetail.Quantity < 1)
                {
                    errors.Add("Need atleast one purchase order quantity");
                }

                if (errors.Count > 0)
                {
                    throw new BusinessRuleException("Unable to add to Purchase Order", errors);
                }

                context.PurchaseOrderDetails.Add(purchaseorderdetail);


                context.SaveChanges();

            }
        }
        #endregion

        #region Get Order
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SuggestedOrderItems> GetOrder(int purchaseorderid)
        {
            using (var context = new ToolsContext())
            {
                var results = from item in context.PurchaseOrderDetails
                              where item.PurchaseOrderID == purchaseorderid
                              select new SuggestedOrderItems
                              {
                                  ID = item.StockItemID,
                                  Description = item.StockItem.Description,
                                  QuantityOnHand = item.StockItem.QuantityOnHand,
                                  ReOrderLevel = item.StockItem.ReOrderLevel,
                                  QuantityOnOrder = item.StockItem.QuantityOnOrder,
                                  PurchaseOrderQuantity = item.Quantity,
                                  UnitPrice = item.StockItem.PurchasePrice
                              };
                return results.ToList();
            }
        }
        #endregion

        #region DeleteFromOrder
        public void RemoveFromOrder(int purchaseorderid, int stockitemid)
        {
            using (ToolsContext context = new ToolsContext())
            {
                List<string> errors = new List<string>();

                if (errors.Count > 0)
                {
                    throw new BusinessRuleException("Unable to add to Purchase Order", errors);
                }

                var purchaseorder = context.PurchaseOrders.Find(purchaseorderid);
                if (purchaseorder == null) throw new ArgumentException("Invalid Purchase Order ID - does not exist");

                List<PurchaseOrderDetail> toRemove = new List<PurchaseOrderDetail>();
                foreach (var item in purchaseorder.PurchaseOrderDetails)
                {
                    if (item.StockItemID == stockitemid && item.PurchaseOrderID == purchaseorderid)
                    {
                        toRemove.Add(item);
                    }

                }

                foreach (var item in toRemove)
                {
                    context.PurchaseOrderDetails.Remove(item);
                }
                context.SaveChanges();

            }
        }
        #endregion

        #region Place Order
        public void PlaceOrder(DateTime now, int purchaseorderid, List<SuggestedOrderItems> stockitemquantity)
        {
            using (var context = new ToolsContext())
            {
                var purchaseOrder = context.PurchaseOrders.Find(purchaseorderid);
                if (purchaseOrder == null) throw new ArgumentException("Invalid PO ID - Does not exist");

                purchaseOrder.OrderDate = now;

                context.Entry<PurchaseOrder>(context.PurchaseOrders.Attach(purchaseOrder)).State = System.Data.Entity.EntityState.Modified;


                List<PurchaseOrderDetail> toUpdateStockItem = new List<PurchaseOrderDetail>();
                foreach (var item in purchaseOrder.PurchaseOrderDetails)
                {
                    bool founditem = stockitemquantity.Any(x => x.ID == item.StockItemID);
                    if (founditem)
                    {
                        toUpdateStockItem.Add(item);
                    }
                }
                foreach (var item in toUpdateStockItem)
                {
                    //var purchasedetail = context.PurchaseOrderDetails.Find(item.PurchaseOrderDetailID);
                    var foundstockitem = context.StockItems.Find(item.StockItemID);
                    //bool match = podetails.Any(x => x.ID == item.StockItemID);
                    var match = stockitemquantity.Single(x => x.ID == item.StockItemID);

                    foundstockitem.QuantityOnOrder = match.PurchaseOrderQuantity;
                    context.Entry<StockItem>(context.StockItems.Attach(foundstockitem)).State = System.Data.Entity.EntityState.Modified;


                }

                context.SaveChanges();
            }
        }
        #endregion


        #region Review Suggested Order
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SuggestedOrderItems> ReviewSuggestedOrder(int vendorid)
        {
            using (var context = new ToolsContext())
            {
                var results = from item in context.StockItems
                              where item.ReOrderLevel - (item.QuantityOnHand + item.QuantityOnOrder) > 0 && item.VendorID == vendorid
                              select new SuggestedOrderItems()
                              {
                                  ID = item.StockItemID,
                                  Description = item.Description,
                                  QuantityOnHand = item.QuantityOnHand,
                                  ReOrderLevel = item.ReOrderLevel,
                                  QuantityOnOrder = item.QuantityOnOrder,
                                  PurchaseOrderQuantity = item.ReOrderLevel - item.QuantityOnHand,
                                  UnitPrice = item.PurchasePrice
                              };
                return results.ToList();
            }
        }
        #endregion

        #endregion



        #endregion
    }
}
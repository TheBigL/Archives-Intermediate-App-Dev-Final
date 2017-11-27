using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tools.Framework.BLL;
using Tools.Framework.Entities;
using Tools.Framework.Entities.POCOs;

public partial class Purchasing_Default : System.Web.UI.Page
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            GetSuggestedOrder.Text = "";
            UpdateOrderLink.Text = "";
            RefreshVendorList("0");
        }

        // Checks if the user is authenticated and belongs to the admin or staff roles
        if (!Request.IsAuthenticated) // user is not authenticated
        {
            Response.Redirect("~/Account/Login.aspx");
        }
        else if (!User.IsInRole("WebsiteAdmins")) // if the user is not an admin
        {
            if (!User.IsInRole("Staff")) // if the user is not a staff member
            {
                Response.Redirect("~/Account/Login.aspx");
            }
        }
        else if (!User.IsInRole("Staff")) // if the user is not a staff member
        {
            if (!User.IsInRole("WebsiteAdmins")) // if the user is not an admin
            {
                Response.Redirect("~/Account/Login.aspx");
            }
        }
    }

    #endregion


    #region Refresh Vendor List
    protected void RefreshVendorList(string selectedvalue)
    {
        VendorDropdown.DataBind();
        VendorDropdown.Items.Insert(0, "[Select a Vendor]");
        if (VendorDropdown.SelectedValue.Contains(selectedvalue))
        {
            VendorDropdown.SelectedIndex = 0;
        }
    }
    #endregion

    #region Delete Order
    protected void DeleteOrderLink_Click(object sender, EventArgs e)
    {
        string purchaseOrderIDString = PurchaseOrderDisplay.Text;
        string vendorID = VendorDropdown.Text;



        if (purchaseOrderIDString == "" || purchaseOrderIDString == "0")
        {
            MessageUserControl.ShowInfo("You must have purchase order first");
        }

        else if(vendorID == "")
        {
            MessageUserControl.ShowInfo("You must have a vendor.");
        }

        else if(SuggestedItems.Rows.Count == 0)
        {
            MessageUserControl.ShowInfo("You must have at least one item in order to delete the purchase order.");
        }

        else
        {
            MessageUserControl.TryRun(() =>
            {
                int PurchaseOrderID = int.Parse(purchaseOrderIDString);

                var stockItemID = int.Parse(purchaseOrderIDString);
                var stockItems = new List<SuggestedOrderItems>();
                foreach (GridViewRow gvr in SuggestedItems.Rows)
                {
                    SuggestedOrderItems item = new SuggestedOrderItems();
                    item.ID = int.Parse(SuggestedItems.Rows[gvr.RowIndex].Cells[0].Text);
                    stockItems.Add(item);
                }
                PurchasingController sysmgr = new PurchasingController();
                sysmgr.DeleteOrder(PurchaseOrderID, stockItems);

                SuggestedItems.DataSource = null;
                SuggestedItems.DataBind();
                NonSuggestedItems.DataBind();
                GetSuggestedOrder.Text = "";
                VendorName.Text = "";
                VendorPhone.Text = "";
                UpdateOrderLink.Text = "";
                RefreshVendorList("0");
                subtotalAmount.Text = "";
                gstAmount.Text = "";
                totalAmount.Text = "";


            }, "Purchase Order Deleted", "All purchase order details removed as well from that order");


        }
    }
    #endregion

    #region Update Order
    protected void UpdateOrderLink_Click(object sender, EventArgs e)
    {

        decimal subtotal = 0.00M;
        decimal unitPrice = 0.00M;
        decimal gst = 0.00M;
        int quantity = 0;


        if (PurchaseOrderDisplay.Text == "")
        {
            MessageUserControl.ShowInfo("You need at least one item");
        }

        else if (VendorDropdown.SelectedValue == "")
        {
            MessageUserControl.ShowInfo("You need to select a Vendor!");
        }
        else
        {
            MessageUserControl.TryRun(() =>
            {
                int purchaseOrderID = int.Parse(PurchaseOrderDisplay.Text);
                int vendorID = int.Parse(VendorDropdown.SelectedValue);

                var list = new List<SuggestedOrderItems>();
                foreach (GridViewRow gvr in SuggestedItems.Rows)
                {
                    SuggestedOrderItems updateditem = new SuggestedOrderItems();
                    updateditem.ID = (int.Parse(SuggestedItems.Rows[gvr.RowIndex].Cells[0].Text));
                    updateditem.PurchaseOrderQuantity = int.Parse((gvr.FindControl("PurchaseOrderQuantity") as TextBox).Text);
                    updateditem.UnitPrice = decimal.Parse((gvr.FindControl("UnitPrice") as TextBox).Text);
                    unitPrice = updateditem.UnitPrice;
                    quantity = updateditem.PurchaseOrderQuantity;
                    list.Add(updateditem);

                    subtotal = subtotal + (quantity * unitPrice);

                }

                PurchaseOrder updatedorder = new PurchaseOrder();
                updatedorder.PurchaseOrderID = purchaseOrderID;
                updatedorder.SubTotal = subtotal;
                updatedorder.TaxAmount = gst;

                PurchasingController sysmgr = new PurchasingController();
                sysmgr.UpdatePurchaseOrder(list, updatedorder);
                subtotalAmount.Text = subtotal.ToString("C");
                gstAmount.Text = subtotal.ToString("C");
                totalAmount.Text = (subtotal + gst).ToString("C");


            }, "Updated", "The order has been updated accordingly");
        }

    }
    #endregion

    #region Place Order
    protected void PlaceOrderLink_Click(object sender, EventArgs e)
    {
        decimal subtotal = decimal.Parse(subtotalAmount.Text);
        decimal taxAmount = decimal.Parse(gstAmount.Text);
        int quantity = 0;

        
        if (SuggestedItems.Rows.Count == 0)
        {
            MessageUserControl.ShowInfo("You don't have any a single item to order on your list. Put at least one item in and try again.");
        }

        else if(VendorDropdown.SelectedValue == "")
        {
            MessageUserControl.ShowInfo("You must select a vendor before you can select a value.");
        }



        else
        {
            MessageUserControl.TryRun(() =>
            {
                string vendorIDString = VendorDropdown.SelectedValue;
                int purchaseOrder;
                int employeeID = int.Parse(EmployeeDropdown.SelectedValue);
                int vendorID = int.Parse(vendorIDString);
                DateTime now = DateTime.Now;
                string stringOrder = VendorDropdown.SelectedItem.Text;
                string cutString = stringOrder.Substring(stringOrder.Length - 3);
               
                if (int.TryParse(cutString, out purchaseOrder))
                {
                    var list = new List<SuggestedOrderItems>();
                    foreach (GridViewRow gvr in SuggestedItems.Rows)
                    {
                        SuggestedOrderItems updatedItem = new SuggestedOrderItems();
                        updatedItem.ID = (int.Parse(SuggestedItems.Rows[gvr.RowIndex].Cells[0].Text));
                        updatedItem.PurchaseOrderQuantity = int.Parse((gvr.FindControl("PurchaseOrderQuantity") as TextBox).Text);
                        updatedItem.UnitPrice = decimal.Parse((gvr.FindControl("UnitPrice") as TextBox).Text);
                        quantity = updatedItem.PurchaseOrderQuantity;
                        list.Add(updatedItem);
                        subtotal += quantity * decimal.Parse((gvr.FindControl("UnitPrice") as TextBox).Text);
                    }

                    taxAmount = subtotal * 0.05M;
                    PurchasingController sysmgr = new PurchasingController();
                    
                    

                    PurchaseOrder updatedorder = new PurchaseOrder();
                    updatedorder.PurchaseOrderID = purchaseOrder;
                    updatedorder.OrderDate = now;
                    updatedorder.EmployeeID = employeeID;
                    updatedorder.TaxAmount = taxAmount;
                    updatedorder.SubTotal = subtotal;
                    updatedorder.Closed = true;
                    sysmgr.UpdatePurchaseOrder(list, updatedorder);
                    sysmgr.PlaceOrder(now, purchaseOrder, list);



                    UpdateOrderLink.Text = "";
                    GetSuggestedOrder.Text = "";
                    RefreshVendorList(vendorID.ToString());
                }

                else
                {
                    MessageUserControl.ShowInfo("Purchase Order must exist before placing order!");
                    RefreshVendorList(vendorIDString);
                }
            }, "Order has been Placed", "The order has now been placed and the list has been reset");




        }







    }
    #endregion

    #region Clear Order
    protected void ClearOrderLink_Click(object sender, EventArgs e)
    {
        decimal subtotal = 0.00M;
        decimal gst = 0.00M;

        RefreshVendorList("0");
        EmployeeDropdown.SelectedIndex = 0;
        PurchaseOrderDisplay.Text = "";
        VendorLocation.Text = "";
        VendorName.Text = "";
        VendorPhone.Text = "";
        GetSuggestedOrder.Text = "";
        subtotalAmount.Text = subtotal.ToString("C");
        gstAmount.Text = gst.ToString("C");
        totalAmount.Text = (gst + subtotal).ToString("C");
        SuggestedItems.DataSource = null;
        SuggestedItems.DataBind();
    }
    #endregion


    #region Remove from Orders
    protected void SuggestedItems_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (PurchaseOrderDisplay.Text != "")
        {
            if (e.CommandName.Equals("RemoveOrder"))
            {
                MessageUserControl.TryRun(() =>
                {
                    int purchaseorderid = int.Parse(PurchaseOrderDisplay.Text);
                    int stockitemid = int.Parse(e.CommandArgument.ToString());
                    PurchasingController po = new PurchasingController();
                    po.RemoveFromOrder(purchaseorderid, stockitemid);
                    SuggestedItems.DataBind();
                    NonSuggestedItems.DataBind();
                }, "Item Removed", "Item has been removed from purchase order");
            }

        }
    }
    #endregion

    #region Add To Orders
    protected void NonSuggestedItems_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int vendorID = int.Parse(VendorDropdown.SelectedValue);
        int purchaseOrderID = int.Parse(PurchaseOrderDisplay.Text);

        if (purchaseOrderID == 0)
        {
            MessageUserControl.ShowInfo("The Purchase Order ID doesn't exist.");
        }

        else if(vendorID == 0)
        {
            MessageUserControl.ShowInfo("The Vendor has not been selected. Pick a vendor.");
        }

        else if (e.CommandName.Equals("AddToOrder"))
        {
            MessageUserControl.TryRun(() =>
            {
                purchaseOrderID = int.Parse(PurchaseOrderDisplay.Text);
                int stockItemID = int.Parse(e.CommandArgument.ToString());
                int POQTY = 1;
                //int POQTY = int.Parse((SuggestedItems.FindControl("PurchaseOrderQuantity") as TextBox).Text);
                PurchaseOrderDetail addpod = new PurchaseOrderDetail();
                addpod.PurchaseOrderID = purchaseOrderID;
                addpod.StockItemID = stockItemID;
                addpod.Quantity = POQTY;
                PurchasingController sysmgr = new PurchasingController();
                sysmgr.AddToOrder(addpod);
                SuggestedItems.DataSource = sysmgr.GetOrder(addpod.PurchaseOrderID);
                SuggestedItems.DataBind();
                NonSuggestedItems.DataSource = sysmgr.GetNonSuggestedItems(addpod.PurchaseOrderID, vendorID);
                NonSuggestedItems.DataBind();
            }, "Item Added", "Item has been added to the current purchase order.");
        }
    }
    #endregion

    #region Get Suggested Order
    protected void GetSuggestedOrder_Click(object sender, EventArgs e)
    {
        int purchaseOrder;
        string stringOrder = VendorDropdown.SelectedItem.Text;
        string cutString = stringOrder.Substring(stringOrder.Length - 3);

        if (VendorDropdown.SelectedIndex != 0)
        {
            if (int.TryParse(cutString, out purchaseOrder))
            {
                PurchaseOrderDisplay.Text = purchaseOrder.ToString();
                PurchasingController sysmgr = new PurchasingController();
                List<SuggestedOrderItems> list = sysmgr.GetSuggestedOrders(purchaseOrder);
                SuggestedItems.DataSource = list;
                SuggestedItems.DataBind();

                List<NonSuggestedOrderItems> list2 = sysmgr.GetNonSuggestedItems(int.Parse(VendorDropdown.SelectedValue), purchaseOrder);
                NonSuggestedItems.DataSource = list2;
                NonSuggestedItems.DataBind();
                UpdateOrderLink.Text = "Update Order";
                DeleteOrderLink.Enabled = true;
                PlaceOrderLink.Enabled = true;
            }

            else
            {
                PurchaseOrderDisplay.Text = purchaseOrder.ToString();
                PurchasingController sysmgr = new PurchasingController();
                List<SuggestedOrderItems> list = sysmgr.ReviewSuggestedOrder(int.Parse(VendorDropdown.SelectedValue));
                SuggestedItems.DataSource = list;
                SuggestedItems.DataBind();
                foreach (GridViewRow gvr in SuggestedItems.Rows)
                {
                    (gvr.FindControl("RemoveItem") as Button).Visible = true;
                    (gvr.FindControl("RemoveItem") as Button).Enabled = false;
                }
                List<NonSuggestedOrderItems> list2 = sysmgr.GetNonSuggestedOrderItems(int.Parse(VendorDropdown.SelectedValue));
                foreach(GridViewRow gvr in NonSuggestedItems.Rows)
                {
                    (gvr.FindControl("AddToOrder") as Button).Enabled = false;
                    (gvr.FindControl("AddToOrder") as Button).Visible = true;
                }
                NonSuggestedItems.DataSource = list2;
                NonSuggestedItems.DataBind();
                DeleteOrderLink.Enabled = true;
                PlaceOrderLink.Enabled = true;



                
            }

            GetVendorInfo();

        }

        else
        {
            MessageUserControl.ShowInfo("You need a select a vendor before you can make the transaction");
        }
    }
    #endregion

    #region Retrieve Vendor Info
    public void GetVendorInfo()
    {
        PurchasingController sysmgr = new PurchasingController();
        var po = sysmgr.GetVendorByID(int.Parse(VendorDropdown.SelectedValue));
        VendorName.Text = "Vendor: " + po.Name;
        VendorLocation.Text = "Location: " + po.Province;
        VendorPhone.Text = "Phone: " + po.Phone;
    }
    #endregion

    #region Start Order
    protected void StartOrderLink_Click(object sender, EventArgs e)
    {
        decimal subtotal = 0.00M;
        decimal unitPrice = 0.00M;
        decimal gst = 0.00M;
        int quantity = 0;
        string stringOrder = VendorDropdown.SelectedItem.Text;
        string cutString = stringOrder.Substring(stringOrder.Length - 3);
        string purchaseorderstring = PurchaseOrderDisplay.Text;

        if (VendorDropdown.SelectedValue == null || VendorDropdown.SelectedIndex == 0)
        {
            MessageUserControl.ShowInfo("Please select a vendor to start order");
        }

       

        
        else
        {
            int vendorID = int.Parse(VendorDropdown.SelectedValue);
            MessageUserControl.TryRun(() =>
            {
                var list = new List<SuggestedOrderItems>();
                var employeeID = int.Parse(EmployeeDropdown.SelectedValue);
                foreach (GridViewRow gvr in SuggestedItems.Rows)
                {
                    SuggestedOrderItems newItem = new SuggestedOrderItems();
                    newItem.ID = (int.Parse(SuggestedItems.Rows[gvr.RowIndex].Cells[0].Text));
                    newItem.PurchaseOrderQuantity = int.Parse((gvr.FindControl("PurchaseOrderQuanity") as TextBox).Text);
                    newItem.UnitPrice = decimal.Parse((gvr.FindControl("UnitPrice") as TextBox).Text);
                    unitPrice = newItem.UnitPrice;
                    list.Add(newItem);

                    subtotal = subtotal + (quantity * unitPrice);
                }



                gst = subtotal * 0.05M;
                PurchasingController sysmgr = new PurchasingController();
                sysmgr.StartPurchaseOrder(list, employeeID, vendorID, gst, subtotal);
                subtotalAmount.Text = subtotal.ToString("C");
                gstAmount.Text = gst.ToString("C");
                totalAmount.Text = (gst + subtotal).ToString("C");
                

            }, "Created an Order", "Order has been established in the suggested order list");

        }




    }
    #endregion

    #region Vendor DropDown Value Changed
    protected void VendorDropdown_SelectedIndexChanged(object sender, EventArgs e)
    {
        int purchaseOrder;
        string stringOrder = VendorDropdown.SelectedItem.Text;
        string cutstring = stringOrder.Substring(stringOrder.Length - 3);
        if (VendorDropdown.SelectedIndex != 0)
        {
            if (int.TryParse(cutstring, out purchaseOrder))
            {
                GetSuggestedOrder.Text = "Get Order " + purchaseOrder;
            }
            else
            {
                GetSuggestedOrder.Text = "Preview Suggested Order";
            }
        }
        else
        {
            GetSuggestedOrder.Text = "";
        }

    }
    #endregion

    #region Grab Totals
    protected void SuggestedItems_DataBound(object sender, EventArgs e)
    {
        decimal subtotal = 0;
        int quantity = 0;
        decimal gst = 0.00M;

        foreach(GridViewRow gvr in SuggestedItems.Rows)
        {
            quantity = int.Parse((gvr.FindControl("PurchaseOrderQuantity") as TextBox).Text);
            subtotal += (quantity * decimal.Parse((gvr.FindControl("UnitPrice") as TextBox).Text));
        }

        subtotalAmount.Text = subtotal.ToString("C");
        gst = subtotal * 0.05M;
        gstAmount.Text = gst.ToString("C");
        totalAmount.Text = (subtotal + gst).ToString("C");
    }
    #endregion
}
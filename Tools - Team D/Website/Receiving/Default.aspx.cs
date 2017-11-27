using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tools.Framework.BLL;
using Tools.Framework.Entities.POCOs;
using Tools.UI;

public partial class Receiving_Default : System.Web.UI.Page
{
    private int currentOrderId = 0; // temporary purchase order ID when viewing and order
    private List<ReceivingItems> currentItemsList = new List<ReceivingItems>();// temporary ReceivingOrder items list that gets populated when clicking on the Receive button

    /// <summary>
    /// Executed when the page loads
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ReceivingItemsDiv.Visible = false;
        }

        // Checks if the user is authenticated and belongs to the admin or staff roles
        if(!Request.IsAuthenticated) // user is not authenticated
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
            if(!User.IsInRole("WebsiteAdmins")) // if the user is not an admin
            {
                Response.Redirect("~/Account/Login.aspx");
            }
        }
    }

    /// <summary>
    /// Executed when the user clicks on "View Order"
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ReceivingVendorOrderGridView_SelectedIndexChanged(object sender, EventArgs e)
    {
        ReceivingItemsDiv.Visible = true;
        MessageUserControl.TryRun((ProcessRequest)ViewOrder);
    }

    /// <summary>
    /// Retrieves vendor information and order items
    /// </summary>
    private void ViewOrder()
    {
        ReceivingController controller = new ReceivingController();
        currentOrderId = int.Parse(ReceivingVendorOrderGridView.SelectedValue.ToString());

        // Displays vendor information
        ReceivingVendorOrder vendorOrderInfo = controller.FindVendorByOrder(currentOrderId);
        PurchaseOrderIDLabel.Text = "PO #" + vendorOrderInfo.PurchaseOrderID.ToString();
        VendorNameLabel.Text = "Vendor: " + vendorOrderInfo.VendorName;
        VendorPhoneLabel.Text = "Contact Phone: (" + vendorOrderInfo.VendorPhone + ")";

        // Retrieves order information
        List<ReceivingItems> currentReceivingOrder = new List<ReceivingItems>();
        currentReceivingOrder = controller.FindOrderById(currentOrderId);
        foreach (var item in currentReceivingOrder)
        {
            item.PurchaseOrderID = currentOrderId;
        }

        // Populates GridView
        ReceivingItemsGridView.DataSource = currentReceivingOrder;
        ReceivingItemsGridView.DataBind();

        string parameter = Request.QueryString[currentOrderId.ToString()];
    }

    /// <summary>
    /// Executed when the user clicks on the Receive button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ReceiveButton_Click(object sender, EventArgs e)
    {
        currentItemsList.Clear(); // starts the list from scratch

        foreach (GridViewRow row in ReceivingItemsGridView.Rows)
        {
            // Gets all values from the GridView
            int stockItemID = int.Parse(row.Cells[0].Text);
            string stockItemDescription = row.Cells[1].Text;
            int quantityOrdered = int.Parse(row.Cells[2].Text);
            int quantityOutstanding = int.Parse(row.Cells[3].Text);
            TextBox receiveInput, returnInput, reasonInput;
            receiveInput = row.FindControl("ReceiveTextBox") as TextBox;
            returnInput = row.FindControl("ReturnTextBox") as TextBox;
            reasonInput = row.FindControl("ReasonTextBox") as TextBox;
            string receiveText = receiveInput.Text.Trim();
            string returnText = returnInput.Text.Trim();
            string reasonText = reasonInput.Text.Trim();

            // Replaces empty texts in quantities with zeros
            if (String.IsNullOrEmpty(receiveText))
            {
                receiveText = "0";
                receiveInput.Text = receiveText;
            }
            if (String.IsNullOrEmpty(returnText))
            {
                returnText = "0";
                returnInput.Text = returnText;
            }

            // Gets booleans to validate numbers
            int receiveNumber;
            bool receiveIsNumber = int.TryParse(receiveText, out receiveNumber);
            int returnNumber;
            bool returnIsNumber = int.TryParse(returnText, out returnNumber);

            // Validation
            if (!receiveIsNumber || !returnIsNumber || receiveNumber < 0 || returnNumber < 0)
            {
                // Validates integers
                MessageUserControl.ShowInfo("Only positive integers are allowed in the Receive and Return fields.");
                return;
            }
            else if (receiveNumber > quantityOutstanding)
            {
                // Quantity received must not be greater than the quantity outstanding
                MessageUserControl.ShowInfo("Quantity received must not be greater than the quantity outstanding.");
                return;
            }
            else if (returnNumber > quantityOutstanding)
            {
                // Quantity returned must not be greater than the quantity outstanding
                MessageUserControl.ShowInfo("Quantity returned must not be greater than the quantity outstanding.");
                return;
            }
            else if ((receiveNumber + returnNumber) > quantityOutstanding)
            {
                // Receive and return fields summed cannot exceed the outstanding field
                MessageUserControl.ShowInfo("The number of items to receive and return cannot exceed the number of outstanding items.");
                return;
            }
            else if (int.Parse(returnText) > 0 && reasonText == "")
            {
                // Returned items must have reasons specified
                MessageUserControl.ShowInfo("You must specify a reason for all items to be returned.");
                return;
            }
            else if (!String.IsNullOrEmpty(reasonText) && int.Parse(returnText) < 1)
            {
                // Specified reasons must have returned quantities entered
                MessageUserControl.ShowInfo("You must specify the quantity of all items to be returned.");
                return;
            }
            else
            {
                // Adds row to the temporary list
                ReceivingItems receivingItems = new ReceivingItems();
                receivingItems.StockItemID = stockItemID;
                receivingItems.StockItemDescription = Server.HtmlDecode(stockItemDescription); // Server.HtmlDecode reverses HTML encode changes made (e.g. & converted to amp;)
                receivingItems.QuantityOrdered = quantityOrdered;
                receivingItems.QuantityOutstanding = quantityOutstanding;
                receivingItems.QuantityReceived = int.Parse(receiveText);
                receivingItems.QuantityReturned = int.Parse(returnText);
                receivingItems.ReturnReason = Server.HtmlDecode(reasonText); // Server.HtmlDecode reverses HTML encode changes made (e.g. & converted to amp;)

                currentItemsList.Add(receivingItems);
            }
        }

        // Checks if a field was changed to proceed with adding a new receive order
        foreach (var item in currentItemsList)
        {
            if (item.QuantityReceived > 0 || item.QuantityReturned > 0)
            {
                currentOrderId = (int)ReceivingItemsGridView.DataKeys[0].Value; // gets the order ID from a DataKey
                MessageUserControl.TryRun((ProcessRequest)SubmitReceiveOrder, "Receive Order", "Sucessfully updated the Purchase Order information.");
                return;
            }
        }

        // Displays a message if the user does not make changes in any field
        MessageUserControl.ShowInfo("No changes were made since no fields were edited.");
        ClearAndRefreshPage();
    }

    /// <summary>
    /// Adds a Receive Order to the database
    /// </summary>
    private void SubmitReceiveOrder()
    {
        // Stores information in a ReceivingItems instance
        List<ReceivingItems> receivingItems = new List<ReceivingItems>();
        receivingItems = currentItemsList;
        foreach (var item in receivingItems)
        {
            item.PurchaseOrderID = currentOrderId;
        }

        // Submits information
        ReceivingController controller = new ReceivingController();
        controller.ReceiveOrder(currentOrderId, receivingItems);

        ClearAndRefreshPage();
    }

    /// <summary>
    /// Clears fields and refreshes the Outstanding Orders GridView
    /// </summary>
    private void ClearAndRefreshPage()
    {
        //Clears fields
        ClearFields();

        // Refreshes GridView
        ReceivingVendorOrderGridView.DataBind();
    }

    /// <summary>
    /// Clears fields
    /// </summary>
    private void ClearFields()
    {
        // Properties
        currentOrderId = 0;
        currentItemsList.Clear();

        // Labels
        PurchaseOrderIDLabel.Text = "";
        VendorNameLabel.Text = "";
        VendorPhoneLabel.Text = "";

        // GridView
        ReceivingItemsGridView.DataSource = null;
        ReceivingItemsGridView.DataBind();

        // Div
        ReceivingItemsDiv.Visible = false;
    }

    /// <summary>
    /// Executed when the user clicks on the Force Close button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ForceCloseButton_Click(object sender, EventArgs e)
    {
        if (String.IsNullOrWhiteSpace(ForceCloseReasonTextBox.Text.Trim()))
        {
            MessageUserControl.ShowInfo("You must specify a reason for closing the order.");
        }
        else
        {
            currentOrderId = (int)ReceivingItemsGridView.DataKeys[0].Value; // gets the order ID from a DataKey
            MessageUserControl.TryRun((ProcessRequest)SubmitForceClose, "Force Close Order", "The Purchase Order was successfully closed.");
        }
    }

    /// <summary>
    /// Force closes an order
    /// </summary>
    private void SubmitForceClose()
    {
        // Creates a list of outstanding items (StockItemID and quantity outstanding)
        List<OutstandingItem> outstandingItems = new List<OutstandingItem>();
        foreach (GridViewRow row in ReceivingItemsGridView.Rows)
        {
            // Adds row to the temporary list
            OutstandingItem item = new OutstandingItem();
            item.StockItemID = int.Parse(row.Cells[0].Text);
            item.QuantityOutstanding = int.Parse(row.Cells[3].Text);

            outstandingItems.Add(item);
        }

        // Submits information
        ReceivingController controller = new ReceivingController();
        controller.CloseOrder(currentOrderId, ForceCloseReasonTextBox.Text.Trim(), outstandingItems);

        ClearAndRefreshPage();
    }

    /// <summary>
    /// Handles exception with the user control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void CheckForException(object sender, ObjectDataSourceStatusEventArgs e)
    {
        MessageUserControl.HandleDataBoundException(e);
    }
}
<%@ Page Title="Receiving" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Receiving_Default" %>

<%@ Register Src="~/UserControls/MessageUserControl.ascx" TagPrefix="uc1" TagName="MessageUserControl" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="row">
        <h2>Receiving</h2>
        <%-- User Control --%>
        <uc1:MessageUserControl runat="server" ID="MessageUserControl" />

        <%-- List of outstanding orders --%>
        <h3>List of Outstanding Orders</h3>
        <asp:GridView ID="ReceivingVendorOrderGridView"
            DataSourceID="ReceivingVendorOrderDataSource"
            DataKeyNames="PurchaseOrderID"
            runat="server"
            AutoGenerateColumns="False"
            AllowPaging="True"
            OnSelectedIndexChanged="ReceivingVendorOrderGridView_SelectedIndexChanged"
            EmptyDataText="There are no outstanding orders."
            CssClass="table table-hover">
            <Columns>
                <asp:BoundField DataField="PurchaseOrderID" HeaderText="Order" SortExpression="PurchaseOrderID"></asp:BoundField>
                <asp:BoundField DataField="OrderDate" HeaderText="Order Date" SortExpression="OrderDate" DataFormatString="{0:MMM d, yyyy}"></asp:BoundField>
                <asp:BoundField DataField="VendorName" HeaderText="Vendor" SortExpression="VendorName"></asp:BoundField>
                <asp:BoundField DataField="VendorPhone" HeaderText="Contact Phone" SortExpression="VendorPhone" DataFormatString="({0})"></asp:BoundField>
                <asp:CommandField ShowSelectButton="True" SelectText="View Order" ItemStyle-CssClass="text-center"></asp:CommandField>
            </Columns>
        </asp:GridView>
        <asp:ObjectDataSource runat="server"
            ID="ReceivingVendorOrderDataSource"
            OldValuesParameterFormatString="original_{0}"
            SelectMethod="ListOutstandingOrders"
            TypeName="Tools.Framework.BLL.ReceivingController"
            OnSelected="CheckForException"></asp:ObjectDataSource>

        <%-- Receive Items Form --%>
        <div id="ReceivingItemsDiv" runat="server">
            <hr />

            <%-- Vendor information --%>
            <div class="col-md-4 text-center">
                <asp:Label ID="PurchaseOrderIDLabel" runat="server" Text="PurchaseOrderIDLabel" CssClass="order-details-label"></asp:Label>
            </div>
            <div class="col-md-4 text-center">
                <asp:Label ID="VendorNameLabel" runat="server" Text="VendorNameLabel" CssClass="order-details-label"></asp:Label>
            </div>
            <div class="col-md-4 text-center">
                <asp:Label ID="VendorPhoneLabel" runat="server" Text="VendorPhoneLabel" CssClass="order-details-label"></asp:Label>
            </div>

            <%-- Items list --%>
            <div class="col-md-12 order-details-div">
                <asp:GridView ID="ReceivingItemsGridView"
                    DataKeyNames="PurchaseOrderID"
                    runat="server"
                    ItemType="Tools.Framework.Entities.POCOs.ReceivingItems"
                    AutoGenerateColumns="False"
                    EmptyDataText="There are no items to display at this time."
                    CssClass="table table-striped">
                    <Columns>
                        <asp:BoundField DataField="StockItemID" HeaderText="Stock #" SortExpression="StockItemID"></asp:BoundField>
                        <asp:BoundField DataField="StockItemDescription" HeaderText="Description" SortExpression="StockItemDescription"></asp:BoundField>
                        <asp:BoundField DataField="QuantityOrdered" HeaderText="Ordered" SortExpression="QuantityOrdered"></asp:BoundField>
                        <asp:BoundField DataField="QuantityOutstanding" HeaderText="Outstanding" SortExpression="QuantityOutstanding"></asp:BoundField>
                        <asp:TemplateField HeaderText="Receive">
                            <ItemTemplate>
                                <asp:TextBox ID="ReceiveTextBox" runat="server" Text="0" CssClass="quantity-input"></asp:TextBox><br />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Return">
                            <ItemTemplate>
                                <asp:TextBox ID="ReturnTextBox" runat="server" Text="0" CssClass="quantity-input"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Reason">
                            <ItemTemplate>
                                <asp:TextBox ID="ReasonTextBox" runat="server" CssClass="input"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

                <%-- Actions --%>
                <br />
                <asp:Button ID="ReceiveButton" runat="server" Text="Receive" OnClick="ReceiveButton_Click" CssClass="btn btn-primary" /><br />
                <br />

                <asp:Button ID="ForceCloseButton" runat="server" Text="Force Close" CssClass="btn btn-danger" OnClick="ForceCloseButton_Click" />
                <asp:Label ID="ForceCloseReasonLabel" runat="server" Text="Reason:" CssClass="close-reason-label"></asp:Label>
                <asp:TextBox ID="ForceCloseReasonTextBox" runat="server" CssClass="close-reason-input"></asp:TextBox>
            </div>
        </div>
    </div>
    <style>
        .order-details-label {
            font-size: 16px;
            font-weight: bold;
        }

        .order-details-div {
            margin-top: 20px;
        }

        .quantity-input {
            width: 75px;
        }

        .input {
            width: 100%;
        }

        .close-reason-label {
            margin-left: 25px;
        }

        .close-reason-input {
            min-width: 350px;
        }
    </style>
</asp:Content>

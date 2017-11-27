<%@ Page Title="Purchasing" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Purchasing_Default" %>

<%@ Register Src="~/UserControls/MessageUserControl.ascx" TagPrefix="uc1" TagName="MessageUserControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <uc1:MessageUserControl runat="server" ID="MessageUserControl" />
     <div class="row">
        <h2>Purchasing</h2>
        <table style="width: 100%">
        <tr>
            
            <td colspan="8">
                <asp:Label ID="VendorDD" runat="server" Text="Vendor: "></asp:Label>
                <asp:DropDownList ID="VendorDropdown" runat="server" DataSourceID="VendorDataSource" DataTextField="VendorNameAndOrder" DataValueField="VendorID" OnSelectedIndexChanged="VendorDropdown_SelectedIndexChanged"></asp:DropDownList>
                <asp:LinkButton ID="GetSuggestedOrder" runat="server" OnClick="GetSuggestedOrder_Click">Get Suggested Order</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td colspan="8">
            <asp:Label ID="EmployeeDD" runat="server" Text="Employee: "></asp:Label>
            <asp:DropDownList ID="EmployeeDropdown" runat="server" DataSourceID="EmployeeDataSource" DataTextField="Name" DataValueField="ID" AppendDataBoundItems="True"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td style="width: 11%; height: 22px">Purchase Order #:</td>
            <td style="height: 22px; " colspan="4">
                <asp:Label ID="PurchaseOrderDisplay" runat="server" Text=""></asp:Label>
            </td>
            <td style="height: 22px; width: 15%;">
                &nbsp;</td>
            <td style="width: 8%">
                <asp:Label ID="VendorName" runat="server" Text=""></asp:Label>
            </td>
            <td style="height: 22px; width: 10%;">
                <asp:Label ID="VendorLocation" runat="server" Text=""></asp:Label>
            </td>
            <td style="height: 22px; width: 11%;">
                <asp:Label ID="VendorPhone" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="9">
                <asp:GridView ID="SuggestedItems" runat="server" ItemType="Tools.Framework.Entities.POCOs.SuggestedOrderItems" CssClass="table" OnRowCommand="SuggestedItems_RowCommand" OnDataBound="SuggestedItems_DataBound">
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" />
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                        <asp:BoundField DataField="QuantityOnHand" HeaderText="QuantityOnHand" />
                        <asp:BoundField DataField="ReOrderLevel" HeaderText="ReOrderLevel" />
                        <asp:BoundField DataField="QuantityOnOrder" HeaderText="QuantityOnOrder" />
                        <asp:TemplateField HeaderText="PurchaseOrderQuantity">
                            <ItemTemplate>
                                <asp:TextBox ID="PurchaseOrderQuantity" runat="server" Text="<%# Item.PurchaseOrderQuantity %>"/>
                            </ItemTemplate>    
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="$$">
                            <ItemTemplate>
                                <asp:TextBox ID="UnitPrice" runat="server" Text="<%# Item.UnitPrice %>" Enabled="false" ></asp:TextBox>
                            </ItemTemplate>    
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="">
                            <ItemTemplate>
                                <asp:Button ID="RemoveItem" runat="server" CommandName="RemoveOrder" CommandArgument="<%# Item.ID %>" Text="Remove from Order" />
                            </ItemTemplate>
                         </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
       </tr>
        <tr>
            <td style="width: 11%">&nbsp;</td>
            <td style="width: 5%">&nbsp;</td>
            <td style="width: 7%">&nbsp;</td>
            <td style="width: 31%; text-align: right;">Subtotal:</td>
            <td style="width: 34px; text-align: right">
                <asp:Label ID="subtotalAmount" runat="server" Text="$0.00"></asp:Label>
            </td>
            <td style="width: 15%; text-align: right;">GST:</td>
            <td style="width: 8%">
                <asp:Label ID="gstAmount" runat="server" Text="$0.00"></asp:Label>
            </td>
            <td style="width: 10%; text-align: right;">Total:</td>
            <td style="width: 11%">
                <asp:Label ID="totalAmount" runat="server" Text="$0.00"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="5">&nbsp;</td>
            <td style="width: 10%" class="text-center">
                <asp:LinkButton ID="StartOrderLink" runat="server" Text="" OnClick="StartOrderLink_Click">Start Order</asp:LinkButton>
            </td>


            <td style="width: 15%" class="text-center">
                <asp:LinkButton ID="UpdateOrderLink" runat="server" Text="" OnClick="UpdateOrderLink_Click"/>
            </td>
            <td style="width: 8%" class="text-center">
                <asp:LinkButton ID="DeleteOrderLink" runat="server" OnClick="DeleteOrderLink_Click">Delete Order</asp:LinkButton>
            </td>
            <td style="width: 10%" class="text-center">
                <asp:LinkButton ID="PlaceOrderLink" runat="server" OnClick="PlaceOrderLink_Click">Place Order</asp:LinkButton>
            </td>
            <td class="text-center" style="width: 11%">
                <asp:LinkButton ID="ClearOrderLink" runat="server" OnClick="ClearOrderLink_Click">Clear</asp:LinkButton>
            </td>
        </tr>

    </table>
        <asp:GridView ID="NonSuggestedItems" runat="server" ItemType="Tools.Framework.Entities.POCOs.NonSuggestedOrderItems" CssClass="table" OnRowCommand="NonSuggestedItems_RowCommand" AllowPaging="true">
            <Columns>
                <asp:BoundField Datafield="ID" HeaderText="ID" />
                <asp:BoundField Datafield="Description" HeaderText="Description" />
                <asp:BoundField DataField="QuantityOnHand" HeaderText="QOH" />
                <asp:BoundField DataField="ReOrderLevel" HeaderText="ROL" />
                <asp:BoundField DataField="QuantityOnOrder" HeaderText="QOO" />
                <asp:BoundField DataField="Buffer" HeaderText="Buffer" />
                <asp:BoundField DataField="PurchasePrice" HeaderText="$$" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Button ID="AddToOrder" runat="server" CommandName="AddToOrder" CommandArgument="<%# Item.ID %>" Text="Add To Order" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView> 
    </div>

    <asp:ObjectDataSource runat="server" ID="VendorDataSource" OldValuesParameterFormatString="original_{0}" SelectMethod="ListVendors" TypeName="Tools.Framework.BLL.PurchasingController"></asp:ObjectDataSource>
    <asp:ObjectDataSource runat="server" ID="EmployeeDataSource" OldValuesParameterFormatString="original_{0}" SelectMethod="ListEmployees" TypeName="Tools.Framework.BLL.PurchasingController"></asp:ObjectDataSource>
</asp:Content>
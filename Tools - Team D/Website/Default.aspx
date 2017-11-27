<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-12">
            <asp:Image ID="Logo" runat="server" ImageUrl="~/Images/logo.png" AlternateText="Team D logo" />
        </div>
        <div class="col-md-6">
            <h2>Members & Responsibilities</h2>
            <h4>Danilo Durso</h4>
            <ul>
                <li>Solution setup (website, class library, entities, classes, and connections)</li>
                <li>Security</li>
                <li>Receiving Page</li>
            </ul>
            <h4>Leban Mohamed</h4>
            <ul>
                <li>Pages and folders creation</li>
                <li>User controls and validation messages for entities' properties</li>
                <li>Purchasing Page</li>
            </ul>
        </div>
        <div class="col-md-6">
            <h2>Known Bugs</h2>
        </div>
    </div>

    <style type="text/css">
        h2 { margin-bottom: 25px; }
        div.row { margin-top: 25px; }
    </style>
</asp:Content>
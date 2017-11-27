<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="About.aspx.cs" Inherits="About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <h2>About Us</h2>
        <div class="col-md-12">
            <h3>Members</h3>
            <div class="member">
                <asp:Image ID="Danilo" AlternateText="danilo" runat="server" ImageUrl="~/Images/danilo.png" />
                <h4>Danilo Durso</h4>
            </div>
            <div>
                <asp:Image ID="Leban" AlternateText="leban" runat="server" ImageUrl="~/Images/leban.png" />
                <h4>Leban Mohamed</h4>
            </div>
        </div>
        <div class="col-md-12">
            <h3>Security Roles</h3>
            <ul>
                <li><strong>WebsiteAdmins</strong> - Access to all pages</li>
                <li><strong>RegisteredUsers</strong> - Access to all pages except Purchasing, Receiving, and pages in the Admin folder</li>
                <li><strong>Staff</strong> - Access to all pages except in the Admin folder</li>
            </ul>
            <p>The default password for users is <em>Pa$$word1</em>.</p>
        </div>
        <div class="col-md-12">
            <h3>Connection String for Development</h3>
            <pre><code>&lt;connectionStrings&gt;
    &lt;add name="DefaultConnection" connectionString="Data Source=.;Initial Catalog=eTools;Integrated Security=true;" providerName="System.Data.SqlClient" /&gt;
    &lt;add name="ToolsDb" connectionString="Data Source=.;Initial Catalog=eTools;Integrated Security=true;" providerName="System.Data.SqlClient" /&gt;
&lt;/connectionStrings&gt;</code></pre>
        </div>
    </div>

    <style type="text/css">
        div.member {
            float: left;
            margin-right: 15px;
            width: 150px;
        }
    </style>
</asp:Content>

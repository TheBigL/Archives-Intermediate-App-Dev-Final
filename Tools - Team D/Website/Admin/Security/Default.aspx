<%@ Page Title="Site Administration" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin_Security_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="row"">
        <h2>Site Administration</h2>
        <div class="col-md-12" style="margin-top:15px;">
            <!-- Nav tabs -->
            <ul class="nav nav-tabs">
                <li class="active"><a href="#users" data-toggle="tab">Users</a></li>
                <li><a href="#roles" data-toggle="tab">Security Roles</a></li>
                <li><a href="#unregistered" data-toggle="tab">Unregistered Users</a></li>
            </ul>
            <div class="tab-content">
                <div class="tab-pane fade in active" id="users">
                    <asp:ListView ID="UserListView" runat="server"
                        DataSourceID="UserProfileDataSource"
                        ItemType="Tools.Framework.Entities.Security.UserProfile"
                        DataKeyNames="UserId" InsertItemPosition="LastItem"
                        OnItemInserting="UserListView_ItemInserting"
                        OnItemDeleted="RefreshAll" OnItemInserted="RefreshAll">
                        <LayoutTemplate>
                            <div class="row bg-info">
                                <div class="col-sm-2 h4">Action</div>
                                <div class="col-sm-2 h4">User Name</div>
                                <div class="col-sm-5 h4">Profile</div>
                                <div class="col-sm-3 h4">Roles</div>
                            </div>
                            <div runat="server" id="itemPlaceholder"></div>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <div class="row">
                                <div class="col-sm-2">
                                    <asp:LinkButton runat="server" CommandName="Delete" Text="Delete" ID="DeleteButton" />
                                </div>
                                <div class="col-sm-2"><%# Item.UserName %></div>
                                <div class="col-sm-5">
                                    <%# Item.Email %>
                                    <%# Item.FirstName + " " + Item.LastName %>
                                </div>
                                <div class="col-sm-3">
                                    <asp:Repeater ID="RoleUserRepeater" runat="server"
                                        DataSource="<%# Item.RoleMemberships %>" ItemType="System.String">
                                        <ItemTemplate><%# Item %></ItemTemplate>
                                        <SeparatorTemplate>, </SeparatorTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </ItemTemplate>
                        <InsertItemTemplate>
                            <hr />
                            <div class="row">
                                <div class="col-sm-2">
                                    <asp:LinkButton runat="server" CommandName="Insert" Text="Insert" ID="InsertButton" />
                                    <asp:LinkButton runat="server" CommandName="Cancel" Text="Clear" ID="CancelButton" />
                                </div>
                                <div class="col-sm-2">
                                    <asp:TextBox Text='<%# BindItem.UserName %>' runat="server" ID="UserNameTextBox" placeholder="User Name" />
                                </div>
                                <div class="col-sm-5">
                                    <asp:TextBox Text='<%# BindItem.Email %>' runat="server" ID="EmailTextBox" placeholder="Email" TextMode="Email" />
                                </div>
                                <div class="col-sm-3">
                                    <asp:CheckBoxList ID="RoleMemberships" runat="server" DataSource="<%# GetRoleNames() %>"></asp:CheckBoxList>
                                </div>
                            </div>
                        </InsertItemTemplate>
                    </asp:ListView>
                    <asp:ObjectDataSource ID="UserProfileDataSource" runat="server" DataObjectTypeName="Tools.Framework.Entities.Security.UserProfile" DeleteMethod="RemoveUser" InsertMethod="AddUser" OldValuesParameterFormatString="original_{0}" SelectMethod="ListAllUsers" TypeName="Tools.Framework.BLL.Security.UserManager"></asp:ObjectDataSource>
                </div>
                <div class="tab-pane fade" id="roles">
                    <asp:ListView ID="RoleListView" runat="server" DataSourceID="RoleDataSource" 
                         InsertItemPosition="LastItem" DataKeyNames="RoleId" 
                         ItemType="Tools.Framework.Entities.Security.RoleProfile"
                         OnItemDeleted="RefreshAll" OnItemInserted="RefreshAll">
                        <EmptyDataTemplate>
                            <span>No security roles have been set up.</span>
                        </EmptyDataTemplate>
                        <LayoutTemplate>
                            <div class="row bg-info">
                                <div class="col-sm-3 h4">Action</div>
                                <div class="col-sm-3 h4">Role</div>
                                <div class="col-sm-6 h4">Members</div>
                            </div>
                            <div runat="server" id="itemPlaceholder"></div>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <div class="row">
                                <div class="col-sm-3"><asp:LinkButton runat="server" CommandName="Delete" Text="Delete" ID="DeleteButton" /></div>
                                <div class="col-sm-3">
                                    <%# Item.RoleName %>
                                </div>
                                <div class="col-sm-6">
                                    <asp:Repeater ID="RoleUserRepeater" runat="server"
                                            DataSource="<%# Item.UserNames %>" ItemType="System.String">
                                        <ItemTemplate><%# Item %></ItemTemplate>
                                        <SeparatorTemplate>, </SeparatorTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </ItemTemplate>
                        <InsertItemTemplate>
                            <hr />
                            <div class="row">
                                <div class="col-sm-3">
                                    <asp:LinkButton runat="server" CommandName="Insert" Text="Insert" ID="InsertButton" />
                                    <asp:LinkButton runat="server" CommandName="Cancel" Text="Clear" ID="CancelButton" />
                                </div>
                                <div class="col-sm-3">
                                    <asp:TextBox Text='<%# BindItem.RoleName %>' runat="server" ID="RoleNameTextBox" placeholder="Role Name" />
                                </div>
                            </div>
                        </InsertItemTemplate>
                    </asp:ListView>
                    <asp:ObjectDataSource runat="server" ID="RoleDataSource" DataObjectTypeName="Tools.Framework.Entities.Security.RoleProfile" DeleteMethod="RemoveRole" InsertMethod="AddRole" OldValuesParameterFormatString="original_{0}" SelectMethod="ListAllRoles" TypeName="Tools.Framework.BLL.Security.RoleManager"></asp:ObjectDataSource>
                </div>
                <div class="tab-pane fade" id="unregistered">
                    <asp:GridView ID="UnregisteredUsersGridView" runat="server" 
                         CssClass="table table-hover" AutoGenerateColumns="False" 
                         DataSourceID="UnregisteredUserDataSource"
                         ItemType="Tools.Framework.BLL.Security.UnregisteredUser" 
                         AllowPaging="True" DataKeyNames="Id"
                         OnSelectedIndexChanging="UnregisteredUsersGridView_SelectedIndexChanging">
                        <Columns>
                            <asp:CommandField ShowSelectButton="True"></asp:CommandField>
                            <asp:BoundField DataField="UserType" HeaderText="User Type" SortExpression="UserType"></asp:BoundField>
                            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name"></asp:BoundField>
                            <asp:BoundField DataField="OtherName" HeaderText="Other Name" SortExpression="OtherName"></asp:BoundField>
                            <asp:TemplateField HeaderText="Assigned UserName" SortExpression="AssignedUserName">
                                <ItemTemplate>
                                    <asp:TextBox runat="server" Text='<%# Bind("AssignedUserName") %>' ID="GivenUserName"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Assigned Email" SortExpression="AssignedEmail">
                                <ItemTemplate>
                                    <asp:TextBox runat="server" Text='<%# Bind("AssignedEmail") %>' ID="GivenEmail"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:ObjectDataSource runat="server" ID="UnregisteredUserDataSource" OldValuesParameterFormatString="original_{0}" SelectMethod="ListAllUnregsiteredUsers" TypeName="Tools.Framework.BLL.Security.UserManager"></asp:ObjectDataSource>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<%@ Application Language="C#" %>
<%@ Import Namespace="Website" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="Tools.Framework.BLL.Security" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e)
    {
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);

        // Ensure the default roles are set up
        var RoleManager = new RoleManager();
        RoleManager.AddStartupRoles();

        // Ensure there is a Webmaster account
        var UserManager = new UserManager();
        UserManager.AddWebMaster();
    }

</script>

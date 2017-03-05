<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebhostV2.Login" %>

<%@ Register src="ADLoginControl.ascx" tagname="ADLoginControl" tagprefix="uc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Mobile/mobile.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <uc1:ADLoginControl ID="ADLoginControl1" runat="server" />
        <hr />
        <asp:Label ID="BrowserMessage" runat="server"></asp:Label>    
    </div>
    </form>
    <script>
        background();
    </script>
</body>
</html>

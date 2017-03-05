<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StandardNavigationControls.ascx.cs" Inherits="WebhostV2.UserControls.StandardNavigationControls" %>

<%@ Register src="TagLinkPanel.ascx" tagname="TagLinkPanel" tagprefix="uc1" %>

<nav class="control">
    <asp:Panel ID="LinkPanel" runat="server">
        <asp:HyperLink ID="HomeLink" runat="server">Home</asp:HyperLink>
        <asp:HyperLink ID="WeekendLink" runat="server" Text="Weekend"></asp:HyperLink>
        <asp:HyperLink ID="CommentLink" runat="server">Comment Letters</asp:HyperLink>
        <asp:HyperLink ID="CourseRequestLink" runat="server">Course Request</asp:HyperLink>
        <asp:HyperLink ID="DeviceRegistration" runat="server">Register Wifi Device</asp:HyperLink>
    </asp:Panel>
</nav>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TagLinkPanel.ascx.cs" Inherits="WebhostV2.UserControls.TagLinkPanel" %>
<asp:ScriptManagerProxy ID="TagLinkPanelSMP" runat="server"></asp:ScriptManagerProxy>
<asp:HiddenField ID="TagIdField" runat="server" />
<asp:Panel ID="TitlePanel" runat="server">
    <header>
        <asp:Label ID="txtLabel" runat="server" Text=""></asp:Label>
        <asp:Label ID="TitleLabel" runat="server"></asp:Label></header>
</asp:Panel>
<nav class="colapse">
    <asp:Panel ID="LinkPanel" runat="server"></asp:Panel>
</nav>
<ajaxToolkit:CollapsiblePanelExtender ID="TagPanelColapser" runat="server" AutoCollapse="False" AutoExpand="False" Collapsed="True" CollapseControlID="TitlePanel" ExpandControlID="TitlePanel" TargetControlID="LinkPanel" TextLabelID="txtLabel" CollapsedText="> " ExpandedText="^ "></ajaxToolkit:CollapsiblePanelExtender>

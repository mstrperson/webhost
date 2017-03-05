<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentScheduleColapsePanel.ascx.cs" Inherits="WebhostV2.UserControls.StudentScheduleColapsePanel" %>
<asp:ScriptManagerProxy ID="sscpsmp" runat="server"></asp:ScriptManagerProxy>
<asp:HiddenField ID="StudentIdField" runat="server" />
<article class="sub_control">
    <header>
        <asp:Panel ID="TitlePanel" runat="server">
            <asp:Label ID="textLabel" runat="server" Text="Label"></asp:Label>
            <asp:Label ID="StudentNameLabel" runat="server" Text="Label"></asp:Label>
        </asp:Panel>
    </header>
    <asp:Panel ID="SchedulePanel" runat="server">
        <asp:Table CssClass="bordered" ID="ScheduleTable" runat="server"></asp:Table>
    </asp:Panel>
    <ajaxToolkit:CollapsiblePanelExtender ID="SchedulePanelCollapsibleExtender" runat="server" 
        TextLabelID="textLabel" CollapseControlID="TitlePanel" ExpandControlID="TitlePanel" TargetControlID="SchedulePanel"
        Collapsed="true" AutoCollapse="false" AutoExpand="false" CollapsedText=">" ExpandedText="^"></ajaxToolkit:CollapsiblePanelExtender>
</article>

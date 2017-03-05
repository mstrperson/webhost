<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WeekendSelector.ascx.cs" Inherits="WebhostV2.UserControls.WeekendSelector" %>
<asp:ScriptManagerProxy ID="WS_SMP" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="WeekendSelectorUpdate" runat="server" OnLoad="WeekendSelectorUpdate_Load">
    <ContentTemplate>
        <article class="sub_control">
            <asp:HiddenField ID="WeekendIDField" runat="server" />
            Weekend of <asp:TextBox ID="StartDate" runat="server" AutoPostBack="True"></asp:TextBox>
            <ajaxToolkit:CalendarExtender ID="StartDate_CalendarExtender" runat="server" PopupPosition="Right" TargetControlID="StartDate" TodaysDateFormat="mm/dd/yyyy">
            </ajaxToolkit:CalendarExtender>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>

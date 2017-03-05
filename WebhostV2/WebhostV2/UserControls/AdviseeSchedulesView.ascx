<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdviseeSchedulesView.ascx.cs" Inherits="WebhostV2.UserControls.AdviseeSchedulesView" %>
<asp:ScriptManagerProxy ID="AdviseeScheduelSMP" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="AdviseeSchedulePanel" runat="server">
    <ContentTemplate>
        <article class="control">
            <asp:Table ID="AdviseeTable" runat="server"></asp:Table>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>

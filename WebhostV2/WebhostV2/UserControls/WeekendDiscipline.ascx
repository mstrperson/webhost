<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WeekendDiscipline.ascx.cs" Inherits="WebhostV2.UserControls.WeekendDiscipline" %>
<%@ Register src="StudentGroupSelector.ascx" tagname="StudentGroupSelector" tagprefix="uc1" %>
<%@ Register Src="~/UserControls/WeekendSelector.ascx" TagPrefix="uc1" TagName="WeekendSelector" %>

<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="DisciplinePanel" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="IDField" runat="server" />
        <article class="control">
            <asp:Button ID="LoadBtn" runat="server" OnClick="LoadBtn_Click" Text="Load Weekend Lists" />
            <asp:Button ID="SaveBtn" runat="server" OnClick="SaveBtn_Click" Text="Save Changes" Visible="False" />
            <article class="sub_control">
                <header>Detention List</header>
                <hr />
                <uc1:StudentGroupSelector ID="DetentionListSelector" runat="server" Visible="False" />
            </article>
            <article class="sub_control">
                <header>Campused List</header>
                <hr />
                <uc1:StudentGroupSelector ID="CampusedListSelector" runat="server" Visible="False" />
            </article>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>

<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="DisciplinePanel">
    <ProgressTemplate>
            <div class="progress_container">
        <article class="progress">
            Working...
        </article></div>
    </ProgressTemplate>
</asp:UpdateProgress>



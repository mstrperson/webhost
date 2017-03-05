<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="DTL.aspx.cs" Inherits="WebhostV2.DTL" %>

<%@ Register Src="~/UserControls/DutyRosterEditor.ascx" TagPrefix="uc1" TagName="DutyRosterEditor" %>

<%@ Register src="UserControls/WeekendBuilder.ascx" tagname="WeekendBuilder" tagprefix="uc2" %>

<%@ Register src="UserControls/RosterEditor.ascx" tagname="RosterEditor" tagprefix="uc4" %>

<%@ Register src="UserControls/WeekendDiscipline.ascx" tagname="WeekendDiscipline" tagprefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    Duty Team Leaders
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
    <nav class="control">
        <asp:RadioButtonList ID="DutyTeamSelectList" runat="server">
        </asp:RadioButtonList>
        <hr />
        <asp:Button ID="LoadDutyTeam" runat="server" Text="Load Duty Team" OnClick="LoadDutyTeam_Click" />
        <hr />

        <asp:RadioButtonList ID="JobSelectList" runat="server">
            <asp:ListItem Value="0">Edit Teams</asp:ListItem>
            <asp:ListItem Value="1" Selected="True">Weekend Schedule</asp:ListItem>
            <asp:ListItem Value="2">Detention Lists</asp:ListItem>
        </asp:RadioButtonList>
        <hr />
        <asp:Button ID="OpenJob" runat="server" Width="100%" Text="Load Job" OnClick="OpenJob_Click" />
        <hr />
        <asp:Button ID="DownloadSchedule" runat="server" Text="Download PDF Schedule" OnClick="DownloadSchedule_Click" Width="100%" />
    </nav>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <asp:UpdatePanel ID="MainContent" runat="server">
        <ContentTemplate>
            <uc1:DutyRosterEditor ID="DutyRosterEditor1" runat="server" Visible="False" />
            <uc3:WeekendDiscipline ID="WeekendDiscipline1" runat="server" Visible="False" />
            <uc2:WeekendBuilder ID="WeekendBuilder1" runat="server" Visible="True" />
        </ContentTemplate>
    </asp:UpdatePanel>
    </asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

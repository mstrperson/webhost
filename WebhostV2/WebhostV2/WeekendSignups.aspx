<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="WeekendSignups.aspx.cs" Inherits="WebhostV2.WeekendSignups" %>
<%@ Register src="UserControls/StandardNavigationControls.ascx" tagname="StandardNavigationControls" tagprefix="uc1" %>
<%@ Register src="UserControls/StudentSignupSelector.ascx" tagname="StudentSignupSelector" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    Weekend Signups
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <uc2:StudentSignupSelector ID="StudentSignupSelector1" runat="server" />
    <asp:Panel ID="NotAvailablePanel" Visible="false" runat="server">
        <article class="control">
            <header>Signups are not Available Yet.</header>
            Signups will be available at 11:30 on Friday.
            <hr />
            <asp:Table ID="ViewTable" runat="server" Width="100%" BorderStyle="Outset" GridLines="Horizontal"></asp:Table>
        </article>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

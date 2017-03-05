<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Import.aspx.cs" Inherits="WebhostV2.Import1" %>
<%@ Register src="UserControls/BlackbaudImports.ascx" tagname="BlackbaudImports" tagprefix="uc1" %>
<%@ Register src="UserControls/StandardNavigationControls.ascx" tagname="StandardNavigationControls" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    Import Stuff!
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
    <nav class="control">
        <header>Other Tools</header>
        <asp:Button ID="UpdateFacultyCommonsGroupsBtn" runat="server" Text="Update Faculty and Commons Groups" OnClick="UpdateFacultyCommonsGroupsBtn_Click" />
        <asp:Button ID="GoogleCalendarUpdateBtn" runat="server" Text="Update Google Calendars" OnClick="GoogleCalendarUpdateBtn_Click" />
    </nav>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">

    <uc1:BlackbaudImports ID="BlackbaudImports1" runat="server" />

</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="CreditReports.aspx.cs" Inherits="WebhostV2.CreditReports" %>
<%@ Register src="UserControls/StudentCreditReport.ascx" tagname="StudentCreditReport" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    Credit Reports
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
    <nav class="control">
        <header>Select Student</header>
        <ajaxToolkit:ComboBox ID="StudentComboBox" AutoCompleteMode="SuggestAppend" runat="server"></ajaxToolkit:ComboBox>
        <asp:Button ID="SelectBtn" runat="server" Text="Load Credits" OnClick="SelectBtn_Click" />
        <header>Exports</header>
        <asp:Button ID="CreditAuditReportBtn" runat="server" Text="Credit Audit" OnClick="CreditAuditReportBtn_Click" />
    </nav>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <uc1:StudentCreditReport ID="StudentCreditReport1" runat="server" ReadOnly="false"/>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

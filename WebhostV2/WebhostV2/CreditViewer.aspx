<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="CreditViewer.aspx.cs" Inherits="WebhostV2.CreditViewer" %>

<%@ Register Src="~/UserControls/StudentCreditReport.ascx" TagPrefix="uc1" TagName="StudentCreditReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
    <nav class="control">
        <header>Select Advisee</header>
        <ajaxToolkit:ComboBox ID="StudentComboBox" AutoCompleteMode="SuggestAppend" runat="server"></ajaxToolkit:ComboBox>
        <asp:Button ID="SelectBtn" runat="server" Text="Load Credits" OnClick="SelectBtn_Click" />
    </nav>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <uc1:StudentCreditReport runat="server" ID="StudentCreditReport1" ReadOnly="true" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

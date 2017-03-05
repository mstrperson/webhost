<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="GradeReportView.aspx.cs" Inherits="WebhostV2.GradeReportView" %>
<%@ Register src="UserControls/GradeReportDisplay.ascx" tagname="GradeReportDisplay" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    Grade Reports
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <nav class="control">
                <header>Student</header>
                <ajaxToolkit:ComboBox ID="StudentSelectBox" runat="server" AutoCompleteMode="SuggestAppend"></ajaxToolkit:ComboBox>
                <asp:Button runat="server" ID="SelectBtn" Text="Generate Student Grade Report" OnClick="SelectBtn_Click" />
            </nav>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <uc1:GradeReportDisplay ID="GradeReportDisplay1" runat="server" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="CommentArchive.aspx.cs" Inherits="WebhostV2.CommentArchive" %>
<%@ Register src="UserControls/CommentEditor.ascx" tagname="CommentEditor" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
        Comment Letter Archive
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
    <nav class="control">
        <header>Select Term and Year</header>
        <asp:DropDownList ID="TermSelectDDL" runat="server"></asp:DropDownList>
        <asp:Button ID="LoadTerm" runat="server" Text="Load Term" OnClick="LoadTerm_Click" />
    </nav>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    
    <uc1:CommentEditor ID="CommentEditor1" runat="server" />
    
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

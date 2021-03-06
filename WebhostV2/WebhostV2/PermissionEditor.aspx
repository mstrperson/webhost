﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="PermissionEditor.aspx.cs" Inherits="WebhostV2.PermissionEditor" %>
<%@ Register src="UserControls/PermissionEditor.ascx" tagname="PermissionEditor" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    Permission Editor
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
    <nav class="control">
        <header>Select Permission</header>
        <ajaxToolkit:ComboBox ID="PermissionSelectionCB" runat="server"></ajaxToolkit:ComboBox>
        <asp:Button ID="SelectPermissionBtn" runat="server" Text="Edit Permission" OnClick="SelectPermissionBtn_Click" />
    </nav>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <uc1:PermissionEditor ID="PermissionEditor1" runat="server" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

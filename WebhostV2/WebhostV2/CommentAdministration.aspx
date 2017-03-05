<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="CommentAdministration.aspx.cs" Inherits="WebhostV2.CommentAdministration" %>

<%@ Register Src="~/UserControls/CommentAdmin.ascx" TagPrefix="uc1" TagName="CommentAdmin" %>

<%@ Register src="UserControls/StandardNavigationControls.ascx" tagname="StandardNavigationControls" tagprefix="uc2" %>
<%@ Register src="UserControls/CommentEditor.ascx" tagname="CommentEditor" tagprefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
    <uc1:CommentAdmin runat="server" id="CommentAdmin1" OnMasquerade="CommentAdmin1_Masquerade" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <uc3:CommentEditor ID="CommentEditor1" runat="server" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

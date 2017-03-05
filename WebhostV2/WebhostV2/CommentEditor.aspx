<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="CommentEditor.aspx.cs" Inherits="WebhostV2.CommentEditor" %>
<%@ Register src="UserControls/StandardNavigationControls.ascx" tagname="StandardNavigationControls" tagprefix="uc1" %>
<%@ Register src="UserControls/CommentEditor.ascx" tagname="CommentEditor" tagprefix="uc2" %>
<%@ Register Src="~/UserControls/DepartmentalComments.ascx" TagPrefix="uc1" TagName="DepartmentalComments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    Comment Letter Editor
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
    <uc1:DepartmentalComments runat="server" id="DepartmentalComments1" Visible="false"/>
    <nav class="control">
        <details>
            <summary>Signature</summary>
            <asp:FileUpload ID="SignatureUpload" runat="server" /><asp:Button ID="SaveSigBtn" runat="server" Text="Save Signature Image" OnClick="SaveSigBtn_Click" />
        </details>
    </nav>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <uc2:CommentEditor ID="CommentEditor1" runat="server" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="CreditCreator.aspx.cs" Inherits="WebhostV2.CreditCreator" %>

<%@ Register Src="~/UserControls/CreditBuilder.ascx" TagPrefix="uc1" TagName="CreditBuilder" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    Create New Transfer or Waiver Credits
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <uc1:CreditBuilder runat="server" id="CreditBuilder" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

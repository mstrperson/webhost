<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="LivingLevels.aspx.cs" Inherits="WebhostV2.LivingLevels" %>

<%@ Register Src="~/UserControls/LivingLevelsBuilder.ascx" TagPrefix="uc1" TagName="LivingLevelsBuilder" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    Living Levels
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <uc1:LivingLevelsBuilder runat="server" ID="LivingLevelsBuilder1" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

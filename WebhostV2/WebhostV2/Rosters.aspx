<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Rosters.aspx.cs" Inherits="WebhostV2.Rosters" %>
<%@ Register src="UserControls/RosterEditor.ascx" tagname="RosterEditor" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    Roster Editing
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
    <nav class="control">
        <asp:HiddenField ID="TermIdField" runat="server" />
        <asp:DropDownList ID="TermSelectDDL" runat="server" AutoPostBack="True" OnSelectedIndexChanged="TermSelectDDL_SelectedIndexChanged">
            <asp:ListItem>Summer</asp:ListItem>
            <asp:ListItem>Fall</asp:ListItem>
            <asp:ListItem>Winter</asp:ListItem>
            <asp:ListItem>Spring</asp:ListItem>
        </asp:DropDownList>
        <asp:DropDownList ID="SectionSelectList" runat="server">
        </asp:DropDownList>
        <asp:Button ID="LoadSection" runat="server" OnClick="LoadSection_Click" Text="Open Section" />
        <asp:Button ID="CleanBtn" runat="server" OnClick="CleanBtn_Click" Text="Clean Bad Sections" />
    </nav>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <uc1:RosterEditor ID="RosterEditor1" runat="server" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

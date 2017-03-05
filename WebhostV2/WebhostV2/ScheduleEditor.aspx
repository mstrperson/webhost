<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="ScheduleEditor.aspx.cs" Inherits="WebhostV2.ScheduleEditor" %>
<%@ Register src="UserControls/ScheduleView.ascx" tagname="ScheduleView" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
    <nav class="control">
        <asp:DropDownList ID="StudentSelectDDL" runat="server"></asp:DropDownList>
        <asp:Button ID="StudentSelectBtn" runat="server" Text="Load Student Schedule" OnClick="StudentSelectBtn_Click" />
        <hr />
        <asp:DropDownList ID="FacultySelectDDL" runat="server"></asp:DropDownList>
        <asp:Button ID="FacultySelectBtn" runat="server" Text="Load Teacher Schedule" OnClick="FacultySelectBtn_Click" />
    </nav>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <uc2:ScheduleView ID="ScheduleView1" runat="server" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

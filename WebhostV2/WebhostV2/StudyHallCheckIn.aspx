<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="StudyHallCheckIn.aspx.cs" Inherits="WebhostV2.StudyHallCheckIn" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    Study Hall Check In
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
    <nav class="control">
        <header>Location</header>
        <asp:DropDownList ID="StudyHallLocationSelector" runat="server"></asp:DropDownList>
        <asp:Button ID="LoadStudyHallBtn" runat="server" Text="Start Check In" OnClick="LoadStudyHallBtn_Click" />
    </nav>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <article class="control">
        <header>
            <asp:Label ID="StudyHallLabel" runat="server" Text="Study Hall"></asp:Label></header>
        <asp:Table ID="CheckInTable" runat="server"></asp:Table>
    </article>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

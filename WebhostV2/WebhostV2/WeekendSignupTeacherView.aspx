<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="WeekendSignupTeacherView.aspx.cs" Inherits="WebhostV2.WeekendSignupTeacherView" %>

<%@ Register Src="~/UserControls/StudentSignupManager.ascx" TagPrefix="uc1" TagName="StudentSignupManager" %>
<%@ Register Src="~/UserControls/WeekendTripAttendanceList.ascx" TagPrefix="uc1" TagName="WeekendTripAttendanceList" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    Weekend Signups
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
    <nav class="control">
        <asp:DropDownList ID="ActivitySelectDDL" runat="server"></asp:DropDownList>
        <asp:Button ID="LoadActivityBtn" runat="server" Text="Show Signpus" OnClick="LoadActivityBtn_Click" />
        <hr />
        <asp:Button ID="DownloadSchedule" runat="server" Text="Download Weekend Schedule" OnClick="DownloadSchedule_Click" />
    </nav>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <uc1:WeekendTripAttendanceList runat="server" id="WeekendTripAttendanceList1" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

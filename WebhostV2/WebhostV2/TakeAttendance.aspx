<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="TakeAttendance.aspx.cs" Inherits="WebhostV2.TakeAttendance" %>
<%@ Register src="UserControls/StandardNavigationControls.ascx" tagname="StandardNavigationControls" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    Take Attendance
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
    <nav class="control">
        <header>Select Class</header>
        <ajaxToolkit:ComboBox ID="ClassSelectCB" runat="server"></ajaxToolkit:ComboBox>
        <asp:Button ID="LoadClassBtn" runat="server" Text="Take Attendance" OnClick="LoadClassBtn_Click" />
    </nav>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <asp:UpdatePanel ID="AttendanceUpdatePanel" runat="server">
            <ContentTemplate>
                <article class="control">
                    <header><asp:Label runat="server" ID="ClassNameLabel"></asp:Label></header>
                    <asp:CheckBox ID="TodayCB" runat="server" Checked="True" Text="Today" AutoPostBack="True" OnCheckedChanged="TodayCB_CheckedChanged" /><asp:Label runat="server" ID="SubmittedLabel"></asp:Label>
                    <asp:TextBox ID="DateInput" Visible="false" runat="server"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender ID="DateInput_CalendarExtender" runat="server" TargetControlID="DateInput">
                    </ajaxToolkit:CalendarExtender>
                    <asp:Button ID="DateSelectBtn" CssClass="bordered" runat="server" Text="Change Date" Visible="false" OnClick="DateSelectBtn_Click"/><asp:Button ID="SubmitBtn" runat="server" Text="Submit Attendance" Enabled="false" OnClick="SubmitBtn_Click" />
                    <asp:Table runat="server" ID="AttendanceTable" Width="100%" HorizontalAlign="Center" CssClass="bordered" GridLines="Horizontal"></asp:Table>
                </article>
            </ContentTemplate>
        </asp:UpdatePanel>
    <asp:UpdateProgress ID="AttendanceUP" runat="server">
        <ProgressTemplate>
            <div class="progress_container">
            <article class="progress">
                Working....
            </article></div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

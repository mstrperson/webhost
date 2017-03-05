<%@ Page Title="" Language="C#" MasterPageFile="~/Mobile/Mobile.Master" AutoEventWireup="true" CodeBehind="Attendance.aspx.cs" Inherits="WebhostV2.Mobile.Attendance" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    Attendance
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="NavPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainPlaceHolder" runat="server">
    <asp:UpdatePanel ID="AttendanceUpdatePanel" runat="server">
        <ContentTemplate>
            <article class="control">
                <header><asp:Label runat="server" ID="ClassNameLabel"></asp:Label></header>
                <asp:CheckBox ID="TodayCB" runat="server" Checked="True" Text="Today" AutoPostBack="True" OnCheckedChanged="TodayCB_CheckedChanged" /><asp:Label runat="server" ID="SubmittedLabel"></asp:Label>
                <asp:TextBox ID="DateInput" Visible="false" runat="server"></asp:TextBox>
                
                <ajaxToolkit:CalendarExtender ID="DateInput_CalendarExtender" runat="server" TargetControlID="DateInput">
                </ajaxToolkit:CalendarExtender>
                <br />
                <asp:Button ID="DateSelectBtn" CssClass="bordered" runat="server" Text="Change Date" Visible="false" OnClick="DateSelectBtn_Click"/><asp:Button ID="SubmitBtn" runat="server" Text="Submit Attendance" Enabled="false" OnClick="SubmitBtn_Click" />
                <asp:Table runat="server" ID="AttendanceTable" Width="100%" HorizontalAlign="Center" CssClass="bordered" GridLines="Horizontal"></asp:Table>
                <br />
                <asp:Button ID="SubmitBtn0" runat="server" Enabled="false" OnClick="SubmitBtn_Click" Text="Submit Attendance" />
            </article>
            <article class="control">
                <header>Select Class</header>
                <asp:RadioButtonList ID="ClassSelectCB" runat="server" RepeatLayout="Table" CssClass="bordered"></asp:RadioButtonList><br />
                <asp:Button ID="LoadClassBtn" runat="server" Text="Take Attendance" OnClick="LoadClassBtn_Click" />
            </article>
            <asp:Panel runat="server" ID="WarningDialog" Visible="false">
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="AttendanceUP" runat="server" AssociatedUpdatePanelID="AttendanceUpdatePanel">
        <ProgressTemplate>
            <div id="progress_container">
                <article class="progress">
                    Working....
                </article>
            </div>            
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

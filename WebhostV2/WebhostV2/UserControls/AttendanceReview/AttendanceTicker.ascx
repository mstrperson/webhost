<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttendanceTicker.ascx.cs" Inherits="WebhostV2.UserControls.AttendanceReview.AttendanceTicker" %>
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Timer OnTick="TickerTimer_Tick" ID="TickerTimer" Enabled="true" Interval="150000" runat="server"></asp:Timer>
        <article class="control">
            <header>Attendance Ticker</header>
            <asp:Table ID="TickerTable" CssClass="bordered" runat="server"></asp:Table>
        </article>
    </ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID="TickerTimer" />
    </Triggers>
</asp:UpdatePanel>

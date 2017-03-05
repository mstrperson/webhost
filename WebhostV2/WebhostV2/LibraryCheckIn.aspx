<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="LibraryCheckIn.aspx.cs" Inherits="WebhostV2.LibraryCheckIn" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    Library Study Hall Check In
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <asp:UpdatePanel ID="LibraryPassesPanel" runat="server">
        <ContentTemplate>
            <asp:Timer ID="PassRefreshTimer" runat="server" Interval="30000" OnTick="PassRefreshTimer_Tick"></asp:Timer>
            <article class="control">
                <asp:Table ID="PassTable" runat="server"></asp:Table>
            </article>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="PassRefreshTimer" EventName="Tick" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

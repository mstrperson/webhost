<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="CourseRequestConfirmed.aspx.cs" Inherits="WebhostV2.CourseRequestConfirmed" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    <p>
        Course Request Completed</p>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <article class="control">
        <header>Courses Requested</header>
        <section>
            <asp:TextBox ID="Output" runat="server" ReadOnly="true" TextMode="MultiLine" Width="100%" Height="480px"></asp:TextBox>
        </section>
        <hr />
        <asp:Button ID="GoBack" runat="server" Text="Return to Course Request Form." OnClick="GoBack_Click" />
    </article>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

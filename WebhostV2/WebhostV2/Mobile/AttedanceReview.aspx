<%@ Page Title="" Language="C#" MasterPageFile="~/Mobile/Mobile.Master" AutoEventWireup="true" CodeBehind="AttedanceReview.aspx.cs" Inherits="WebhostV2.Mobile.AttedanceReview" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    Attendance Review
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="NavPlaceHolder" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <article class="control">
                <header>
                    <asp:Button ID="PreviousWeekBtn" runat="server" Text="  <-  " OnClick="PreviousWeekBtn_Click" />
                    <asp:Label ID="WeekLabel" runat="server" Text=""></asp:Label>
                    <asp:Button ID="NextWeekBtn" runat="server" Text="  ->  " OnClick="NextWeekBtn_Click" />
                </header>
                <asp:Table ID="ClassesTable" runat="server" CssClass="bordered"></asp:Table>
            </article>
            <asp:Panel ID="AdminPanel" runat="server" Visible="false">
                <article class="control">
                    <header>Admin Tools</header>
                    <section>
                        <header>Teachers <asp:Button ID="PingBtn" runat="server" Text="Send Reminder Email" Visible="false" OnClick="PingBtn_Click" /></header>
                        <ajaxToolkit:ComboBox ID="TeacherSelect" runat="server"></ajaxToolkit:ComboBox>
                        <asp:Button ID="TeacherBtn" runat="server" Text="View Teacher" OnClick="TeacherBtn_Click" />
                    </section>
                    <section>
                        <header>Students</header>
                        <ajaxToolkit:ComboBox ID="StudentSelect" runat="server"></ajaxToolkit:ComboBox>
                        <asp:Button ID="StudentBtn" runat="server" Text="View Student" OnClick="StudentBtn_Click" />
                    </section>
                </article>
            </asp:Panel>
            <asp:Panel ID="ConfirmPanel" runat="server" Visible="false">
                <article class="success">
                    <header>Are you sure you want to send this email?</header>
                    It will automatically send an email to every Faculty who has not marked attendance yet today for classes that meet today.
                    <table style="width:100%">
                        <tr>
                            <td><asp:Button ID="ConfirmBtn" runat="server" Text="Confirm" BackColor="Green" OnClick="ConfirmBtn_Click"/></td>
                            <td><asp:Button ID="CancelBtn" runat="server" Text="Cancel" BackColor="Red" OnClick="CancelBtn_Click" /></td>
                        </tr>
                    </table>
                </article>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
        <ProgressTemplate>
            <article class="progress">
                Working...
            </article>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

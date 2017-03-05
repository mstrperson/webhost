<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentAttendanceTable.ascx.cs" Inherits="WebhostV2.UserControls.AttendanceReview.StudentAttendanceTable" %>
<asp:HiddenField ID="StudentIdField" runat="server" />
<asp:HiddenField ID="DateRangeStartField" runat="server" />
<asp:HiddenField ID="DateRangeEndField" runat="server" />
<asp:ScriptManagerProxy ID="AttendanceReviewSMP" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="AttendanceReviewPanel" runat="server">
    <ContentTemplate>
        <article class="sub_control">
            <details>
                <summary>
                    <header>
                        <table class="bordered">
                            <tr>
                                <th>Name</th>
                                <th>Cuts</th>
                                <th>Lates</th>
                                <th>Excused</th>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="NameLabel" runat="server" Text=""></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="CutsLabel" runat="server" Text=""></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="LatesLabel" runat="server" Text=""></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="ExcusedLabel" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </header>
                </summary>
                <asp:Table ID="ScheduleTable" CssClass="bordered" runat="server"></asp:Table>
            </details>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>

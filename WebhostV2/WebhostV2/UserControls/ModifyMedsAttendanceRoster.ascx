<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ModifyMedsAttendanceRoster.ascx.cs" Inherits="WebhostV2.UserControls.ModifyMedsAttendanceRoster" %>
<asp:ScriptManagerProxy ID="ModifyMedsAttendanceSMP" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="ModifyMedsAttendancePanel" runat="server">
    <ContentTemplate>
        <article class="control">
            <header>Meds Attendance Rosters</header>
            <asp:Table ID="MedsTable" runat="server" CssClass="bordered"></asp:Table>
            <article class="sub_control">
                <header>Add a Student</header>
                <table>
                    <tr>
                        <td>
                            <ajaxToolkit:ComboBox ID="StudentSelectComboBox" runat="server"></ajaxToolkit:ComboBox>
                        </td>
                        <td>
                            <asp:CheckBox ID="MorningCB" runat="server" Text="Morning" />
                        </td>
                        <td>
                            <asp:CheckBox ID="LunchCB" runat="server" Text="Lunch" />
                        </td>
                        <td>
                            <asp:CheckBox ID="DinnerCB" runat="server" Text="Dinner" />
                        </td>
                        <td>
                            <asp:CheckBox ID="BedtimeCB" runat="server" Text="Bedtime" />
                        </td>
                        <td>
                            <asp:Button ID="AddBtn" runat="server" Text="Save" OnClick="AddBtn_Click" />
                        </td>
                    </tr>
                </table>
            </article>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>


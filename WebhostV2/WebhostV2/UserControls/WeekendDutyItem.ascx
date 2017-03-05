<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WeekendDutyItem.ascx.cs" Inherits="WebhostV2.UserControls.WeekendDutyItem" %>
<%@ Register Src="~/UserControls/FacultyGroupSelector.ascx" TagPrefix="uc1" TagName="FacultyGroupSelector" %>
<%@ Register Src="~/UserControls/TimeSelector.ascx" TagPrefix="uc1" TagName="TimeSelector" %>


<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="DutyItemPanel" runat="server">
    <ContentTemplate>
        <article class="sub_control">
            <header>
                Weekend Duty
                    <asp:Button ID="SaveBtn" runat="server" CssClass="left" Text="Save" OnClick="SaveBtn_Click" />
                    <asp:Button ID="DeleteBtn" runat="server" CssClass="right" Text="Delete" OnClick="DeleteBtn_Click" />
            </header>
            <asp:HiddenField ID="DutyIdField" runat="server" />
            <asp:HiddenField ID="WeekendIdField" runat="server" />
            <table>
                <tr>
                    <td>
                        <asp:TextBox ID="DutyInput" runat="server" Text=""></asp:TextBox>
                    </td>
                    <td>
                        <asp:DropDownList ID="DaySelect" runat="server">
                            <asp:ListItem Text="Friday"></asp:ListItem>
                            <asp:ListItem Text="Saturday"></asp:ListItem>
                            <asp:ListItem Text="Sunday"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <uc1:TimeSelector runat="server" ID="StartTimeSelector" />
                    </td>
                    <td>
                        -
                    </td>
                    <td>
                        <uc1:TimeSelector runat="server" ID="EndTimeSelector" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="AllTeamCB" runat="server" Text="All Team Members" Checked ="false" AutoPostBack="true" OnCheckedChanged="AllTeamCB_CheckedChanged"/>
                    </td>
                    <td colspan="3">
                        <uc1:FacultyGroupSelector runat="server" ID="DutyAssignmentSelector" />
                    </td>
                </tr>
            </table>
            <asp:Panel ID="NotesPanel" runat="server">
                <asp:TextBox ID="NotesInput" TextMode="MultiLine" runat="server" Width="100%" Text="Notes"></asp:TextBox>
            </asp:Panel>
        </article>
        <asp:Panel runat="server" CssClass="progress_container" ID="SuccessPanel" Visible="false">
            <article class="success">
                <asp:Label runat="server" ID="SuccessLabel"></asp:Label>
                <asp:Button runat="server" ID="ConfirmBtn" Text="Ok" OnClick="ConfirmBtn_Click" />
            </article>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExcuseAttendance.ascx.cs" Inherits="WebhostV2.UserControls.ExcuseAttendance" %>

<asp:ScriptManagerProxy ID="ExcuseSMP" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="ExcusePanel" runat="server">
    <ContentTemplate>
        <article class="control">
            <header>Excuse an Absence</header>
            <asp:CheckBox ID="TodayCB" runat="server" Checked="True" Text="Today" OnCheckedChanged="TodayCB_CheckedChanged" AutoPostBack="True" />
            <asp:TextBox ID="DateInput" runat="server" Visible="false"></asp:TextBox>
            <ajaxToolkit:CalendarExtender ID="DateInput_CalendarExtender" runat="server" TargetControlID="DateInput" PopupPosition="Right" FirstDayOfWeek="Monday">
            </ajaxToolkit:CalendarExtender>
            <asp:CheckBox ID="MultiDayCB" runat="server" OnCheckedChanged="MultiDayCB_CheckedChanged" Text="Multiple Days" Visible="False" AutoPostBack="True" />
            <asp:TextBox ID="EndDateInput" runat="server" Visible="False"></asp:TextBox>
            <ajaxToolkit:CalendarExtender ID="EndDateInput_CalendarExtender" runat="server" FirstDayOfWeek="Monday" PopupPosition="Right" TargetControlID="EndDateInput">
            </ajaxToolkit:CalendarExtender>
            <asp:Button ID="SelectDateBtn" runat="server" OnClick="SelectDateBtn_Click" Text="Select Date" Visible="False" />
            <table class="bordered">
                <tr>
                    <th>
                        Student:
                    </th>
                    <td colspan="2">
                        <ajaxToolkit:ComboBox ID="StudentNameCBX" runat="server" AutoCompleteMode="SuggestAppend"></ajaxToolkit:ComboBox>
                    </td>
                </tr>
                <tr>
                    <th>Blocks:</th>
                    <td>
                        <asp:CheckBox ID="AllDayCB" runat="server" Text="All Day" OnCheckedChanged="AllDayCB_CheckedChanged" AutoPostBack="True" /><br />
                        <asp:CheckBox ID="MorningOnlyCB" runat="server" Text="Morning Classes" OnCheckedChanged="MorningOnlyCB_CheckedChanged" AutoPostBack="True" /><br />
                        <asp:CheckBox ID="AfternoonOnlyCB" runat="server" Text="Afternoon Classes" OnCheckedChanged="AfternoonOnlyCB_CheckedChanged" AutoPostBack="True"/><br />
                        <asp:CheckBox ID="EveningCB" runat="server" Text="Evening Activities" AutoPostBack="True" OnCheckedChanged="EveningCB_CheckedChanged" />
                    </td>
                    <td>
                        <asp:CheckBoxList ID="BlocksCBL" runat="server"></asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        Notes For Teachers: (Will be Emailed to relevant teachers and advisor)<br />
                        <asp:TextBox ID="NotesInput" runat="server" TextMode="MultiLine" Width="100%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:Button ID="SubmitBtn" runat="server" Text="Submit" OnClick="SubmitBtn_Click" />
                    </td>
                </tr>
            </table>
        </article>
        <asp:Panel ID="ErrorPanel" runat="server" Visible="false" CssClass="progress_container">
            <article class="error">
                <header>Invalid Information</header>
                <asp:Label ID="ErrorLabel" runat="server" Text="Error"></asp:Label><br />
                <asp:Button ID="DismissBtn" runat="server" Text="Ok" OnClick="DismissBtn_Click" />
            </article>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="ExcuseUP" runat="server">
    <ProgressTemplate>
            <div class="progress_container">
        <article class="progress">
            Working...
        </article></div>
    </ProgressTemplate>
</asp:UpdateProgress>



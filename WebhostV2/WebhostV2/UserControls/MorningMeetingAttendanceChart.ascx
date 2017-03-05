<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MorningMeetingAttendanceChart.ascx.cs" Inherits="WebhostV2.UserControls.MorningMeetingAttendanceChart" %>
<%@ Register Src="~/UserControls/StudentGroupSelector.ascx" TagPrefix="uc1" TagName="StudentGroupSelector" %>
<%@ Register Src="~/UserControls/MorningMeetingSeatClicker.ascx" TagPrefix="uc1" TagName="MorningMeetingSeatClicker" %>


<asp:ScriptManagerProxy ID="MorningMeetingSMP" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="MorningMeetingPanel" runat="server">
    <ContentTemplate>
        <article class="morning_meeting">
            <header>
                Date: <asp:TextBox ID="DateInput" runat="server"></asp:TextBox>
                <ajaxToolkit:CalendarExtender ID="DateInput_CalendarExtender" runat="server" TargetControlID="DateInput">
                </ajaxToolkit:CalendarExtender>
                <asp:Button ID="DateSelectBtn" runat="server" Text="Load Selected Date" OnClick="DateSelectBtn_Click" />
            <uc1:StudentGroupSelector runat="server" ID="LateStudentsSelector" ActiveOnly="true" />
            <asp:Button ID="MarkLatesBtn" runat="server" Text="Mark Lates" OnClick="MarkLatesBtn_Click" /><asp:Button ID="SubmitBtn" runat="server" Text="Submit Attendance" OnClick="SubmitBtn_Click" />
            </header>
            <br />
            <table class="morning_meeting">
                <tr>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="L1" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="L2" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="L3" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="L4" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="L5" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="L6" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="L7" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="L8" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="L9" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="L10" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="L11" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="L12" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="L13" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="L14" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="K1" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="K2" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="K3" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="K4" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="K5" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="K6" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="K7" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="K8" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="K9" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="K10" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="K11" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="J1" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="J2" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="J3" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="J4" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="J5" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="J6" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="J7" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="J8" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="J9" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="J10" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="J11" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td></td><td></td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="I1" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="I2" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="I3" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="I4" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="I5" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="I6" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="I7" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="I8" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="I9" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="I10" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="H1" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="H2" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="H3" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="H4" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="H5" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="H6" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="H7" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="H8" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="H9" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="H10" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="H11" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="H12" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="H13" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="H14" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="G1" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="G2" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="G3" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="G4" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="G5" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="G6" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="G7" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="G8" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="G9" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="G10" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="G11" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="G12" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="G13" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="G14" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="F1" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="F2" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="F3" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="F4" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="F5" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="F6" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="F7" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="F8" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="F9" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="F10" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="F11" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="F12" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="F13" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="F14" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="E1" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="E2" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="E3" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="E4" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="E5" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="E6" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="E7" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="E8" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="E9" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="E10" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="E11" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="E12" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="E13" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="E14" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="D1" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="D2" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="D3" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="D4" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="D5" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="D6" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="D7" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="D8" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="D9" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="D10" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="D11" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="D12" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="D13" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="D14" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="C1" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="C2" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="C3" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="C4" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="C5" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="C6" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="C7" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="C8" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="C9" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="C10" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="C11" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="C12" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="C13" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="C14" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="B1" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="B2" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="B3" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="B4" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="B5" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="B6" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="B7" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="B8" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="B9" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="B10" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="B11" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="B12" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="B13" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="B14" runat="server" Enabled="false" />
                    </td>
                </tr>
                <tr>
                    <td></td><td></td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="A1" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="A2" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="A3" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="A4" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="A5" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="A6" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="A7" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="A8" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="A9" runat="server" />
                    </td>
                    <td>
                        <uc1:MorningMeetingSeatClicker ID="A10" runat="server" />
                    </td>
                </tr>
            </table>
            
        </article>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="MorningMeetingUP" runat="server">
    <ProgressTemplate>
            <div class="progress_container">
        <article class="progress">
            Working...
        </article></div>
    </ProgressTemplate>
</asp:UpdateProgress>

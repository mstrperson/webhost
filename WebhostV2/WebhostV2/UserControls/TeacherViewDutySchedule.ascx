<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TeacherViewDutySchedule.ascx.cs" Inherits="WebhostV2.UserControls.TeacherViewDutySchedule" %>
<asp:HiddenField ID="WeekendIdField" runat="server" />
<article class="control">
    <header>
        <asp:Label ID="WeekendLabel" runat="server">This weekend has not been Built Yet!</asp:Label>
    </header>
    <details>
        <summary>Duty Team Info 
            <asp:Button ID="DownloadSchedule" runat="server" Text="Download PDF Schedule" />
            </summary>
        <asp:Table ID="DutyTeamTable" runat="server"></asp:Table>
    </details>
    <details>
        <summary>Friday</summary>
        <asp:Table ID="FridayTable" runat="server"></asp:Table>
    </details>
    <details>
        <summary>Saturday</summary>
        <asp:Table ID="SaturdayTable" runat="server"></asp:Table>
    </details>
    <details>
        <summary>Sunday</summary>
        <asp:Table ID="SundayTable" runat="server"></asp:Table>
    </details>
</article>
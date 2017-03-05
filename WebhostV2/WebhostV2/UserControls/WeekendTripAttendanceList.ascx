<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WeekendTripAttendanceList.ascx.cs" Inherits="WebhostV2.UserControls.WeekendTripAttendanceList" %>
<%@ Register Src="~/UserControls/StudentGroupSelector.ascx" TagPrefix="uc1" TagName="StudentGroupSelector" %>

<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="ActivityIdField" runat="server" />
        <article class="control">
            <header>
                <asp:Label ID="TripNameLabel" runat="server" Text="TripName"></asp:Label></header>
            <asp:Table ID="AttendanceTabel" runat="server" CssClass="bordered" Width="8cm"></asp:Table>
            <asp:Button ID="SubmitBtn" runat="server" Text="Submit" OnClick="SubmitBtn_Click" />
            <uc1:StudentGroupSelector runat="server" ID="AdditionalStudentsSelector" />
            <asp:Button ID="AddToTripBtn" runat="server" Text="Add Selected Students to the Trip" OnClick="AddToTripBtn_Click" />
        </article>
    </ContentTemplate>
</asp:UpdatePanel>

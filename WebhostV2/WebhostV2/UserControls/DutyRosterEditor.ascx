<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DutyRosterEditor.ascx.cs" Inherits="WebhostV2.UserControls.DutyRosterEditor" %>
<%@ Register Src="~/UserControls/FacultyGroupSelector.ascx" TagPrefix="uc1" TagName="FacultyGroupSelector" %>


<article class="control">
    <header>
        <asp:Label runat="server" ID="DutyTeamLabel">DutyTeam</asp:Label>&nbsp;&nbsp;&nbsp;&nbsp; 
    </header><hr />
    <asp:HiddenField ID="DutyTeamIDField" runat="server" />
    <article class="sub_control">
        <header>Leaders</header>
        <table>
            <tr>
                <th>DTL</th>
                <th>AOD</th>
            </tr>
            <tr>
                <td>
                    <ajaxToolkit:ComboBox ID="DTLSelector" runat="server"></ajaxToolkit:ComboBox>
                </td>
                <td>
                    <ajaxToolkit:ComboBox ID="AODSelector" runat="server"></ajaxToolkit:ComboBox>
                </td>
            </tr>
        </table>
    </article>
    <article class="sub_control">
        <header>Team</header>
        <uc1:FacultyGroupSelector runat="server" ID="FacultyGroupSelector1" />
    </article>
    <asp:Button ID="SaveBtn" runat="server" Text="Save Changes" OnClick="SaveBtn_Click" />
</article>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetentionGenerator.ascx.cs" Inherits="WebhostV2.UserControls.DetentionGenerator" %>
<%@ Register Src="~/UserControls/StudentGroupSelector.ascx" TagPrefix="uc1" TagName="StudentGroupSelector" %>

<asp:ScriptManagerProxy ID="DetentionSMP" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="DetentionPanel" runat="server">
    <ContentTemplate>
        <article class="control">
            <header>Detention List Generator</header>
            <section class="control_list">
                <table>
                    <tr>
                        <th>Lates Per Cut</th>
                        <td>
                            <asp:TextBox ID="LPCInput" runat="server" TextMode="Number"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Cuts for 1 hour Detention</th>
                        <td>
                            <asp:TextBox ID="OneHrCutsInput" runat="server" TextMode="Number"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>
                            Cuts for 2 hour Detention
                        </th>
                        <td>
                            <asp:TextBox ID="TwoHrCutsInput" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <th><asp:CheckBox ID="CampusCB" runat="server" Checked="true" Text="Cuts for Campusing"/></th>
                        <td>
                            <asp:TextBox ID="CampusCutsInput" runat="server" TextMode="Number"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <th>Attendance Week Start</th>
                        <th>Attendance Week End</th>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="StartDateInput" runat="server"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="StartDateInput_CalendarExtender" runat="server" TargetControlID="StartDateInput">
                            </ajaxToolkit:CalendarExtender>
                        </td>
                        <td>
                            <asp:TextBox ID="EndDateInput" runat="server"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="EndDateInput_CalendarExtender" runat="server" TargetControlID="EndDateInput">
                            </ajaxToolkit:CalendarExtender>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="GenerateBtn" runat="server" Text="Generate Lists" OnClick="GenerateBtn_Click" />
                        </td>
                    </tr>
                </table>
            </section>
            <article class="subcontrol">
                <header>Detention List</header>
                <uc1:StudentGroupSelector runat="server" ID="OneHrDetentionListSelector" Title="One Hour Detention" />
                <uc1:StudentGroupSelector runat="server" ID="TwoHrDetentionListSelector" Title="Two Hour Detention" />
            </article>
            <article class="subcontrol">
                <header>Campused List</header>
                <uc1:StudentGroupSelector runat="server" ID="CampusedListSelector" />
            </article>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="DetentionUP" runat="server" AssociatedUpdatePanelID="DetentionPanel">
    <ProgressTemplate>
        
            <div class="progress_container">
        <article class="progress">
            Working...
        </article></div>
    </ProgressTemplate>
</asp:UpdateProgress>

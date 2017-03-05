<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ScheduleView.ascx.cs" Inherits="WebhostV2.UserControls.ScheduleView" %>
<asp:ScriptManagerProxy ID="ScheduleViewSMP" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="ScheduleViewPanel" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="UserIdField" runat="server" />
        <asp:HiddenField ID="IsStudentField" runat="server" />
        <article class="control">
            <header>Schedule for <asp:Label ID="NameLabel" runat="server" Text=""></asp:Label></header>
            <asp:Table ID="ScheduleTable" runat="server"></asp:Table>
            <table>
                <tr><th>Add a Class</th><th>Drop a Class</th></tr>
                <tr>
                    <td>
                        <asp:DropDownList ID="AddClassDDL" runat="server"></asp:DropDownList><asp:Button ID="AddClassBtn" runat="server" Text=" + " OnClick="AddClassBtn_Click" Enabled="False" />
                    </td>
                    <td>
                        <asp:DropDownList ID="DropClassDDL" runat="server"></asp:DropDownList><asp:Button ID="DropClassBtn" runat="server" Text=" X " OnClick="DropClassBtn_Click" Enabled="False" />
                    </td>
                </tr>
            </table>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="ScheduleViewUP" runat="server" AssociatedUpdatePanelID="ScheduleViewPanel">
    <ProgressTemplate>
        
            <div class="progress_container">
        <article class="progress">
            Loading...
        </article></div>
    </ProgressTemplate>
</asp:UpdateProgress>

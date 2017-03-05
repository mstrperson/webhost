<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PermissionEditor.ascx.cs" Inherits="WebhostV2.UserControls.PermissionEditor" %>
<%@ Register src="FacultyGroupSelector.ascx" tagname="FacultyGroupSelector" tagprefix="uc1" %>
<%@ Register src="StudentGroupSelector.ascx" tagname="StudentGroupSelector" tagprefix="uc2" %>
<asp:ScriptManagerProxy ID="PermissionEditorSMP" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="PermissionEditorPanel" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="PermissionIdField" runat="server" />
            <article class="control">
            <header>Permission:  <asp:TextBox ID="PermissionNameInput" runat="server"></asp:TextBox></header>
            <table>
                <tr>
                    <th>Faculty</th><th>Students</th>
                </tr>
                <tr>
                    <td>
                        <uc1:FacultyGroupSelector ID="FacultyGroupSelector1" runat="server" />
                    </td>
                    <td>
                        <uc2:StudentGroupSelector ID="StudentGroupSelector1" runat="server" />
                    </td>
                </tr>
                <tr>
                    <th colspan="2">Description</th>
                </tr>
                <tr>
                    <td colspan ="2">
                        <asp:TextBox ID="NotesInput" TextMode="MultiLine" runat="server" Width="100%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="SaveBtn" runat="server" Text="Save Changes" OnClick="SaveBtn_Click" />
                    </td>
                    <td>
                        <asp:Button ID="CreateBtn" runat="server" Text="Clear Form and Create New" OnClick="CreateBtn_Click" />
                    </td>
                </tr>
            </table>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress AssociatedUpdatePanelID="PermissionEditorPanel" ID="PermissionEditorUP" runat="server">
    <ProgressTemplate>
            <div class="progress_container">
        <article class="progress">
            Working....
        </article></div>
    </ProgressTemplate>
</asp:UpdateProgress>

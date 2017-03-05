<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PermissionSelector.ascx.cs" Inherits="WebhostV2.UserControls.PermissionSelector" %>
<asp:ScriptManagerProxy ID="StudentGroupSMP" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="PermissionPanel" runat="server" OnLoad="UpdatePanel1_Load">
    <ContentTemplate>
        <asp:HiddenField ID="PermissionListField" runat="server" />
        <asp:Label ID="TitleLabel" runat="server"></asp:Label>
        <table>
            <tr>
                <td>
                    <ajaxToolkit:ComboBox ID="PermissionSelectionCB" runat="server" Width="3cm" AutoCompleteMode="SuggestAppend"></ajaxToolkit:ComboBox>
                </td>
                <td>
                    <asp:Button ID="AddStudentBtn" runat="server" OnClick="AddStudent_Click" Text=" + " />
                </td>
                </tr>
            <tr>
                <td colspan="2">
                    <asp:Table ID="SelectedTable" runat="server" Width="5cm"></asp:Table>
                </td>
            </tr>
        </table>
        <asp:DropDownList ID="RemoveList" Visible="false" runat="server"></asp:DropDownList><asp:Button ID="RemoveBtn" Visible="false" runat="server" Text=" X " OnClick="RemoveBtn_Click" />
    </ContentTemplate>
</asp:UpdatePanel>
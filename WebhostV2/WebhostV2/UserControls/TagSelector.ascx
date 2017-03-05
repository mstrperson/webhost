<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TagSelector.ascx.cs" Inherits="WebhostV2.UserControls.TagSelector" %>
<asp:ScriptManagerProxy ID="TagSMP" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="TagPanel" runat="server" OnLoad="UpdatePanel1_Load">
    <ContentTemplate>
        <asp:HiddenField ID="TagListField" runat="server" />
        <asp:Label ID="TitleLabel" runat="server"></asp:Label>
        <table>
            <tr>
                <td>
                    <ajaxToolkit:ComboBox ID="TagSelectionCB" runat="server" Width="3cm" AutoCompleteMode="SuggestAppend"></ajaxToolkit:ComboBox>
                </td>
                <td>
                    <asp:Button ID="AddTagBtn" runat="server" OnClick="AddStudent_Click" Text=" + " />
                </td>
                </tr>
            <tr>
                <td colspan="2">
                    <asp:Table ID="SelectedTable" runat="server" Width="5cm"></asp:Table>
                </td>
            </tr>
            <tr>
                <th colspan="2">New Tag</th>
            </tr>
            <tr>
                <td><asp:TextBox ID="NewTagNameInput" runat="server"></asp:TextBox></td>
                <td>
                    <asp:Button ID="CreateTagBtn" runat="server" Text=" + " OnClick="CreateTagBtn_Click" /></td>
            </tr>
        </table>
        <asp:DropDownList ID="RemoveList" Visible="false" runat="server"></asp:DropDownList><asp:Button ID="RemoveBtn" Visible="false" runat="server" Text=" X " OnClick="RemoveBtn_Click" />
    </ContentTemplate>
</asp:UpdatePanel>
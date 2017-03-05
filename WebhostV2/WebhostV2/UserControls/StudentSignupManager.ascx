<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentSignupManager.ascx.cs" Inherits="WebhostV2.UserControls.StudentSignupManager" %>
<asp:ScriptManagerProxy ID="SSMSMP" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="SignupManagerPanel" runat="server">
    <ContentTemplate>
        <article class="control">
            <asp:HiddenField ID="SignupIdField" runat="server" />
            <header>
                <asp:Label ID="ActivityNameLabel" runat="server"></asp:Label>
            </header>
            <asp:TextBox ID="CurrentSignups" TextMode="MultiLine" ReadOnly="true" runat="server" Width="100%" Height="380px"></asp:TextBox>
            <table>
                <tr>
                    <td colspan="2"><asp:DropDownList ID="StudentsSignedUpDDL" runat="server"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="KickBtn" runat="server" Text="Remove This Student" OnClick="KickBtn_Click" />
                    </td>
                    <td>
                        <asp:Button ID="UnBanBtn" runat="server" Text="Allow This Student" OnClick="UnBanBtn_Click" />
                    </td>
                </tr>
            </table>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>

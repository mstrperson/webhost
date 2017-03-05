<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LibraryPassCheckIn.ascx.cs" Inherits="WebhostV2.UserControls.LibraryPassCheckIn" %>
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="PassIdField" runat="server" />
        <article class="sub_control">
            <table class="studyhall">
                <tr>
                    <td>
                        <asp:Label ID="NameLabel" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="Info" runat="server" TextMode="MultiLine" ReadOnly="True"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Button ID="SignBtn" runat="server" Text="Sign In" OnClick="SignBtn_Click" />
                    </td>
                </tr>
            </table>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>

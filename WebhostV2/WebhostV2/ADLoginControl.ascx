<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ADLoginControl.ascx.cs" Inherits="WebhostV2.ADLoginControl" %>

<article id="login_container" class="control">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    </asp:ScriptManagerProxy>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:HiddenField ID="AuthenticatedUid" runat="server" />            
            <header id="login_header">
                <div>
                    <asp:Image ID="Image1" runat="server" Height="2in" 
                        ImageUrl="~/images/seal.png" Width="2in" />
                </div>
                <div>
                    Dublin School Webhost Login
                </div>
            </header>
            <table id="login">
                <tr>
                    <td>
                        User Name:
                    </td>
                    <td>
                        <asp:TextBox ID="UserNameInput" runat="server" Width="100%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Password:
                    </td>
                    <td>
                        <asp:TextBox ID="PasswordInput" runat="server" TextMode="Password" Width="100%"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <asp:Label ID="ErrorLabel" runat="server" CssClass="ErrorMessage" Width="100%"></asp:Label>
            <div id="login_button" class="Centre">
                <asp:Button ID="LoginoutBtn" runat="server" Text="Login" OnClick="LoginoutBtn_Click" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <div class="progress_container">
            <article class="progress">
                Processing Login Information...
            </article>
                </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</article>
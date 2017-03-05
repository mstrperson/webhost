<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentSignup.ascx.cs" Inherits="WebhostV2.UserControls.StudentSignup" %>
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <article class="sub_control">
            <asp:HiddenField ID="SignupIdField" runat="server" />
            <header>
                <asp:Label ID="ActivityNameLabel" runat="server"></asp:Label>
            </header>
            <asp:Button ID="Signup" runat="server" Text="Sign me up for this Activity!" OnClick="Signup_Click" /><br />
            <asp:TextBox ID="CurrentSignups" TextMode="MultiLine" ReadOnly="true" runat="server" Width="100%" Height="380px"></asp:TextBox>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>

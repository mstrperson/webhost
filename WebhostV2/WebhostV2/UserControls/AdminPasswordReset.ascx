<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminPasswordReset.ascx.cs" Inherits="WebhostV2.UserControls.AdminPasswordReset" %>
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <nav class="control">
            <header>Password Reset</header>
            <ajaxToolkit:ComboBox ID="EmailSelect" runat="server"></ajaxToolkit:ComboBox>
            <asp:CheckBox ID="RandomizeCB" runat="server" Text="Random Password" Checked="true" AutoPostBack="True" OnCheckedChanged="RandomizeCB_CheckedChanged"/>
            <asp:TextBox ID="NewPasswordInput" runat="server" TextMode="Password" Visible="false"></asp:TextBox>
            Send Reset Info:<br />
            <asp:TextBox ID="ResetInfoEmail" runat="server" TextMode="Email"></asp:TextBox>
            <asp:Button ID="ResetBtn" runat="server" Text="Reset" OnClick="ResetBtn_Click" />
        </nav>
    </ContentTemplate>
</asp:UpdatePanel>

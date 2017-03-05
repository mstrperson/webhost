<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeviceRegistrationAdmin.ascx.cs" Inherits="WebhostV2.UserControls.DeviceRegistrationAdmin" %>
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <article class="control">
            <header>Student Device Registration Requests</header>
            <asp:Table ID="RegistrationRequestTable" runat="server"></asp:Table>
            <hr />
            <asp:Button ID="GenerateImportBtn" runat="server" Text="Generate DHCP Import File." OnClick="GenerateImportBtn_Click" />
        </article>
    </ContentTemplate>
</asp:UpdatePanel>

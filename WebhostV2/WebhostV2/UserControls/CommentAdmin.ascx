<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentAdmin.ascx.cs" Inherits="WebhostV2.UserControls.CommentAdmin" %>
<nav class="control">
    <header>Comment Admin</header>
    <asp:Button ID="DownloadAllBtn" runat="server" Text="Download All" OnClick="DownloadAllBtn_Click" />
    <hr />
    <asp:Button ID="GetHeadersBtn" runat="server" Text="Get Comment Headers" OnClick="GetHeadersBtn_Click"/>
    <hr />
    <ajaxToolkit:ComboBox ID="FacultyCmbBx" runat="server"></ajaxToolkit:ComboBox><br />
    <asp:Button ID="MasqBtn" runat="server" Text="Edit" OnClick="MasqBtn_Click" />
</nav>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourseRequestAdmin.ascx.cs" Inherits="WebhostV2.UserControls.CourseRequestAdmin" %>
<nav class="control">
    <header>Get Other Course Requests</header>
    <hr />
    <ajaxToolkit:ComboBox ID="FacultyCmbBx" runat="server"></ajaxToolkit:ComboBox><br />
    <asp:Button ID="MasqBtn" runat="server" Text="Edit" OnClick="MasqBtn_Click" />
    <hr />
    <asp:Button ID="BlackbaudExport" runat="server" Text="Download for Blackbaud" OnClick="BlackbaudExport_Click" />
    <asp:Button ID="ExcelExport" runat="server" Text="Download for Excel" OnClick="ExcelExport_Click" />
    <asp:Button ID="NotesExport" runat="server" Text="Download Notes" OnClick="NotesExport_Click" />
    <asp:Button ID="GetOverviewBtn" runat="server" OnClick="GetOverviewBtn_Click" Text="Check Status" />
    <asp:Button ID="ClassCountBtn" runat="server" OnClick="ClassCountBtn_Click" Text="Class Counts" />
    <asp:Button ID="ExportPack" runat="server" OnClick="ExportPack_Click" Text="Download all Students' Requets" />
</nav>
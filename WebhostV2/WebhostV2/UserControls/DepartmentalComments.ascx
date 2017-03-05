<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DepartmentalComments.ascx.cs" Inherits="WebhostV2.UserControls.DepartmentalComments" %>

<nav class="control">
    <header>Department Comments</header>
    <asp:DropDownList ID="DepartmentDDL" runat="server"></asp:DropDownList>
    <asp:Button ID="LoadDept" runat="server" Text="Load Department Comments" OnClick="LoadDept_Click" />
    <asp:Button ID="Clear" runat="server" Text="Exit Departmental Mode" OnClick="Clear_Click" />
</nav>

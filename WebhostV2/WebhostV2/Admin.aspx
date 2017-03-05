<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="WebhostV2.Admin" %>
<%@ Register src="UserControls/DormBuilder.ascx" tagname="DormBuilder" tagprefix="uc1" %>
<%@ Register src="UserControls/UserCreation.ascx" tagname="UserCreation" tagprefix="uc2" %>
<%@ Register src="UserControls/TextEditor.ascx" tagname="TextEditor" tagprefix="uc3" %>
<%@ Register Src="~/UserControls/CreateSection.ascx" TagPrefix="uc1" TagName="CreateSection" %>
<%@ Register Src="~/UserControls/LivingLevelsBuilder.ascx" TagPrefix="uc1" TagName="LivingLevelsBuilder" %>
<%@ Register Src="~/UserControls/CreateFacultyEntry.ascx" TagPrefix="uc1" TagName="CreateFacultyEntry" %>
<%@ Register Src="~/UserControls/AdminPasswordReset.ascx" TagPrefix="uc1" TagName="AdminPasswordReset" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    Webhost Administration
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
    <nav class="control">
        <asp:Menu ID="AdminMenu" runat="server" RenderingMode="Table">
            <DynamicMenuStyle CssClass="menu" />
            <Items>
                <asp:MenuItem Text="Task" Value="Task">
                    <asp:MenuItem NavigateUrl="~/Rosters.aspx" Text="Roster Management" Value="Roster Management"></asp:MenuItem>
                    <asp:MenuItem NavigateUrl="~/DTL.aspx" Text="Weekend Schedule" Value="Weekend Schedule"></asp:MenuItem>
                    <asp:MenuItem NavigateUrl="~/Import.aspx" Text="Import Data" Value="Import Data" />
                    <asp:MenuItem NavigateUrl="~/PermissionEditor.aspx" Text="Edit Permissions" Value="Edit Permissions" />
                </asp:MenuItem>
            </Items>
        </asp:Menu>
    </nav>
    <nav class="control">
        <header>Admin Tools</header>
        <ajaxToolkit:ComboBox ID="TermSelect" runat="server"></ajaxToolkit:ComboBox>
        <asp:Button ID="SchoologyExport" runat="server" Text="Schoology Export" OnClick="SchoologyExport_Click" />
        <br />
        <asp:Button ID="MorningMeetingGenBtn" runat="server" OnClick="MorningMeetingGenBtn_Click" Text="Get Morning Meeting Chart" />
        <br />
        <asp:Button ID="AttendanceDump" runat="server" OnClick="AttendanceDump_Click" Text="Attendance Dump" />
        <asp:Button ID="GetTutorBtn" runat="server" OnClick="GetTutorBtn_Click" Text="Get Tutorial Sections" />
        <asp:Button ID="SchoologyPull" runat="server" Text="Pull Data From Schoology" OnClick="SchoologyPull_Click" />
        <br />
        <br />
        <asp:FileUpload ID="NewUsersUpload" runat="server" />
        <br />
        <asp:Button ID="AddPasswordsBtn" runat="server" OnClick="AddPasswordsBtn_Click" Text="Add Passwords To UserFile" />
        <hr />
        <header>Analytics</header>
        <asp:Button ID="ByClassBtn" runat="server" Text="Grades By Class" OnClick="ByClassBtn_Click" />
    </nav>
    <uc1:AdminPasswordReset runat="server" id="AdminPasswordReset1" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <uc1:CreateFacultyEntry runat="server" id="CreateFacultyEntry1" />
    <uc1:DormBuilder ID="DormBuilder1" runat="server" />
    <uc2:UserCreation ID="UserCreation1" runat="server" />
    <uc3:TextEditor ID="TextEditor1" runat="server" />
    <uc1:CreateSection runat="server" id="CreateSection1" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

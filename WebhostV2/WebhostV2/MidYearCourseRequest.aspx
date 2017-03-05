<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="MidYearCourseRequest.aspx.cs" Inherits="WebhostV2.MidYearCourseRequest" %>

<%@ Register Src="~/UserControls/CourseRequestAdmin.ascx" TagPrefix="uc1" TagName="CourseRequestAdmin" %>
<%@ Register Src="~/UserControls/RequestCourseSelector.ascx" TagPrefix="uc1" TagName="RequestCourseSelector" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    <asp:Label ID="TermNameLabel" runat="server" Text="Course Request"></asp:Label>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
    <uc1:CourseRequestAdmin runat="server" ID="CourseRequestAdmin1" OnMasquerade="CourseRequestAdmin_Masquerade" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <article class="control">
                <header>Student Information</header>
                Student Name:  <asp:DropDownList ID="StudentSelectDDL" runat="server"></asp:DropDownList>
                <asp:Button ID="SelectStudentbtn" runat="server" OnClick="SelectStudentbtn_Click" Text="Fill out Course Request" />
                <hr />
                <section>
                    Advisors: Please fill in your advisee&#39;s first and second choice for each subject area (and third choice in some cases). Next to the second choice, please indicate whether it is an alternate to the first choice or a course the student would like to take concurrently.
                </section>
            </article>
            <asp:Panel ID="SignUpPanel" runat="server" Visible="false">
                <article class="control">
                    <header>Add a course to Request</header>
                    <uc1:RequestCourseSelector runat="server" ID="NewCourseRequestSelector" />
                    <asp:Button ID="AddRequestBtn" runat="server" Text="Add to Course requests" OnClick="AddRequestBtn_Click" />
                </article>
                <asp:Panel runat="server" ID="ErrorPanel" Visible="false">
                    <article class="error">
                        <header>Invalid Request</header>
                        <asp:Label runat="server" ID="ErrorLabel" Text=""></asp:Label>
                        <asp:Button runat="server" ID="DismissBtn" OnClick="DismissBtn_Click" Text="Dismiss" />
                    </article>
                </asp:Panel>
                <article class="control">
                    <header>Add a Note to this student's course requests</header>
                    <asp:Label ID="NotesStatus" runat="server" Text="No Notes Saved."></asp:Label>
                    <asp:TextBox ID="NotesEntry" runat="server" TextMode ="MultiLine" Height="150px" Width="100%" />
                    <asp:Button ID="NoteSubmit" runat="server" Text ="Save Notes" OnClick="NoteSubmit_Click" />
                </article>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <article class="control">
        <header>Saved Requests</header>
        <asp:Table ID="SavedRequestsTable" runat="server" CssClass="bordered"></asp:Table>
    </article>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

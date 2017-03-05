<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="APCourseRequests.aspx.cs" Inherits="WebhostV2.APCourseRequests" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    AP Course Requests
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <article class="control">
                <table>
                    <tr>
                        <th>
                            Student
                        </th>
                        <th>
                            Requested AP
                        </th>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList ID="StudentSelectDDL" runat="server"></asp:DropDownList><asp:Button ID="SelectStudent" runat="server" Text="Select Student" OnClick="SelectStudent_Click" />
                        </td>
                        <td>
                            <asp:DropDownList ID="CourseRequestDDL" runat="server"></asp:DropDownList><asp:Button ID="SelectCR" runat="server" Text="Select Course Request" OnClick="SelectCR_Click" />
                        </td>
                    </tr>
                </table>
                <asp:Panel ID="ApprovalPanel" runat="server">
                    <article class="sub_control">
                        <header>Approval</header>
                        <table>
                            <tr>
                                <td colspan="2">
                                    <asp:Label ID="RequestedCourseName" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Primary Current Course and Grade:
                                </td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="CurrentCourseDDL" runat="server" AutoCompleteMode="SuggestAppend"></ajaxToolkit:ComboBox>
                                    <asp:DropDownList ID="CurrentCourseGradeDDL" runat="server" Enabled="false"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Secondary Current Course and Grade:
                                </td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="SecondaryCourseDDL" runat="server" AutoCompleteMode="SuggestAppend"></ajaxToolkit:ComboBox>
                                    <asp:DropDownList ID="SecondaryCourseGradeDDL" runat="server" Enabled="false"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>Current Teacher Signed</td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="CurrentTeacherDDL" runat="server" AutoCompleteMode="SuggestAppend"></ajaxToolkit:ComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Department Head Signed
                                </td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="DeptHeadDDL" runat="server" AutoCompleteMode="SuggestAppend"></ajaxToolkit:ComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td>Approval</td>
                                <td>
                                    <asp:DropDownList ID="ApprovalDDL" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Button ID="SubmitBtn" runat="server" Text="Submit" OnClick="SubmitBtn_Click" />
                                </td>
                            </tr>
                        </table>
                    </article>
                </asp:Panel>
            </article>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlackbaudImports.ascx.cs" Inherits="WebhostV2.UserControls.BlackbaudImports" %>
        <article class="control">
            <header>Blackbaud Imports</header>
            <table>
                <tr>
                    <th colspan="2">include</th><th>Upload CSV file</th>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="AcademicYearCB" runat="server" Text="New Academic Years" /></td>
                    <td>
                        <asp:FileUpload ID="AcademicYearUpload" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="FacultyCB" runat="server" Text="Update Faculty Details" /></td>
                    <td>
                        <asp:FileUpload ID="FacultyUpload" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="StudentsCB" runat="server" Text="Update Student Details" /></td>
                    <td>
                        <asp:FileUpload ID="StudentsUpload" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="CoursesCB" runat="server" Text="New Courses" />
                        <br />
                        <asp:CheckBox ID="CourseRequestsCB" runat="server" Text="For Course Requests" />
                    </td>
                    <td>
                        <asp:FileUpload ID="CoursesUpload" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="SectionsCB" runat="server" Text="Sections of Courses" /></td>
                    <td>
                        <asp:FileUpload ID="SectionsUpload" runat="server" /></td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="EnrollmentCB" runat="server" Text="Rosters" /></td>
                    <td class="right">Student:<br />Faculty:</td>
                    <td>
                        <asp:FileUpload ID="StudentRosterUpload" runat="server" /><br />
                        <asp:FileUpload ID="FacultyRosterUpload" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="DormCB" runat="server" Text="Dorm Rosters" /></td>
                    <td>
                        <asp:FileUpload ID="DormUpload" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="DutyRosterCB" runat="server" Text="Duty Rosters" /></td>
                    <td>
                        <asp:FileUpload ID="DutyRosterUpload" runat="server" /></td>
                </tr>
            </table>
            <hr />
            <table>
                <tr>
                    <td>
                        <ajaxToolkit:ComboBox ID="TermSelect" runat="server"></ajaxToolkit:ComboBox>
                    </td>
                    <td>
                        <asp:Button ID="ImportBtn" runat="server" Text="Do Import" OnClick="ImportBtn_Click" />
                        <asp:Button ID="ForcePasswords" runat="server" OnClick="ForcePasswords_Click" Text="Force Change Passwords" />
                    </td>
                </tr>
            </table>
            

        </article>

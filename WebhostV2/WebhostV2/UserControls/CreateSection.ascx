<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreateSection.ascx.cs" Inherits="WebhostV2.UserControls.CreateSection" %>
<%@ Register src="FacultyGroupSelector.ascx" tagname="FacultyGroupSelector" tagprefix="uc1" %>
<asp:ScriptManagerProxy ID="CreateCourseSMP" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="CreateSectionPanel" runat="server">
    <ContentTemplate>
        <article class="control">
            <header>Create a new Section</header>
            <table>
                <tr>
                    <th colspan="2">Select a Course</th>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="ExistingCourseCB" runat="server" Text="Use Existing Course" Checked="true" OnCheckedChanged="ExistingCourseCB_CheckedChanged" AutoPostBack="True"/>
                    </td>
                    <td>
                        <ajaxToolkit:ComboBox ID="CourseSelectComboBox" runat="server"></ajaxToolkit:ComboBox>
                        <asp:Panel ID="NewCoursePanel" Visible="false" runat="server">
                            <asp:TextBox ID="CourseNameInput" runat="server"></asp:TextBox>
                            <asp:DropDownList ID="DepartmentDDL" runat="server"></asp:DropDownList>
                            <asp:CheckBox ID="SendToSchoologyCB" runat="server" Text="Schoology" Checked="false" />
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <th>Block:</th>
                    <td>
                        <asp:DropDownList ID="BlockSelectDDL" runat="server"></asp:DropDownList>
                        <asp:CheckBox ID="GetsCommentsCB" Text="Gets Comment Letter" Checked="false" runat="server" />
                    </td>
                </tr>
                <tr>
                    <th>Terms:</th>
                    <td>
                        <asp:CheckBoxList ID="TermCBL" runat="server" RepeatLayout="Flow"></asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <th>Teachers:</th>
                    <td>

                        <uc1:FacultyGroupSelector ID="FacultyGroupSelector1" runat="server" />

                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="CreateSectionBtn" runat="server" Text="Create Section" OnClick="CreateSectionBtn_Click" />
                    </td>
                </tr>
            </table>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>

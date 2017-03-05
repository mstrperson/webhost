<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreateFacultyEntry.ascx.cs" Inherits="WebhostV2.UserControls.CreateFacultyEntry" %>
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <article class="control">
            <header>Create Faculty Account</header>
            <table class="bordered">
                <tr>
                    <th>First Name</th><td><asp:TextBox ID="FirstNameInput" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <th>Last Name</th><td><asp:TextBox ID="LastNameInput" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <th>Employee ID</th><td><asp:TextBox ID="EmployeeIdInput" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <th>Primary Group</th><td>
                        <asp:DropDownList runat="server" ID="primaryGroupDDL">
                            <asp:ListItem Text="Faculty" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Staff"></asp:ListItem>
                        </asp:DropDownList></td>
                </tr>
                <tr>
                    <th>Other Groups (comma separated)</th>
                    <td><asp:TextBox runat="server" ID="OtherGroupsInput"></asp:TextBox></td>
                </tr>
                <tr>
                    <th colspan="2">
                        <asp:Button ID="CreateFacultyBtn" runat="server" Text="Create" OnClick="CreateFacultyBtn_Click" />
                    </th>
                </tr>
            </table>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>


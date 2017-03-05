<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RequestCourseSelector.ascx.cs" Inherits="WebhostV2.UserControls.RequestCourseSelector" %>
<asp:HiddenField ID="RequestId" runat="server" />
<table class="bordered">
    <tr>
        <th>Department</th><th>Class</th><th>Priority</th>
    </tr>
    <tr>
        <td>
            <asp:DropDownList ID="DeptDDL" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DeptDDL_SelectedIndexChanged"></asp:DropDownList>
        </td>
        <td>
            <asp:DropDownList ID="ClassDDL" runat="server"></asp:DropDownList>
        </td>
        <td>
            <asp:DropDownList ID="PriorityDDL" runat="server">
                <asp:ListItem Text="First Choice" Value="0"></asp:ListItem>
                <asp:ListItem Text="Second Choice" Value="1"></asp:ListItem>
                <asp:ListItem Text="Third Choice" Value="2"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
</table>
<asp:Button ID="DeleteRequestBtn" runat="server" Text="Delete Request" Visible="false" OnClick="DeleteRequestBtn_Click" />

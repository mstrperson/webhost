<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentGradeBar.ascx.cs" Inherits="WebhostV2.UserControls.CommentGradeBar" %>
<table>
    <tr>
        <td>
            Exam grade:
        </td>
        <td>
            <asp:DropDownList ID="ExamDDL" runat="server"></asp:DropDownList>
        </td>
        <td>
            Trimester Grade:
        </td>
        <td>
            <asp:DropDownList ID="TrimesterDDL" runat="server"></asp:DropDownList>
        </td>
        <td>
            Effort Grade:
        </td>
        <td>
            <asp:DropDownList ID="EffortDDL" runat="server"></asp:DropDownList>
        </td>
        <td>
            Final Grade:
        </td>
        <td>
            <asp:DropDownList ID="FinalDDL" runat="server"></asp:DropDownList>
        </td>
    </tr>
</table>
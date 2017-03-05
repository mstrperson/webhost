<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReadonlyCommentViewer.ascx.cs" Inherits="WebhostV2.UserControls.ReadonlyCommentViewer" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HtmlEditor" TagPrefix="cc1" %>
<article class="control">
    <header>
        <asp:Label ID="CommentLabel" runat="server" Text="Comment for _____ in ______"></asp:Label>
    </header>
    <table class="bordered">
        <tr>
            <th>Exam Grade</th>
            <th>Trimester Grade</th>
            <th>Effort Grade</th>
            <th>Final Grade</th>
        </tr>
        <tr>
            <td>
                <asp:Label ID="ExamGradeLabel" runat="server" Text="Label"></asp:Label>
            </td>
            <td>
                <asp:Label ID="TrimesterGradeLabel" runat="server" Text="Label"></asp:Label>
            </td>
            <td>
                <asp:Label ID="EffortGradeLabel" runat="server" Text="Label"></asp:Label>
            </td>
            <td>
                <asp:Label ID="FinalGradeLabel" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
    </table>

    <cc1:Editor ID="CommentPreview" runat="server" ActiveMode="Preview" Height="8in"/>
</article>
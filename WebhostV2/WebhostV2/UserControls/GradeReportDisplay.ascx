<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GradeReportDisplay.ascx.cs" Inherits="WebhostV2.UserControls.GradeReportDisplay" %>
<asp:HiddenField runat="server" ID="StudentIdField" />
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <article class="control">
            <header><asp:Label runat="server" ID="StudentNameLabel"></asp:Label></header>
            <table class="bordered">
                <tr>
                    <th>Total Credits</th><th>GPA</th>
                </tr>
                <tr>
                    <td><asp:Label runat="server" ID="TotalCreditsLabel"></asp:Label></td><td><asp:Label runat="server" ID="GPALabel"></asp:Label></td>
                </tr>
            </table>
            <asp:Table runat="server" CssClass="bordered" ID="CreditBreakdown"></asp:Table>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>

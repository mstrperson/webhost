<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="AdviseeCommentReview.aspx.cs" Inherits="WebhostV2.AdviseeCommentReview" %>

<%@ Register Src="~/UserControls/ReadonlyCommentViewer.ascx" TagPrefix="uc1" TagName="ReadonlyCommentViewer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    Advisee Comment Review
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <nav class="control">
        <header>Advisee Comments</header>
        <asp:Button ID="DownloadCurrentBtn" runat="server" Text="Download This Term's Comments" OnClick="DownloadCurrentBtn_Click" />
    </nav>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <asp:UpdatePanel ID="AdviseeReviewPanel" runat="server">
        <ContentTemplate>
            <article class="control">
                <table class="bordered">
                    <tr>
                        <th>Advisee</th>
                        <th>Term</th>
                        <th>Class</th>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList ID="AdviseeSelectDDL" runat="server" AutoPostBack="True" OnSelectedIndexChanged="AdviseeSelectDDL_SelectedIndexChanged"></asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="TermSelectDDL" runat="server" AutoPostBack="True" OnSelectedIndexChanged="TermSelectDDL_SelectedIndexChanged"></asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="ClassSelectDDL" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ClassSelectDDL_SelectedIndexChanged"></asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </article>
            <uc1:ReadonlyCommentViewer runat="server" id="AdviseeCommentViewer"/>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

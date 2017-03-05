<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="CommentViewer.aspx.cs" Inherits="WebhostV2.CommentViewer" %>
<%@ Register src="UserControls/ReadonlyCommentViewer.ascx" tagname="ReadonlyCommentViewer" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    Read your Past Comments
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <article class="control">
                <header>Select a Class</header>
                <table>
                    <tr>
                        <th>Term</th><th>Class</th>
                    </tr>
                    <tr>
                        <td>
                            <ajaxToolkit:ComboBox ID="TermSelect" runat="server" AutoCompleteMode="SuggestAppend" AutoPostBack="true" OnSelectedIndexChanged="TermSelect_SelectedIndexChanged">
                            </ajaxToolkit:ComboBox>
                        </td>
                        <td>
                            <ajaxToolkit:ComboBox ID="ClassSelect" runat="server" Enabled="false" AutoCompleteMode="SuggestAppend" AutoPostBack="true" OnSelectedIndexChanged="ClassSelect_SelectedIndexChanged">
                            </ajaxToolkit:ComboBox>
                        </td>
                    </tr>
                </table>
            </article>
            <uc1:ReadonlyCommentViewer ID="ReadonlyCommentViewer1" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

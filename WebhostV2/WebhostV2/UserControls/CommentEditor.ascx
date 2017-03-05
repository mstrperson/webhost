<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentEditor.ascx.cs" Inherits="WebhostV2.UserControls.CommentEditor" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HtmlEditor" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/CommentGradeBar.ascx" TagPrefix="uc1" TagName="CommentGradeBar" %>

<asp:ScriptManagerProxy ID="CommentSMP" runat="server"></asp:ScriptManagerProxy>

<asp:UpdatePanel ID="CommentPanel" runat="server">
    <ContentTemplate>
        <article class="control">
            
            <article class="sub_control">
                <table style="width: 100%">
                    <tr>
                        <th colspan="3">
                            <asp:Label ID="EditorLabel" runat="server" Text="Select a Term to begin."></asp:Label>
                        </th>
                    </tr>
                    <tr>
                        <th>
                            Term
                        </th>
                        <th>
                            Class
                        </th>
                        <th>
                            Student
                        </th>
                    </tr>
                    <tr>
                        <td style="width:15%">
                            <asp:DropDownList ID="TermDDL" runat="server"></asp:DropDownList><asp:Button ID="TermSelBtn" runat="server" Text="Load Term" OnClick="TermSelBtn_Click" />
                        </td>
                        <td style="width:45%">
                            <ajaxToolkit:ComboBox ID="ClassSelectCmbBx" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ClassSelectCmbBx_SelectedIndexChanged" Width="7cm" DropDownStyle="DropDownList"></ajaxToolkit:ComboBox>
                            <asp:Button ID="HeaderBtn" runat="server" Text="Go Back To Header" Visible="False" OnClick="ClassSelectCmbBx_SelectedIndexChanged" />
                        </td>
                        <td style="text-align:right; width:39%">
                            <ajaxToolkit:ComboBox ID="StudentSelectCmbBx" runat="server" Visible="false" AutoPostBack="True" OnSelectedIndexChanged="StudentSelectCmbBx_SelectedIndexChanged" Width="7cm"></ajaxToolkit:ComboBox>
                        </td>
                    </tr>
                </table>
                <asp:Panel ID="ControlPanel" runat="server">
                    <table class="comment_controls bordered" style="width:100%">
                        <tr>
                            <td style="width:33%">
                                <h1>Saving</h1>
                                <asp:Button ID="SaveBtn" runat="server" Text="Save" OnClick="SaveBtn_Click" Width="75%" />
                            </td>
                            <td style="width:33%">
                                <h1>Review</h1>
                                <asp:Button ID="PreviewBtn" runat="server" Text="Download Current Comment" OnClick="PreviewBtn_Click" Width="75%" />
                                <asp:Button ID="ClassReviewBtn" runat="server" Text="Download for Review" OnClick="ClassReviewBtn_Click" Width="75%" />
                            </td>
                            <td style="width:33%">
                                <h1>Publish</h1>
                                <asp:Button ID="DownloadClassBtn" runat="server" Text="Download  Class for Printing" OnClick="DownloadClassBtn_Click" Width="75%" />
                                <asp:Button ID="DownloadAllBtn" runat="server" Text="Download All" OnClick="DownloadAllBtn_Click" Width="75%" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Button ID="ShowPFWBtn" runat="server" Text="Paste from Word" OnClick="ShowPFWBtn_Click" />
                            </td>
                            <td colspan="2">
                                <asp:Panel ID="PFWPanel" Visible="false" runat="server">
                                    Paste text here:<br />
                                    <asp:TextBox ID="PFWInput" runat="server" Width="100%" Height="5cm" TextMode="MultiLine"></asp:TextBox>
                                    <asp:Button ID="DoPFWBtn" runat="server" Text="Finish" OnClick="DoPFWBtn_Click" />
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </article>
            <asp:Panel runat="server" CssClass="progress_container" ID="MessagePanel" Visible="false">
                <article class="success">
                    <header>Operation Successful</header>
                    <asp:Label ID="SuccessMessage" runat="server" Text=""></asp:Label>
                    <asp:Button ID="SuccessOK" runat="server" Text="OK" OnClick="SuccessOK_Click" />
                </article>
            </asp:Panel>
            <asp:Panel ID="HeaderPanel" runat="server" Visible="false">
                <article class="sub_control">
                    <header>Class Header Paragraph</header>
                    <cc1:Editor ID="HeaderEditor" runat="server" Height="10cm" InitialCleanUp="True" NoScript="True" Font-Names="Times New Roman" />
                </article>
            </asp:Panel>
            <asp:Panel ID="StudentPanel" runat="server" Visible ="false">
                <article class="sub_control">
                    <header>
                        <asp:Label ID="StudentNameLabel" runat="server" Text="student's Comment"></asp:Label></header>
                    <!-- Grade Selector! -->
                    <uc1:CommentGradeBar runat="server" id="CommentGrades" />
                    <hr />
                    <cc1:Editor ID="StudentEditor" runat="server" Height="10cm" InitialCleanUp="True" NoScript="True" Font-Names="Times New Roman" />
                </article>
            </asp:Panel>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="CommentPanel" runat="server">
    <ProgressTemplate>
        
            <div class="progress_container">
        <article class="progress">
            Working...
        </article></div>
    </ProgressTemplate>
</asp:UpdateProgress>

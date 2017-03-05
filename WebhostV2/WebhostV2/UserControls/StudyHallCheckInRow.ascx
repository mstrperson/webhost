<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudyHallCheckInRow.ascx.cs" Inherits="WebhostV2.UserControls.StudyHallCheckInRow" %>

<asp:ScriptManagerProxy ID="RowSMP" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="RowUpdatePanel" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="StudentIdField" runat="server" />
        <asp:HiddenField ID="DateField" runat="server" />
        <table class="studyhall">
            <tr>
                <td rowspan="2">
                    <asp:Label ID="NameLabel" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="AttendanceDDL" runat="server"></asp:DropDownList>
                </td>
                <td rowspan="2">
                    <asp:Panel ID="LibraryPanel" runat="server">
                        <table>
                            <tr>
                                <td><asp:Label ID="LibraryLabel" runat="server" Text="Library Pass Available"></asp:Label></td>
                                <td><asp:Button ID="SignOutBtn" runat="server" Text="Sign Out to Library" OnClick="SignOutBtn_Click" />
                                    <asp:Button ID="CancelPassBtn" runat="server" OnClick="CancelPassBtn_Click" Text="Cancel Pass" Visible="False" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2"><asp:TextBox ID="SignedOutInfo" Width="100%" Visible="false" TextMode="MultiLine" runat="server"></asp:TextBox></td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="MarkBtn" runat="server" OnClick="MarkBtn_Click" Text="Mark" />
                </td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>

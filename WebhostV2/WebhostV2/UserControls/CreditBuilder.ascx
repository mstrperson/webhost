<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreditBuilder.ascx.cs" Inherits="WebhostV2.UserControls.CreditBuilder" %>

<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <article class="control">
            <header>Credit Builder</header>
            <table class="bordered">
                <tr>
                    <th>Student</th><th>Credit Type</th><th>Requirement Filled</th><th>Credit Value</th>
                </tr>
                <tr>
                    <td>
                        <ajaxToolkit:ComboBox ID="StudentSelect" runat="server"></ajaxToolkit:ComboBox>
                    </td>
                    <td>
                        <asp:RadioButtonList ID="TransferWaiverSelect" runat="server">
                            <asp:ListItem Text="Transfer Credit" Value="Transfer" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Credt Waiver" Value="Waiver"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                    <td>
                        <asp:DropDownList ID="CreditTypeDDL" runat="server"></asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList ID="CreditValueDDL" runat="server"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <th>Notes</th>
                    <td colspan="3">
                        <asp:TextBox ID="NotesInput" TextMode="MultiLine" runat="server" Width="450px" Height="150px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Button ID="SubmitBtn" runat="server" Text="Create Credit" OnClick="SubmitBtn_Click" />
                    </td>
                </tr>
            </table>
        </article>
        <asp:Panel ID="SuccessPanel" CssClass="progress_container" Visible="false" runat="server">
            <article class="success">
                Credit Saved Successfully.<br />
                <asp:Button ID="OKBtn" runat="server" Text="Ok" OnClick="OKBtn_Click" />
            </article>
        </asp:Panel>
        <asp:Panel ID="ErrorPanel" CssClass="progress_container" runat="server" Visible="false">
            <article class="error">
                Credit was not saved successfully.
                <asp:Label ID="ErrorLabel" runat="server" Text="[Error]"></asp:Label><br />
                <asp:Button ID="DismissBtn" runat="server" Text="Ok" OnClick="DismissBtn_Click" />
            </article>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreditEditor.ascx.cs" Inherits="WebhostV2.UserControls.CreditEditor" %>
<article class="subcontrol">
    <table class="bordered">
        <tr>
            <th>Credit Type</th><td><asp:DropDownList ID="CreditTypeDDL" runat="server"></asp:DropDownList></td>
        </tr>
        <tr>
            <th>Credit Value</th><td><asp:DropDownList ID="CreditValueDDL" runat="server"></asp:DropDownList></td>
        </tr>
        <tr>
            <th>Notes</th><td><asp:TextBox ID="NotesInput" runat="server" TextMode="MultiLine" Width="450px" Height="150px"></asp:TextBox></td>
        </tr>
        <tr>
            <th colspan="2">
                <asp:CheckBox ID="UpdateAll" runat="server" Checked="True" Text="Update for All Students in this section." />
                <asp:Button ID="SubmitBtn" runat="server" Text="Save Changes" OnClick="SubmitBtn_Click" />&nbsp;
                <asp:Button ID="DelBtn" runat="server" OnClick="DelBtn_Click" Text="Delete Credit" />
            </th>
        </tr>
    </table>
    <asp:Panel ID="SuccessPanel" runat="server" Visible="false">
        <article class="success">
            <asp:Label ID="SuccessMessage" runat="server" Text=""></asp:Label>
            <asp:Button ID="SuccessConfirm" runat="server" Text="OK" OnClick="SuccessConfirm_Click" />
        </article>
    </asp:Panel>
    <asp:Panel ID="ErrorPanel" runat="server" Visible="false">
        <article class="success">
            <asp:Label ID="ErrorMessage" runat="server" Text=""></asp:Label>
            <asp:Button ID="ErrorConfirm" runat="server" Text="OK" OnClick="ErrorConfirm_Click" />
        </article>
    </asp:Panel>
</article>
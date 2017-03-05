<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentCreditReport.ascx.cs" Inherits="WebhostV2.UserControls.StudentCreditReport" %>
<%@ Register Src="~/UserControls/CreditEditor.ascx" TagPrefix="uc1" TagName="CreditEditor" %>

<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <article class="control">
            <header>Credit Report for <asp:Label ID="StudentNameLabel" runat="server" Text="[Select an Advisee]"></asp:Label></header>
            <asp:Table ID="CreditTable" CssClass="bordered" runat="server"></asp:Table>
            <asp:Panel ID="EditPanel" Visible="false" runat="server">
                <article class="subcontrol">
                    <header>Select Credit to Edit</header>
                    <asp:DropDownList ID="CreditSelectDDL" runat="server"></asp:DropDownList><asp:Button ID="EditCreditBtn" runat="server" Text="Edit" OnClick="EditCreditBtn_Click" />
                    <uc1:CreditEditor runat="server" id="CreditEditor1"/>
                </article>
            </asp:Panel>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>

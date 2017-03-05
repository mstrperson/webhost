<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserCreation.ascx.cs" Inherits="WebhostV2.UserControls.UserCreation" %>

<%@ Register src="StudentGroupSelector.ascx" tagname="StudentGroupSelector" tagprefix="uc1" %>
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UserCreationPanel" runat="server">
    <ContentTemplate>
        <article class="control">
            <header>Select New Students&nbsp;&nbsp;&nbsp;&nbsp; <asp:Button ID="GetAccountsBtn" runat="server" Text="Download CSVs" OnClick="GetAccountsBtn_Click" />
            </header>
            <asp:CheckBoxList ID="StudentSelectCBL" runat="server" RepeatColumns="4" AutoPostBack="True"></asp:CheckBoxList>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="UserCreationProgress" AssociatedUpdatePanelID="UserCreationPanel" runat="server">
    <ProgressTemplate>
        Generating CSV Files...
    </ProgressTemplate>
</asp:UpdateProgress>

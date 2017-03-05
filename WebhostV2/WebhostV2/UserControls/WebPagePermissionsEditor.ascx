<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebPagePermissionsEditor.ascx.cs" Inherits="WebhostV2.UserControls.WebPagePermissionsEditor" %>
<%@ Register Src="~/UserControls/PermissionSelector.ascx" TagPrefix="uc1" TagName="PermissionSelector" %>

<%@ Register src="TagSelector.ascx" tagname="TagSelector" tagprefix="uc2" %>

<asp:ScriptManagerProxy ID="WebPagePermissionSMP" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="WebPagePermissionPanel" runat="server">
    <ContentTemplate>
        
        <nav class="control">
            <header>Page Required Permissions</header>
            <div id="permissions" style="visibility: hidden">
                <asp:TextBox ID="TitleInput" runat="server"></asp:TextBox>
                <uc1:PermissionSelector runat="server" id="PagePermissionSelector" /><br />
                <asp:Button ID="SetPermissionsBtn" runat="server" Text="Save Permissions" OnClick="SetPermissionsBtn_Click" />
            </div>
        </nav>
        <nav class="control">
            <header>Tags</header>
            <div id="tags" style="visibility: hidden">
                <uc2:TagSelector ID="TagSelector1" runat="server" />
                <asp:Button ID="SaveTagsBtn" runat="server" Text="Save Tags" OnClick="SaveTagsBtn_Click" />
            </div>
        </nav>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="WebPagePermissionUP" runat="server">
    <ProgressTemplate>
        
            <div class="progress_container">
        <article class="progress">
            Working...
        </article></div>
    </ProgressTemplate>
</asp:UpdateProgress>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentSignupSelector.ascx.cs" Inherits="WebhostV2.UserControls.StudentSignupSelector" %>

<%@ Register src="StudentSignup.ascx" tagname="StudentSignup" tagprefix="uc1" %>
<asp:HiddenField ID="WeekendIdField" runat="server" />
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
<article class="control">
    <header>
        <asp:Label ID="WeekendLabel" runat="server"></asp:Label>
    </header>
    <asp:DropDownList ID="ActivityList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ActivityList_SelectedIndexChanged"></asp:DropDownList>
    <hr />
    <uc1:StudentSignup ID="SignupCtrl" runat="server" />
    <hr />
    <details>
        <summary>Weekend Activities:</summary>
        <asp:Table ID="ActivitiesTable" runat="server"></asp:Table>
    </details>
</article>
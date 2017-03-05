<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TextEditor.ascx.cs" Inherits="WebhostV2.UserControls.TextEditor" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HtmlEditor" TagPrefix="cc1" %>
<asp:ScriptManagerProxy ID="TextEditSMP" runat="server"></asp:ScriptManagerProxy>
<article class="control">
    <cc1:Editor ID="TheEditor" runat="server" />
</article>
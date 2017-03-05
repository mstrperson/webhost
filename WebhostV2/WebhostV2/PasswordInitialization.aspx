<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="PasswordInitialization.aspx.cs" Inherits="WebhostV2.PasswordInitialization" %>
<%@ Register src="UserControls/PasswordResetForm.ascx" tagname="PasswordResetForm" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    Welcome to Dublin School Webhost
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <article class="control">
        <div>
            Welcome to your new Dublin School Account.<br />
            You may notice on the left sidebar, a number of links.  Weekend Signups will be of particular interest to boarding students!
            For now though, let's leave those alone.<br />
            I will now walk you through setting up your new Dublin School Gmail account.  For security purposes, Dublin School requires 
            a fairly strong password.  I'm guessing that you really don't want to memorize the random password that I generated for you.<br />
            Here are the rules:
            <ul>
                <li>Minimum of 8 Characters</li>
                <li>Cannot contain more than 2 characters in order of your name or user name.</li>
                <li>Must contain at least 3 of the following 4 types of characters.<ul>
                    <li>lowercase letter</li>
                    <li>UPPERCASE LETTER</li>
                    <li>Number character (0-9)</li>
                    <li>Symbol character (!@#$%^ etc.)</li>
                    </ul>
                </li>
            </ul>
            This means that passwords like "password" and "abc123" are <em>not</em> valid.
        </div>
    </article>
    <uc1:PasswordResetForm ID="PasswordResetForm1" runat="server" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

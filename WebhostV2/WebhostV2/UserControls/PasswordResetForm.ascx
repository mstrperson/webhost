<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PasswordResetForm.ascx.cs" Inherits="WebhostV2.UserControls.PasswordResetForm" %>
<asp:ScriptManagerProxy ID="PwdResetSMP" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="PwdResetPanel" runat="server">
    <ContentTemplate>
        <article class="control">
            <header>Reset your Password</header>
            <details>
                <summary>Help me think of a password...</summary>
                There are many tricks to figuring out what a good password that is easy to remeber.  It also differs from person to person.
                For some people, it may be perfectly easy to just memorize a string of fairly random characters.  Other people prefer 
                patterns on the keyboard.  Both of these methods are good for generating passwords that are very random looking.  These
                methods, however, are not easy for everyone--and if you can't remember your password, it's not very good!<br />
                For the sake of simplicity, here are some things to avoid:
                <ul>
                    <li>single dictionary words--really bad!<ul>
                        <li>spelling it backwards still counts</li>
                    </ul></li>
                    <li>common abreviations</li>
                    <li>common character substitutions (like @ for a, or 1 for l)</li>
                </ul>
                A good password is something that is somewhat significant to you, but a random person on the street will see nonsense.
                If you want to use a formula, here's a guide to making a fairly good password.  You'll probably want some scratch paper
                to write on...
                <ol>
                    <li>Start with a short phrase that has some signifigance to you.  Maybe 4 or 5 words.  ( "My cat has 7 toes." )<ul>
                        <li>if you can incorporate a number into the phrase, you knock out another password criteria!</li>
                    </ul></li>
                    <li>Take letters from the phrase so that you can remember them by remembering the phrase. ( "mch7t." )</li>
                    <li>Capitalize the letters in significant words. ("mCh7T.")</li>
                    <li>You can use the punctuation at the end as your symbol character--it doesn't have to stay a period though! ( "mCh7T!" )</li>
                    <li>Make sure you have at least 8 characters.  My example is too short unless I count the quotation marks as part of the password too.</li>
                    <li>If you don't have enough characters, you can throw in additional letters from the words that made up your phrase--try to avoid whole words though.  Spaces can be included too!</li>
                </ol>
            </details>
            <table class="bordered">
                <tr>
                    <th>User Name:</th>
                    <th>
                        <asp:Label ID="UserNameLabel" runat="server" Text=""></asp:Label>
                    </th>
                </tr>
                <tr>
                    <th>Old Password:</th>
                    <td>
                        <asp:TextBox ID="OldPwdInput" runat="server" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <th>New Password:</th>
                    <td>
                        <asp:TextBox ID="NewPwdInput" runat="server" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <th>Retype New Password:</th>
                    <td>
                        <asp:TextBox ID="RepeatPwdInput" runat="server" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="ResetBtn" runat="server" Text="Set your Password" OnClick="ResetBtn_Click" />
                    </td>
                </tr>
            </table>
        </article>
        <asp:Panel ID="ErrorPanel" runat="server" Visible="false" CssClass="progress_container">
            <article class="error">
                <header>There was a problem!</header>
                <asp:TextBox ID="ErrorMessage" runat="server" ReadOnly="true" TextMode="MultiLine" CssClass="error"></asp:TextBox>
                <asp:Button ID="DismissBtn" runat="server" Text="Ok" OnClick="DismissBtn_Click" />
            </article>
        </asp:Panel>
        <asp:Panel ID="SuccessPanel" CssClass="progress_container" runat="server" Visible="False">
            <article class="success">
                <asp:Label ID="ADSuccessMessage" runat="server"></asp:Label><br />
                <asp:Label ID="GoogleSuccessMessage" runat="server"></asp:Label><br />
                <asp:Button ID="RegisterWifi" runat="server" Text="Go to Wifi Registration Page" OnClick="RegisterWifi_Click" /><br />
                <asp:Button ID="DoneBtn" runat="server" Text="Go to Gmail" OnClick="DoneBtn_Click" />
            </article>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="PwdResetUP" runat="server">
    <ProgressTemplate>
            <div class="progress_container">
        <article class="progress">
            Processing...
        </article></div>
    </ProgressTemplate>
</asp:UpdateProgress>

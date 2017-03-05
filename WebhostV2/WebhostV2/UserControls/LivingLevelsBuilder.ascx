<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LivingLevelsBuilder.ascx.cs" Inherits="WebhostV2.UserControls.LivingLevelsBuilder" %>
<%@ Register Src="~/UserControls/StudentGroupSelector.ascx" TagPrefix="uc1" TagName="StudentGroupSelector" %>

<asp:ScriptManagerProxy ID="LivingLevelsSMP" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="LivingLevelUP" runat="server">
    <ContentTemplate>
        <article class="control">
            <table class="bordered">
                <tr>
                    <td>
                        <article class="sub_control">
                            <header>Independent</header>
                            <uc1:StudentGroupSelector runat="server" ID="IndependentSelector" />
                        </article>
                    </td>
                </tr>
                <tr>
                    <td>
                        <article class="sub_control">
                            <header>Library</header>
                            <uc1:StudentGroupSelector runat="server" ID="LibrarySelector" />
                        </article>
                    </td>
                </tr>
                <tr>
                    <td>
                        <article class="sub_control">
                            <header>Supervised</header>
                            <uc1:StudentGroupSelector runat="server" ID="SupervisedSelector" />
                        </article>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="SaveBtn" runat="server" Text="  Save  " OnClick="SaveBtn_Click" />
                    </td>
                </tr>
            </table>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>

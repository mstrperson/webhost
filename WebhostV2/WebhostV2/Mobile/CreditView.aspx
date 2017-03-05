<%@ Page Title="" Language="C#" MasterPageFile="~/Mobile/Mobile.Master" AutoEventWireup="true" CodeBehind="CreditView.aspx.cs" Inherits="WebhostV2.Mobile.CreditView" %>

<%@ Register Src="~/UserControls/StudentCreditReport.ascx" TagPrefix="uc1" TagName="StudentCreditReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    Credit View
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="NavPlaceHolder" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <article class="control">
                <header>Advisees</header>
                <ajaxToolkit:ComboBox ID="AdviseeSelectCB" runat="server"></ajaxToolkit:ComboBox>
                <asp:Button ID="SelectAdviseeBtn" runat="server" Text="Load Credits" OnClick="SelectAdviseeBtn_Click" />
            </article>
            <uc1:StudentCreditReport runat="server" ID="StudentCreditReport1" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

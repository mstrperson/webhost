<%@ Page Title="" Language="C#" MasterPageFile="~/Mobile/Mobile.Master" AutoEventWireup="true" CodeBehind="UpdateInformation.aspx.cs" Inherits="WebhostV2.Mobile.UpdateInformation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="NavPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainPlaceHolder" runat="server">
    <asp:UpdatePanel runat="server" ID ="UpdatePanelI">
        <ContentTemplate>
            <article class="control">
                <header>Aditional Information is required.</header>
                <asp:Label runat="server" ID="ClassInfoLabel"></asp:Label>
                <asp:Table runat="server" ID ="UpdateTable" CssClass="bordered"></asp:Table>
                <asp:Button runat="server" ID="SubmitNoteBtn" Text="Submit" OnClick="SubmitNoteBtn_Click" />
                <asp:Button runat="server" ID="CancelNoteBtn" Text="Cancel" OnClick="CancelNoteBtn_Click" />
            </article>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress runat="server" ID ="UpdateProgressI" AssociatedUpdatePanelID="UpdatePanelI">
        <ProgressTemplate>
            <div id="progress_container">
                <article class="progress">
                    Working....
                </article>
            </div>     
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

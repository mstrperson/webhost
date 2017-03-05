<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="WebhostV2.Home" %>
<%@ Register src="UserControls/StandardNavigationControls.ascx" tagname="StandardNavigationControls" tagprefix="uc1" %>
<%@ Register src="UserControls/AdviseeSchedulesView.ascx" tagname="AdviseeSchedulesView" tagprefix="uc1" %>
<%@ Register Src="~/UserControls/DutyScheduleView.ascx" TagPrefix="uc1" TagName="DutyScheduleView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    Dublin School Webhost
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <ajaxToolkit:TabContainer ID="HomeTabs" runat="server">
                <ajaxToolkit:TabPanel ID="AdviseeTab" HeaderText="Advisees" TabIndex="0" runat="server">
                    <ContentTemplate>
                        <uc1:AdviseeSchedulesView ID="AdviseeSchedulesView1" runat="server" />
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="WeekendTab" HeaderText="Weekend Schedule" TabIndex="1" runat="server">
                    <ContentTemplate>
                        <uc1:DutyScheduleView runat="server" id="DutyScheduleView1" />
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
        </ContentTemplate>
    </asp:UpdatePanel>
    
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

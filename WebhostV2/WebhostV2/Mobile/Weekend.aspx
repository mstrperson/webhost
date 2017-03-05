<%@ Page Title="" Language="C#" MasterPageFile="~/Mobile/Mobile.Master" AutoEventWireup="true" CodeBehind="Weekend.aspx.cs" Inherits="WebhostV2.Mobile.Weekend" %>

<%@ Register Src="~/UserControls/DutyScheduleView.ascx" TagPrefix="uc1" TagName="DutyScheduleView" %>
<%@ Register Src="~/UserControls/StudentSignup.ascx" TagPrefix="uc1" TagName="StudentSignup" %>
<%@ Register Src="~/UserControls/WeekendTripAttendanceList.ascx" TagPrefix="uc1" TagName="WeekendTripAttendanceList" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    This Weekend
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="NavPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainPlaceHolder" runat="server">
    <asp:UpdatePanel ID="WeekendUpdatePanel" runat="server">
        <ContentTemplate>
            <ajaxToolkit:TabContainer CssClass="tab_container" ID="WeekendTabs" runat="server" Width="100%" ActiveTabIndex="0">
                <ajaxToolkit:TabPanel ID="WeekendViewTab" runat="server" HeaderText="Weekend Schedule">
                    <ContentTemplate>
                        <uc1:DutyScheduleView runat="server" ID="DutyScheduleView1" />
                    </ContentTemplate>
                    <HeaderTemplate>
                        <header class="tab">
                            Weekend Schedule
                        </header>
                    </HeaderTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="StudentSignupTab" runat="server" HeaderText="Signups" TabIndex="1">
                    <ContentTemplate>
                        <asp:Panel ID="NotAvailablePanel" Visible="False" runat="server">
                            <article class="control">
                                <header>Signups are not Available Yet.</header>
                                Signups will be available at 11:30 on Friday.
                            </article>
                        </asp:Panel>
                        <asp:Panel ID="SignupPanel" Visible="False" runat="server">
                            <uc1:StudentSignup runat="server" ID="StudentSignup1" />
                        </asp:Panel>
                        
                    </ContentTemplate>
                    <HeaderTemplate>
                        <header class="tab">
                            Signups
                        </header>
                    </HeaderTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="DutyMemberSingupView" runat="server" TabIndex="2" HeaderText="Student Signups">
                    <ContentTemplate>
                        <uc1:WeekendTripAttendanceList runat="server" ID="WeekendTripAttendanceList1" />

                    </ContentTemplate>
                    <HeaderTemplate>
                        <header class="tab">
                            Student Signups
                        </header>
                    </HeaderTemplate>
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="WeekendUpdatePanel" runat="server">
        <ProgressTemplate>
            <div class="progress_container">
            <article class="progress">
                Working...
            </article></div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

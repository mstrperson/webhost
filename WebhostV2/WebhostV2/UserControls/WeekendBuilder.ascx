<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WeekendBuilder.ascx.cs" Inherits="WebhostV2.UserControls.WeekendBuilder" %>
<%@ Register src="WeekendItem.ascx" tagname="WeekendItem" tagprefix="uc1" %>
<%@ Register Src="~/UserControls/WeekendDutyItem.ascx" TagPrefix="uc1" TagName="WeekendDutyItem" %>

<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" OnLoad="UpdatePanel1_Load">
    <ContentTemplate>
        <article class="control">
            <header><asp:Label ID="WeekendLabel" runat="server">Unclaimed Weekend!</asp:Label></header>
    
            <asp:HiddenField ID="IDField" runat="server" />
            <asp:HiddenField ID="DutyTeamIDField" runat="server" />
            Friday:
            <asp:TextBox ID="StartDate" runat="server" ToolTip="Friday's Date" AutoPostBack="True"></asp:TextBox>
            &nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="PublishCalendarBtn" runat="server" OnClick="PublishCalendarBtn_Click" Text="Publish Weekend on Google Calendar" Visible="False" />
            <asp:Panel ID="NewWeekendPnl" runat="server">
                <asp:Button ID="StartBtn" runat="server" OnClick="StartBtn_Click" Text="Start A Blank Weekend Schedule" />
                &nbsp; Or&nbsp;
                <asp:DropDownList ID="TemplateDDL" runat="server">
                </asp:DropDownList>
                <asp:Button ID="LoadTemplateBtn" runat="server" OnClick="LoadTemplate_Click" Text="Copy a Previous Schedule Template" />
            </asp:Panel>
&nbsp;<ajaxToolkit:CalendarExtender ID="StartDate_CalendarExtender" runat="server" TargetControlID="StartDate" Enabled="True" FirstDayOfWeek="Monday" PopupPosition="Right" TodaysDateFormat="mm/dd/yyyy">
            </ajaxToolkit:CalendarExtender>
            <asp:Panel ID="BuilderPanel" runat="server" Visible="False">
                <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" Width="100%" ActiveTabIndex="1">
                    <ajaxToolkit:TabPanel ID="ActivityTab" runat="server" TabIndex="0" HeaderText="Student Activities">
                        <ContentTemplate>
                            <uc1:WeekendItem ID="WeekendItem1" runat="server" />
                        </ContentTemplate>                        
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel ID="DutyTab" runat="server" TabIndex="1" HeaderText="Faculty Duties">
                        <ContentTemplate>
                            <uc1:WeekendDutyItem runat="server" ID="WeekendDutyItem1" />
                        </ContentTemplate>                        
                    </ajaxToolkit:TabPanel>
                </ajaxToolkit:TabContainer>               
                <hr />
                <article class="sub_control">
                    <asp:Table ID="ActivitiesTable" runat="server" Width="100%" CssClass="bordered"></asp:Table>
                    <asp:DropDownList ID="DeleteActivityDDL" runat="server" AutoPostBack="True" Width="40%"></asp:DropDownList>
                    <asp:Button ID="DeleteActivityBtn" runat="server" Text=" X " OnClick="DeleteActivityBtn_Click" />
                    <asp:Button ID="EditBtn" runat="server" OnClick="EditBtn_Click" Text="Edit Activity" />
                </article>
                <asp:CheckBox ID="NotesCB" AutoPostBack="true" OnCheckedChanged="NotesCB_CheckedChanged" runat="server" Text="Weekend Notes" Checked="false" />
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="SaveNotes" Visible="false" OnClick="SaveNotes_Click" runat="server" Text="Save Notes" />
                <br />
                <asp:TextBox ID="NotesInput" runat="server" TextMode="MultiLine" Visible="false" Width="100%" Height="3cm"></asp:TextBox>
            </asp:Panel>
        </article>
        <asp:Panel ID="SuccessPanel" CssClass="progress_container" runat="server" Visible="false">
            <article class="success">
                <asp:Label ID="SuccessLabel" runat="server" Text="Cool."></asp:Label><br />
                <asp:Button ID="OKBtn" runat="server" Text="Ok." OnClick="OKBtn_Click" />
            </article>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress runat="server" ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel1"> 
    <ProgressTemplate>
            <div class="progress_container">
        <article class="progress">
            Processing Request...
        </article></div>
    </ProgressTemplate>
</asp:UpdateProgress>
        
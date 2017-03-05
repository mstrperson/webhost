<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WeekendItem.ascx.cs" Inherits="WebhostV2.UserControls.WeekendItem" %>
<%@ Register Src="~/UserControls/TimeSelector.ascx" TagPrefix="uc1" TagName="TimeSelector" %>

<%@ Register src="FacultyGroupSelector.ascx" tagname="FacultyGroupSelector" tagprefix="uc2" %>
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="WeekendIdField" runat="server" />
        <asp:HiddenField ID="ActivityIdField" runat="server" />
        
        <article class="sub_control">
            <header>Weekend Activity
                <asp:Button ID="SaveBtn" runat="server" CssClass="left" Text="Save" OnClick="SaveBtn_Click" />
                <asp:Button ID="DeleteBtn" runat="server" CssClass="right" Text="Delete" OnClick="DeleteBtn_Click" /></header>
            <asp:DropDownList ID="DaySelect" runat="server">
                <asp:ListItem>Friday</asp:ListItem>
                <asp:ListItem>Saturday</asp:ListItem>
                <asp:ListItem>Sunday</asp:ListItem>
            </asp:DropDownList>
                <asp:CheckBox ID="AllDayCB" runat="server" OnCheckedChanged="AllDayCB_CheckedChanged" AutoPostBack="True" /><uc1:TimeSelector runat="server" ID="StartTimeSelector" />
                <asp:CheckBox ID="DurationCB" runat="server" OnCheckedChanged="DurationCB_CheckedChanged" AutoPostBack="True" Checked="True" />
                <uc1:TimeSelector ID="DurationSelector" runat="server" />
                <asp:TextBox ID="ActivityName" runat="server"></asp:TextBox>
            <hr />
            <uc2:FacultyGroupSelector ID="FacultyGroupSelector1" runat="server" />
            <details>
                <summary>
                    Aditional Information: 
                </summary>
                <asp:CheckBox ID="isSignupCB" runat="server" AutoPostBack="True" Checked="true" OnCheckedChanged="isSignupCB_CheckedChanged" Text="Show Students" />
            <summary>
            <asp:DropDownList ID="MaxSignupDL" runat="server">
                <asp:ListItem Value="0">Unlimited</asp:ListItem>
                <asp:ListItem Value="8">Yukon</asp:ListItem>
                <asp:ListItem Value="14">1 Bus</asp:ListItem>
                <asp:ListItem Value="28">2 Busses</asp:ListItem>
            </asp:DropDownList>
            </summary>
                <asp:TextBox ID="DescriptionInput" runat="server" TextMode="MultiLine" Text="Description" Width="100%"></asp:TextBox>
            </details>
        </article>
        <asp:Panel runat="server" CssClass="progress_container" ID="SuccessPanel" Visible="false">
            <article class="success">
                <asp:Label runat="server" ID="SuccessLabel"></asp:Label>
                <asp:Button runat="server" ID="ConfirmBtn" Text="Ok" OnClick="ConfirmBtn_Click" />
            </article>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
        

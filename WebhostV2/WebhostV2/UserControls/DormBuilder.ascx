<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DormBuilder.ascx.cs" Inherits="WebhostV2.UserControls.DormBuilder" %>
<%@ Register Src="~/UserControls/FacultyGroupSelector.ascx" TagPrefix="uc1" TagName="FacultyGroupSelector" %>
<%@ Register Src="~/UserControls/StudentGroupSelector.ascx" TagPrefix="uc2" TagName="StudentGroupSelector" %>



<asp:ScriptManagerProxy ID="DormBuilderSMP" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="DormBuilderPanel" runat="server">
    <ContentTemplate>
        <article class="control">
            <header>Dorm Builder</header>
            <ajaxToolkit:ComboBox ID="DormSelector" runat="server" AutoCompleteMode="SuggestAppend" AutoPostBack="True" OnSelectedIndexChanged="DormSelector_SelectedIndexChanged"></ajaxToolkit:ComboBox>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="SaveBtn" runat="server" OnClick="SaveBtn_Click" Text="Save Changes" />
            &nbsp; Dorm Head:&nbsp;&nbsp;
            <ajaxToolkit:ComboBox ID="DormHeadSelector" runat="server" AutoCompleteMode="SuggestAppend" AutoPostBack="True">
            </ajaxToolkit:ComboBox>
            &nbsp;<hr />
            <article class="sub_control">
                <header>Dorm Parents</header>
                <uc1:FacultyGroupSelector runat="server" ID="DormParentSelector" />
            </article>
            <article class="sub_control">
                <header>Students</header>
                <uc2:StudentGroupSelector runat="server" ID="StudentSelector" />
            </article>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="DormBuilderProgress" runat="server">
    <ProgressTemplate>
            <div class="progress_container">
        <article class="progress">
            Working...
        </article></div>
    </ProgressTemplate>
</asp:UpdateProgress>

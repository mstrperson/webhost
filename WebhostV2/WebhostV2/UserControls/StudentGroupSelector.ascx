<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentGroupSelector.ascx.cs" Inherits="WebhostV2.UserControls.StudentGroupSelector" %>
<asp:ScriptManagerProxy ID="StudentGroupSMP" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="StudentGroupPanel" runat="server" OnLoad="UpdatePanel1_Load">
    <ContentTemplate>
        <asp:HiddenField ID="StudentListField" runat="server" />
        <asp:HiddenField ID="ActiveOnlyField" runat="server" />
        <asp:Label ID="TitleLabel" runat="server"></asp:Label>
        <div id="StudentSelectorDiv">
                    <ajaxToolkit:ComboBox ID="StudentSelector" runat="server" Width="5cm" AutoCompleteMode="SuggestAppend"></ajaxToolkit:ComboBox>

                    <asp:Button ID="AddStudentBtn" runat="server" OnClick="AddStudent_Click" Text="+" />

                    <asp:Table ID="SelectedTable" runat="server" Width="5cm"></asp:Table>
        </div>
        <asp:DropDownList ID="RemoveList" Visible="false" runat="server"></asp:DropDownList><asp:Button ID="RemoveBtn" Visible="false" runat="server" Text="X" OnClick="RemoveBtn_Click" />
    </ContentTemplate>
</asp:UpdatePanel>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FacultyGroupSelector.ascx.cs" Inherits="WebhostV2.UserControls.FacultyGroupSelector" %>

<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" OnLoad="UpdatePanel1_Load">
    <ContentTemplate>
        <asp:HiddenField ID="FacultyListField" runat="server" />
        <asp:HiddenField ID="ActiveOnlyField" runat="server" />
        <div id="FacultySelectorDiv">
            <ajaxToolkit:ComboBox ID="FacultySelector" EnableTheming="false" runat="server" Width="5cm" AutoCompleteMode="SuggestAppend"></ajaxToolkit:ComboBox>
                
            <asp:Button ID="AddFacultyBtn" runat="server" OnClick="AddFaculty_Click" Text=" + " />
               
            <asp:Table ID="SelectedTable" runat="server" Width="5cm"></asp:Table>
         </div>
        <asp:DropDownList ID="RemoveList" Visible="false" runat="server"></asp:DropDownList><asp:Button ID="RemoveBtn" Visible="false" runat="server" Text="X" OnClick="RemoveBtn_Click" />
    </ContentTemplate>
</asp:UpdatePanel>


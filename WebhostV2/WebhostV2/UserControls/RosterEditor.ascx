<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RosterEditor.ascx.cs" Inherits="WebhostV2.UserControls.RosterEditor" %>
<%@ Register src="StudentGroupSelector.ascx" tagname="StudentGroupSelector" tagprefix="uc1" %>
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="SectionIDField" runat="server" />
        <article class="control">
            <header>
                <asp:Label ID="SectionLabel" runat="server" Text="Select a Class to Edit"></asp:Label></header>
            <details>
                <summary>
                    Teachers:
                    <asp:DropDownList ID="RemoveTeacherDDL" runat="server"></asp:DropDownList>
                </summary>
                <asp:Button ID="RemoveTeacher" runat="server" OnClick="RemoveClick" Text="Remove Selected Teacher" />
                <asp:DropDownList ID="TeacherSelect" runat="server"></asp:DropDownList>
                <asp:Button ID="AddTeacherBtn" runat="server" Text="Add Teacher" OnClick="AddTeacherBtn_Click" />
            </details>
            <hr />
            <details>
                <summary>
                    Students:
                </summary>
                <asp:Table ID="StudentTable" runat="server" BorderStyle="Outset" BorderWidth="1mm" CellPadding="3" CellSpacing="1" GridLines="Horizontal" Width="10cm">
            </asp:Table>
            <summary>
            </summary>
                <asp:Button ID="AddStudentsBtn" runat="server" Text="Set Roster" OnClick="AddStudentsBtn_Click" />
                <uc1:StudentGroupSelector ID="StudentGroupSelector1" runat="server" />
            </details>
            <details>
                <summary>Terms:</summary>
                <asp:Button ID="SetTermBtn" runat="server" Text="Set Terms" OnClick="SetTermBtn_Click" />
                <asp:CheckBoxList ID="TermSelectList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
            </details>
            <details>
                <summary>
                    Block:
                </summary>
                <asp:DropDownList ID="BlockSelectList" runat="server"></asp:DropDownList>
                <span class="input_line">
                    <asp:Button ID="SetBlock" runat="server" Text="Set Block" OnClick="SetBlock_Click" />
                </span>
            </details>
        </article>
    </ContentTemplate>
</asp:UpdatePanel>

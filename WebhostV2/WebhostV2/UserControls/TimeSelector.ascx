<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeSelector.ascx.cs" Inherits="WebhostV2.UserControls.TimeSelector" %>

<span>
    <asp:TextBox ID="TimeInput" Text="5:30" runat="server" Width="50px" OnTextChanged="OnTimeUpdated" AutoPostBack="true"></asp:TextBox>
<asp:DropDownList ID="AMPM" runat="server" OnSelectedIndexChanged="OnTimeUpdated" AutoPostBack="true">
    <asp:ListItem>AM</asp:ListItem>
    <asp:ListItem Selected="True">PM</asp:ListItem>
</asp:DropDownList>
</span>
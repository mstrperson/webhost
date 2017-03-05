<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MorningMeetingSeatClicker.ascx.cs" Inherits="WebhostV2.UserControls.MorningMeetingSeatClicker" %>
<asp:HiddenField ID="StudentIdField" runat="server" />
<asp:HiddenField ID="MarkingField" runat="server" />
<asp:ImageButton ID="MarkingBtn" runat="server" ImageAlign="Middle" ImageUrl="~/images/OK.png" Height="18mm" OnClick="MarkingBtn_Click" />

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MorningMeetingAttendance.aspx.cs" Inherits="WebhostV2.MorningMeetingAttendance" %>

<%@ Register src="UserControls/MorningMeetingAttendanceChart.ascx" tagname="MorningMeetingAttendanceChart" tagprefix="uc1" %>
<%@ Register Src="~/UserControls/WebPagePermissionsEditor.ascx" TagPrefix="uc1" TagName="WebPagePermissionsEditor" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="default.css" rel="stylesheet" type="text/css" />
    <link href="iPad.css" media="screen and (max-width:1133px)" rel="stylesheet" type="text/css" />
    <script>
        function background() {
            var backgroundNumber = Math.floor((Math.random() * 70));
            document.body.style.background = "#749cd0 url('images/Backgrounds/" + backgroundNumber + ".jpg') no-repeat fixed center 0px/100%";
            //alert("'images/Backgrounds/" + backgroundNumber + ".jpg'");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <uc1:MorningMeetingAttendanceChart ID="MorningMeetingAttendanceChart1" runat="server" />
        <uc1:WebPagePermissionsEditor runat="server" ID="WebPagePermissionsEditor" />
    </div>
    </form>
    <script>
        background();
    </script>
</body>
</html>

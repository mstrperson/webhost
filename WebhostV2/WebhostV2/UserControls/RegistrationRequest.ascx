<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RegistrationRequest.ascx.cs" Inherits="WebhostV2.UserControls.RegistrationRequest" %>
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <article class="control">
            <header>Instructions</header>
            In order to use the Wifi at Dublin School, all students must register their devices each year.  
            Both new and returning students must register any device which they wish to use on the Dublin School wifi.<br /><br />
            This form allows you to register ahead of time so that you don't have to wait in line with all your devices on Opening Day.  
            Indicate what kind of device you are regstering and then type in the Wifi MAC Address.  (Depending on your device
            this may also be called the "Physical address", "Hardware Address" or just "Wifi Address").<br /><br />
            The MAC Address will be a series of letters and number 12 digits long (12 Hexadecimal digits).  It may have '-' or ':' separating each two digits.
            On iOS and some Android devices, this address appears directly above the "Bluetooth" address which looks very similar.  
            Make sure to Submit the Wifi Address and not the Bluetooth Address.<br /><br />
            If the device you want to register is not something listed in the Drop-down.  Select "Other" and then type in what kind of device it is into the text box that appears.<br /><br />
            If you need help locating the Wifi MAC Address for your device.  Click the "Help Me find my MAC Address" link after selecting the type of device you have.  
            This will redirect you to a Google Search with appropriate instructions.
        </article>
        <article class="control">
            <header>Device Registration Request</header>
            <table class="bordered">
                <tr>
                    <th style="width:20%">
                        Type of Device
                    </th>
                    <td>
                        <asp:DropDownList ID="DeviceTypeDDL" runat="server" OnSelectedIndexChanged="DeviceTypeDDL_SelectedIndexChanged" Width="100%" AutoPostBack="true">
                            <asp:ListItem>iPad</asp:ListItem>
                            <asp:ListItem>iPhone</asp:ListItem>
                            <asp:ListItem Value="OSX">Mac Computer</asp:ListItem>
                            <asp:ListItem>Android Phone</asp:ListItem>
                            <asp:ListItem>Windows Phone</asp:ListItem>
                            <asp:ListItem Value="Windows">Windows Computer</asp:ListItem>
                            <asp:ListItem>Other</asp:ListItem>
                        </asp:DropDownList>
                        <asp:Label ID="OtherLabel" runat="server" Text="What kind of device is this?  " Visible="false"></asp:Label>
                        <asp:TextBox BorderColor="Black" BorderWidth="1px" BorderStyle="Solid" ID="OtherInput" runat="server" Visible="false"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:LinkButton ID="HelpBtn" runat="server" Text="Help me Find my MAC Address" OnClick="HelpBtn_Click"/>
                    </td>
                </tr>
                <tr>
                    <th style="width:20%">Device MAC Address</th>
                    <td>
                        <asp:TextBox ID="MACAddrInput" runat="server" Width="100%" ToolTip="You can leave out the : or - characters."></asp:TextBox>
                    </td>
                </tr>
            </table>
            <asp:Button ID="SubmitBtn" runat="server" Text="Submit Registration Request" OnClick="SubmitBtn_Click" />
        </article>
        <asp:Panel ID="ErrorPanel" CssClass="progress_container" runat="server" Visible="false">
            <article class="error">
                <header>There was a problem with your submission...</header>
                <asp:Label ID="ErrorMessage" runat="server" Text="Error"></asp:Label>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="ClearErrorBtn" runat="server" Text="Ok." OnClick="ClearErrorBtn_Click" />
            </article>
        </asp:Panel>
        <asp:Panel ID="SuccessPanel" CssClass="progress_container" runat="server" Visible="false">
            <article class="success">
                <header>Request Successful</header>
                Your Registration Request has been submitted successfully.<br />
                <asp:Button ID="OkBtn" runat="server" Text="Fill Out the Form Again for another device." OnClick="OkBtn_Click" />
            </article>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

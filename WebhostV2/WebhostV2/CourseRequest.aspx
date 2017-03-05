<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="CourseRequest.aspx.cs" Inherits="WebhostV2.CourseRequest" %>

<%@ Register src="UserControls/CourseRequestAdmin.ascx" tagname="CourseRequestAdmin" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header_cp" runat="server">
    <p>
        Course Request</p>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="sidebar_cp" runat="server">
       
    
       
    <uc1:CourseRequestAdmin ID="CourseRequestAdmin1" runat="server" OnMasquerade="CourseRequestAdmin1_Masquerade"/>
       
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="main_cp" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <article class="control">
                <header>Student Information</header>
                Student Name:  <asp:DropDownList ID="StudentSelectDDL" runat="server" OnSelectedIndexChanged="StudentSelectDDL_SelectedIndexChanged"></asp:DropDownList>
                <asp:Button ID="SelectStudentbtn" runat="server" OnClick="SelectStudentbtn_Click" Text="Fill out Course Request" />
                <hr />
                <section>
                    Advisors: Please fill in your advisee&#39;s first and second choice for each subject area (and third choice in some cases). Next to the second choice, please indicate whether it is an alternate to the first choice or a course the student would like to take concurrently.
                    <br />
                    <br />
                    Graduation requirements for each department are listed. Those wishing to pursue an Honors Diploma should consult the handbook for specific requirements.</section>
            </article>
            <asp:Panel ID="SignupPanel" runat="server" Visible="false">
                <asp:Panel ID="SeniorProjectPanel" runat="server" Visible="false">
                    <article class="control">
                        <header>Senior Project</header>
                        <section style="font-weight:lighter; font-size: smaller">
                            Students may petition to have Senior Project replace another graduation requirement. Only check this box if you have spoken with your advisor and plan on participating.
                        </section>
                        <asp:CheckBox ID="SeniorProjectCB" runat="server" Text="Yes, I plan on participating in Senior Project." />
                    </article>
                </asp:Panel>
                <asp:Panel ID="EnglishPanel" runat="server">
                    <article class="control">
                        <header>English</header>
                        <section style="font-weight:lighter; font-size: smaller">
                            All students must be enrolled in an English course in every trimester at Dublin School.
                            <br />
                            <asp:CheckBox ID="EnglishFullYearCB" runat="server" Text="Already Enrolled in Full Year English" />
                        </section>
                        <table>
                            <tr>
                                <td>First Choice:</td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="English1DDL" runat="server"></ajaxToolkit:ComboBox></td>
                            </tr>
                            <tr>
                                <td>Second Choice:</td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="English2DDL" runat="server"></ajaxToolkit:ComboBox></td>
                                <td>
                                    <asp:RadioButtonList ID="EnglishSelectionPriority" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Selected="True" Value="1">Alternate</asp:ListItem>
                                        <asp:ListItem Value="0">Concurrent</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                        <hr />
                    </article>
                </asp:Panel>
                <asp:Panel ID="HistoryPanel" runat="server">
                    <article class="control">
                        <header>History</header>
                        <section style="font-weight:lighter; font-size: smaller">
                            Six credits in the two disciplines of history and world language are required. The requirement in history is three credits, including U.S. History. All seniors are required to complete a minimum of one trimester elective in history during their senior year, unless they are already enrolled in a full-year course.<br />
                            <asp:CheckBox ID="HistoryFullYearCB" runat="server" OnCheckedChanged="HistoryFullYearCB_CheckedChanged" Text="Already Enrolled in Full Year History" />
                            <br />
                        
                        </section>
                        <table>
                            <tr>
                                <td>First Choice:</td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="History1DDL" runat="server"></ajaxToolkit:ComboBox></td>
                            </tr>
                            <tr>
                                <td>Second Choice:</td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="History2DDL" runat="server"></ajaxToolkit:ComboBox></td>
                                <td>
                                    <asp:RadioButtonList ID="HistorySelectionPriority" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Selected="True" Value="1">Alternate</asp:ListItem>
                                        <asp:ListItem Value="0">Concurrent</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                        <hr />
                    </article>
                </asp:Panel>
                <asp:Panel ID="WorldLanguagePanel" runat="server">
                    <article class="control">
                        <header>World Language</header>
                        <section style="font-weight:lighter; font-size: smaller">
                            Completion of a minimum of two credits in the same language is required. We strongly encourage students to continue language studies for a third year. (ESL counts as language credit.)<br />
                            <asp:CheckBox ID="WorldLanguageSkip" runat="server" Text="Already Enrolled in a Language or does not need a language." />
                            <br />
                        
                        </section>
                        <table>
                            <tr>
                                <td>First Choice:</td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="WorldLang1DDL" runat="server"></ajaxToolkit:ComboBox></td>
                            </tr>
                            <tr>
                                <td>Second Choice:</td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="WorldLang2DDL" runat="server"></ajaxToolkit:ComboBox>
                                </td>
                                <td>
                                    <asp:RadioButtonList ID="WorldLangPriority" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Selected="True" Value="1">Alternate</asp:ListItem>
                                        <asp:ListItem Value="0">Concurrent</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                        <hr />
                    
                    </article>
                </asp:Panel>
                <asp:Panel ID="SciencePanel" runat="server">
                    <article class="control">
                        <header>Science</header>
                        <section style="font-weight:lighter; font-size: smaller">
                            Seven credits in the two disciplines of mathematics and science are required. Three science credits, including Biology and Chemistry.<br />
                            <asp:CheckBox ID="ScienceFullYearCB" runat="server" Text="Already Enrolled in Full Year Science or No Science." />
                            <br />
                        
                        </section>
                        <table>
                            <tr>
                                <td>First Choice:</td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="Science1DDL" runat="server"></ajaxToolkit:ComboBox></td>

                            </tr>
                            <tr>
                                <td>Second Choice:</td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="Science2DDL" runat="server"></ajaxToolkit:ComboBox></td>
                            
                                <td>

                                    <asp:RadioButtonList ID="SciencePriority" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Selected="True" Value="1">Alternate</asp:ListItem>
                                        <asp:ListItem Value="0">Concurrent</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td>Third Choice:</td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="Science3DDL" runat="server"></ajaxToolkit:ComboBox></td>
                                <td>
                                    (if one of your other choices does not run.)
                                </td>
                            </tr>
                        </table>
                        <hr />
                    </article>
                </asp:Panel>
                <asp:Panel ID="MathPanel" runat="server">
                    <article class="control">
                        <header>Math</header>
                        <section style="font-weight:lighter; font-size: smaller">
                            Seven credits in the two disciplines of mathematics and science are required. Mathematics must include Algebra II.<br />
                            <asp:CheckBox ID="MathFullYear" runat="server" Text="Already Enrolled in Full Year Math." />
                            <br />
                        
                        </section>
                        <table>
                            <tr>
                                <td>First Choice:</td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="Math1DDL" runat="server"></ajaxToolkit:ComboBox></td>
                            </tr>
                            <tr>
                                <td>Second Choice:</td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="Math2DDL" runat="server"></ajaxToolkit:ComboBox></td>
                                <td>
                                
                                    <asp:RadioButtonList ID="MathPriority" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Selected="True" Value="1">Alternate</asp:ListItem>
                                        <asp:ListItem Value="0">Concurrent</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                        <hr />
                    </article>
                </asp:Panel>
            
                <asp:Panel ID="TechPanel" runat="server">
                    <article class="control">
                        <header>Technology</header>
                        <section style="font-weight:lighter; font-size: smaller">
                            One trimester elective in Technology is required.<br />
                        
                        </section>
                        <table>
                            <tr>
                                <td>First Choice:</td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="Tech1DDL" runat="server"></ajaxToolkit:ComboBox></td>
                            </tr>
                            <tr>
                                <td>Second Choice:</td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="Tech2DDL" runat="server"></ajaxToolkit:ComboBox></td>
                                <td>
                                
                                    <asp:RadioButtonList ID="TechPriority" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Selected="True" Value="1">Alternate</asp:ListItem>
                                        <asp:ListItem Value="0">Concurrent</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                        <hr />
                    </article>
                </asp:Panel>
                <asp:Panel ID="ArtPanel" runat="server">
                    <article class="control">
                        <header>Arts</header>
                        <section style="font-weight:lighter; font-size: smaller">
                            Two credits in visual art, music, dance or theater elective courses are required.
                            <br />
                        
                        </section>
                        <table>
                            <tr>
                                <td>First Choice:</td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="Art1DDL" runat="server"></ajaxToolkit:ComboBox></td>
                            </tr>
                            <tr>
                                <td>Second Choice:</td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="Art2DDL" runat="server"></ajaxToolkit:ComboBox></td>
                                <td>
                                    <asp:RadioButtonList ID="ArtPriority" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Selected="True" Value="1">Alternate</asp:ListItem>
                                        <asp:ListItem Value="0">Concurrent</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td>Third Choice:</td>
                                <td>
                                    <ajaxToolkit:ComboBox ID="Art3DDL" runat="server"></ajaxToolkit:ComboBox></td>
                                <td>
                                    <asp:RadioButtonList ID="Art3Priority" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Selected="True" Value="1">Alternate</asp:ListItem>
                                        <asp:ListItem Value="0">Concurrent</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                        <hr />
                    </article>
                </asp:Panel>
                <asp:Panel ID="ESLLSPPanel" runat="server">
                    <article class="control">
                        <header>ESL and LSP</header>
                        <asp:CheckBox ID="ESL" runat="server" Text="I am enrolled in ESL"/>
                        <br />
                        <asp:CheckBox ID="LSP" runat="server" Text="I am enrolled in Tutorial"/>
                    </article>
                </asp:Panel>
                <article class="control">
                    <header>Notes</header>
                    <asp:TextBox ID="NotesInput" runat="server" Height="300px" TextMode="MultiLine" Width="100%"></asp:TextBox><br />
                    <hr />
                    <span style="text-align:center">
                        <asp:Button ID="SubmitBtn" runat="server" Text="Submit Course Request" OnClick="SubmitBtn_Click" /></span><br />
                    You may reload this and edit it again in the future if you wish.
                </article>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    

</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer_cp" runat="server">
</asp:Content>

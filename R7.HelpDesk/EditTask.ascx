<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditTask.ascx.cs" Inherits="R7.HelpDesk.EditTask" %>
<%@ Register Src="Controls/Tags.ascx" TagName="Tags" TagPrefix="uc1" %>
<%@ Register Src="Controls/Comments.ascx" TagName="Comments" TagPrefix="uc2" %>
<%@ Register Src="Controls/Logs.ascx" TagName="Logs" TagPrefix="uc3" %>
<%@ Register Src="Controls/Work.ascx" TagName="Work" TagPrefix="uc4" %>
<asp:Panel ID="pnlEditTask" runat="server" HorizontalAlign="Left">
<table>
<tr>
            <td valign="top"><table class="HelpDesk_MasterTable">
        <tr>
            <td>
                <asp:Image ID="imgNewTicket" runat="server" ImageUrl="~/DesktopModules/R7.HelpDesk/images/layout_add.png" />
                <asp:LinkButton ID="lnkNewTicket" resourcekey="lnkNewTicket" runat="server" OnClick="lnkNewTicket_Click">New Ticket</asp:LinkButton>
            </td>
            <td>
                <asp:Image ID="imgExitingTickets" runat="server" ImageUrl="~/DesktopModules/R7.HelpDesk/images/layout.png" />
                <asp:LinkButton ID="lnkExistingTickets" resourcekey="lnkExistingTickets" runat="server" OnClick="lnkExistingTickets_Click">Existing Tickets</asp:LinkButton>
            </td>
            <td>
                <asp:Image ID="imgAdministrator" runat="server" ImageUrl="~/DesktopModules/R7.HelpDesk/images/cog.png" />
                <asp:LinkButton ID="lnkAdministratorSettings" resourcekey="lnkAdministratorSettings" runat="server"
                    OnClick="lnkAdministratorSettings_Click">Administrator Settings</asp:LinkButton>
            </td>
            <td>
                &nbsp;&nbsp;
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td valign="top" title="Ticket">
                <table>
                    <tr>
                        <td align="center" >
                            <asp:Button ID="btnSave" resourcekey="btnSave" runat="server" CssClass="dnnPrimaryAction" OnClick="btnSave_Click" Text="Save" />
                        </td>
                        <td nowrap="nowrap" colspan="1">
                            <b>
                            <asp:Label ID="lblTicket" runat="server" Font-Bold="True" 
                                resourcekey="lblTicket" Text="Ticket:" />
                            </b>&nbsp;<asp:Label ID="lblTask" runat="server"></asp:Label>
                            &nbsp;<asp:Label ID="lblCreated" runat="server"></asp:Label>
                            &nbsp;<asp:Label ID="lblError" runat="server" Font-Bold="True" Font-Size="X-Small"
                                ForeColor="Red" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <b>
                            <asp:Label ID="lblStatus" runat="server" Font-Bold="True" 
                                resourcekey="lblStatus" Text="Status:" />
                            &nbsp;</b></td>
                        <td>
                            <asp:DropDownList ID="ddlStatus" runat="server">
                        <asp:ListItem resourcekey="ddlStatusAdminNew" Value="New" Text="New" />
                        <asp:ListItem resourcekey="ddlStatusAdminActive" Value="Active" Text="Active" />
                        <asp:ListItem resourcekey="ddlStatusAdminOnHold" Value="On Hold" Text="On Hold" />
                        <asp:ListItem resourcekey="ddlStatusAdminResolved" Value="Resolved" Text="Resolved" />
                        <asp:ListItem resourcekey="ddlStatusAdminCancelled" Value="Cancelled" Text="Cancelled" />
                            </asp:DropDownList>
                            &nbsp;<asp:Label ID="lblAssigned" runat="server" Font-Bold="True" 
                                resourcekey="lblAssigned" Text="Assigned:" />
&nbsp;<asp:DropDownList ID="ddlAssigned" runat="server">
                            </asp:DropDownList>
                            &nbsp; &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <b>
                            <asp:Label ID="lbltxtPriority" runat="server" resourcekey="lbltxtPriority" 
                                Text="Priority:" />
                            &nbsp;</b></td>
                        <td>
                            <asp:DropDownList ID="ddlPriority" runat="server">
                                <asp:ListItem resourcekey="ddlPriorityNormal" Value="Normal" Text="Normal" />
                                <asp:ListItem resourcekey="ddlPriorityHigh" Value="High" Text="High" />
                                <asp:ListItem resourcekey="ddlPriorityLow" Value="Low" Text="Low" />
                            </asp:DropDownList>
                            &nbsp;&nbsp;&nbsp;<b>
                            <asp:Label ID="lbltxtDueDate" runat="server" resourcekey="lbltxtDueDate" 
                                Text="Date Due:" /></b>
                            &nbsp;<b><asp:TextBox ID="txtDueDate" runat="server" Columns="8"></asp:TextBox>
                            </b>
                            <asp:HyperLink ID="cmdtxtDueDateCalendar" runat="server" ImageUrl="~/DesktopModules/R7.HelpDesk/images/calendar.png"></asp:HyperLink>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <b><asp:Label ID="lbltxtName" runat="server" resourcekey="lbltxtName" 
                                Text="Name:" /></b>
&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txtName" runat="server" Columns="50" MaxLength="350"></asp:TextBox>
                            <asp:Label ID="lblName" runat="server" Visible="False"></asp:Label>
                            &nbsp; &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <b><asp:Label ID="lbltxtEmail" runat="server" resourcekey="lbltxtEmail" 
                                Text="Email:" /></b>
&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txtEmail" runat="server" Columns="50" MaxLength="350"></asp:TextBox>
                            <asp:Label ID="lblEmail" runat="server" Visible="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <b><asp:Label ID="lbltxtPhone" runat="server" resourcekey="lbltxtPhone" 
                                Text="Phone:" /></b>
&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txtPhone" runat="server" Columns="20" MaxLength="50"></asp:TextBox>
                            <b>&nbsp;<asp:Label ID="lbltxtEstimateHours" runat="server" 
                                resourcekey="lbltxtEstimateHours" Text="Estimate Hours:" />
&nbsp;<asp:TextBox ID="txtEstimate" runat="server" Columns="2"></asp:TextBox>
                            </b>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <b><asp:Label ID="lbltxtDescription" runat="server" 
                                resourcekey="lbltxtDescription" Text="Description:" /></b>
&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txtDescription" runat="server" Columns="50" MaxLength="50"></asp:TextBox>
                            &nbsp; &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <b>
                            <asp:Label ID="lbltxtStart" runat="server" resourcekey="lbltxtStart" 
                                Text="Start:" />
&nbsp;<asp:TextBox ID="txtStart" runat="server" Columns="8"></asp:TextBox>
                                <asp:HyperLink ID="cmdtxtStartCalendar" runat="server" ImageUrl="~/DesktopModules/R7.HelpDesk/images/calendar.png"></asp:HyperLink>
                                &nbsp;<asp:Label ID="lbltxtComplete" runat="server" 
                                resourcekey="lbltxtComplete" Text="Complete:" />
                            &nbsp;<asp:TextBox ID="txtComplete" runat="server" Columns="8"></asp:TextBox>
                            </b>
                            <asp:HyperLink ID="cmdtxtCompleteCalendar" runat="server" ImageUrl="~/DesktopModules/R7.HelpDesk/images/calendar.png"></asp:HyperLink>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr><td><hr /></td></tr>
        <tr>
            <td valign="top">
                <asp:Panel ID="pnlDetails" runat="server">
                    <table>
                        <tr>
                            <td>
                                <asp:Button ID="btnComments" runat="server" OnClick="btnComments_Click" Text="Comments" resourcekey="btnComments" />
                                <asp:Button ID="btnWorkItems" runat="server" OnClick="btnWorkItems_Click" Text="Work" resourcekey="btnWorkItems" />
                                <asp:Button ID="btnLogs" runat="server" OnClick="btnLogs_Click" Text="Logs" resourcekey="btnLogs" />
                            </td>
                        </tr>
                    </table>
                    <asp:Panel ID="pnlComments" runat="server">
                        <uc2:Comments ID="CommentsControl" runat="server" />
                    </asp:Panel>
                </asp:Panel>
                <asp:Panel ID="pnlWorkItems" runat="server" Visible="False">
                    <uc4:Work ID="WorkControl" runat="server" />
                </asp:Panel>
                <asp:Panel ID="pnlLogs" runat="server" BorderColor="#CCCCCC" BorderStyle="Solid"
                    BorderWidth="1px" Height="300px" ScrollBars="Auto" Width="510px" Visible="False"
                    Wrap="False">
                    <uc3:Logs ID="LogsControl" runat="server" />
                </asp:Panel>
    </table></td> <td valign="top"><p>
    <asp:Image ID="imgTags" runat="server" 
                    ImageUrl="~/DesktopModules/R7.HelpDesk/images/tag_blue.png" />
    <b>
                <asp:Label ID="lbltxtTags" runat="server" resourcekey="lbltxtTags" 
                    Text="Tags:" />
                </b></p>
<uc1:Tags ID="TagsTreeExistingTasks" runat="server" Visible="True" />
                <br />
            </td>
            </tr>
</table>
   
</asp:Panel>
<asp:Label ID="lblDetailsError" runat="server" EnableViewState="False" ForeColor="Red" />


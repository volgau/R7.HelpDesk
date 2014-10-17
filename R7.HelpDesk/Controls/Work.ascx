<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Work.ascx.cs" Inherits="R7.HelpDesk.Work" %>
<asp:Panel ID="pnlInsertComment" runat="server" GroupingText="Insert New Work" Width="540px"
    Font-Size="X-Small" BorderStyle="None">
    <table>
        <tr>
            <td valign="top">
                <asp:TextBox ID="txtComment" runat="server" Columns="50" Rows="2" TextMode="MultiLine"></asp:TextBox>
                &nbsp;
                <asp:Button ID="btnInsertComment" resourcekey="btnInsertComment" runat="server" Font-Bold="True" OnClick="btnInsertComment_Click"
                    Text="Insert" />
                &nbsp;<br />
                <asp:Label ID="lblError" runat="server" EnableViewState="False" ForeColor="Red"></asp:Label>
            </td>
            <td valign="top" align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td valign="top">
                <table align="center">
                    <tr>
                        <td bgcolor="#CCCCCC" rowspan="2">
                            <asp:Label ID="lblStart1" runat="server" Font-Bold="True" Text="Start" resourcekey="lblStart" />
                        </td>
                        <td align="center">
                        <asp:Label ID="lblDate1" runat="server" Font-Bold="False" Text="Date" resourcekey="lblDate" />
                        </td>
                        <td align="center">
                        <asp:Label ID="lblTime1" runat="server" Font-Bold="False" Text="Time" resourcekey="lblTime" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td bgcolor="#CCCCCC" rowspan="2">
                        <asp:Label ID="lblStop1" runat="server" Font-Bold="True" Text="Stop" resourcekey="lblStop" />
                        </td>
                       <td align="center">
                        <asp:Label ID="lblDate2" runat="server" Font-Bold="False" Text="Date" 
                               resourcekey="lblDate" />
                        </td>
                        <td align="center">
                        <asp:Label ID="lblTime2" runat="server" Font-Bold="False" Text="Time" 
                                resourcekey="lblTime" />
                        </td>
                    </tr>
                    <tr>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtStartDay" runat="server" Columns="8"></asp:TextBox>
                            <b>
                                <asp:HyperLink ID="cmdtxtStartCalendar1" runat="server" ImageUrl="~/DesktopModules/R7.HelpDesk/R7.HelpDesk/images/calendar.png"></asp:HyperLink>
                            </b>
                        </td>
                        <td>
                            <asp:TextBox ID="txtStartTime" runat="server" Columns="8"></asp:TextBox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtStopDay" runat="server" Columns="8"></asp:TextBox>
                            <b>
                                <asp:HyperLink ID="cmdtxtStartCalendar2" runat="server" ImageUrl="~/DesktopModules/R7.HelpDesk/R7.HelpDesk/images/calendar.png"></asp:HyperLink>
                            </b>
                        </td>
                        <td>
                            <asp:TextBox ID="txtStopTime" runat="server" Columns="8"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>
            <td align="center" valign="top">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="pnlTableHeader" runat="server">
    <br />
    <style type="text/css">
        .style1
        {
            width: 39px;
        }
        .style2
        {
            width: 158px;
        }
    </style>
    <table cellpadding="0" cellspacing="0" bgcolor="WhiteSmoke" width="540px">
        <tr>
            <td class="style1">
                &nbsp;
            </td>
            <td style="border: 1px solid #CCCCCC" align="center" class="style2">
                <asp:Label ID="lblDescription" runat="server" Font-Bold="True" Width="143px" 
                    Style="margin-left: 0px" resourcekey="lblDescription" Text="Comment" />
            </td>
            <td style="border: 1px solid #CCCCCC" align="center">
                <asp:Label ID="lblUser" runat="server" Width="138px" resourcekey="lblUser" Font-Bold="True" Text="User" />
            </td>
            <td style="border: 1px solid #CCCCCC" align="center">
                <asp:Label ID="lblTime" runat="server" Width="150px" resourcekey="lblTime" Font-Bold="True" Text="Time" />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="pnlExistingComments" runat="server" Height="250px" ScrollBars="Vertical"
    Width="540px">
    <asp:GridView ID="gvComments" runat="server" AutoGenerateColumns="False" DataKeyNames="DetailID"
        DataSourceID="LDSComments" ShowHeader="False" OnRowDataBound="gvComments_RowDataBound"
        OnRowCommand="gvComments_RowCommand" Width="100%" CellPadding="2" 
        CellSpacing="2" GridLines="None">
        <Columns>
            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" Font-Underline="True"
                        CommandArgument='<%# Bind("DetailID") %>' ResourceKey="LinkButton1" CommandName="Select"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Description" SortExpression="Description">
                <ItemTemplate>
                    <asp:Label ID="lblComment" Font-Size="X-Small" runat="server" Text='<%# Bind("Description") %>'
                        Width="144px"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="UserID" SortExpression="UserID">
                <ItemTemplate>
                    <asp:Label ID="gvlblUser" Font-Size="X-Small" runat="server" Text='<%# Bind("UserID") %>'
                        Width="154px"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="InsertDate" SortExpression="InsertDate">
                <ItemTemplate>
                    &nbsp;
                    <asp:Label ID="lblStartTime" runat="server" Text='<%# Bind("StartTime") %>' Visible="False" />
                    <asp:Label ID="lblStopTime" runat="server" Text='<%# Bind("StopTime") %>' Visible="False" />
                    <asp:Label ID="lblTimeSpan" runat="server" Font-Size="XX-Small" Width="148px"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Panel>
<asp:Panel ID="pnlEditComment" runat="server" Visible="False">
    <table>
        <tr>
            <td valign="top" nowrap="nowrap">
                <asp:Image ID="Image3" runat="server" ImageUrl="~/DesktopModules/R7.HelpDesk/R7.HelpDesk/images/application_side_contract.png" />
                <asp:LinkButton ID="lnkBack" resourcekey="lnkBack" runat="server" Font-Underline="True" OnClick="lnkBack_Click" Text="Back" />&nbsp;<asp:Image
                    ID="Image4" runat="server" ImageUrl="~/DesktopModules/R7.HelpDesk/R7.HelpDesk/images/page_add.png" />
                <asp:LinkButton ID="lnkUpdate" resourcekey="lnkUpdate" runat="server" Text="Update" Font-Underline="True"
                    OnClick="lnkUpdate_Click" />
                &nbsp;<asp:Image ID="Image5" runat="server" ImageUrl="~/DesktopModules/R7.HelpDesk/R7.HelpDesk/images/page_delete.png" />
                <asp:LinkButton ID="lnkDelete" resourcekey="lnkDelete" runat="server" OnClientClick='if (!confirm("Are you sure you want to delete?") ){return false;}'
                    Text="Delete" Font-Underline="True" OnClick="lnkDelete_Click" />
            </td>
            <td valign="top" align="right">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td valign="top" colspan="2" style="margin-left: 120px">
                <asp:TextBox ID="txtDescription" runat="server" Columns="60" Rows="10" TextMode="MultiLine"
                    Height="55px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="margin-left: 120px" valign="top">
                <table align="center">
                    <tr>
                        <td bgcolor="#CCCCCC" rowspan="2">
                            <asp:Label ID="lblStart2" runat="server" Font-Bold="True" 
                                resourcekey="lblStart" Text="Start" />
&nbsp;</td>
                        <td align="center">
                            <asp:Label ID="lblDate3" runat="server" Font-Bold="False" resourcekey="lblDate" 
                                Text="Date" />
&nbsp;</td>
                        <td align="center">
                            <asp:Label ID="lblTime3" runat="server" Font-Bold="False" resourcekey="lblTime" 
                                Text="Time" />
                            &nbsp;</td>
                        <td>
                            &nbsp;
                        </td>
                        <td bgcolor="#CCCCCC" rowspan="2">
                            <asp:Label ID="lblStop2" runat="server" Font-Bold="True" resourcekey="lblStop" 
                                Text="Stop" />
&nbsp;</td>
                        <td align="center">
                            <asp:Label ID="lblDate4" runat="server" Font-Bold="False" resourcekey="lblDate" 
                                Text="Date" />
                            &nbsp;</td>
                        <td align="center">
                            <asp:Label ID="lblTime4" runat="server" Font-Bold="False" resourcekey="lblTime" 
                                Text="Time" />
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtStartDayEdit" runat="server" Columns="8"></asp:TextBox>
                            <b>
                                <asp:HyperLink ID="cmdtxtStartCalendar3" runat="server" ImageUrl="~/DesktopModules/R7.HelpDesk/R7.HelpDesk/images/calendar.png"></asp:HyperLink>
                            </b>
                        </td>
                        <td>
                            <asp:TextBox ID="txtStartTimeEdit" runat="server" Columns="8"></asp:TextBox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtStopDayEdit" runat="server" Columns="8"></asp:TextBox>
                            <b>
                                <asp:HyperLink ID="cmdtxtStartCalendar4" runat="server" ImageUrl="~/DesktopModules/R7.HelpDesk/R7.HelpDesk/images/calendar.png"></asp:HyperLink>
                            </b>
                        </td>
                        <td>
                            <asp:TextBox ID="txtStopTimeEdit" runat="server" Columns="8"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;<asp:Label ID="lblUserFooter" runat="server" Font-Bold="True" 
                    Text="User:" resourcekey="lblUserFooter" />
                &nbsp;<asp:Label ID="lblDisplayUser" runat="server"></asp:Label>
                &nbsp;<asp:Label ID="lblInsertDateFooter" runat="server" Font-Bold="True" 
                    Text="Insert Date:" resourcekey="lblInsertDateFooter" />&nbsp;<asp:Label ID="lblInsertDate" runat="server"></asp:Label>
                &nbsp;
                <br />
                <asp:Label ID="lblErrorEditComment" runat="server" EnableViewState="False" ForeColor="Red"></asp:Label>
                <asp:Label ID="lblDetailID" runat="server" Visible="False"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:LinqDataSource ID="LDSComments" runat="server" ContextTypeName="R7.HelpDesk.HelpDeskDALDataContext"
    OrderBy="InsertDate desc" TableName="HelpDesk_TaskDetails" OnSelecting="LDSComments_Selecting">
</asp:LinqDataSource>

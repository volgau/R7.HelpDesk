<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Comments.ascx.cs" Inherits="R7.HelpDesk.Comments" %>
<asp:Panel ID="pnlInsertComment" runat="server" 
    GroupingText="Insert New Comment" width="540px"
    Font-Size="X-Small" BorderStyle="None">
    <table>
        <tr>
            <td valign="top">
                <asp:TextBox ID="txtComment" runat="server" Columns="50" Rows="4" 
                    TextMode="MultiLine"></asp:TextBox>
            </td>
            <td valign="top" align="center">
                <asp:CheckBox ID="chkCommentVisible" runat="server" Checked="True" Font-Size="X-Small"
                    Text="Visible to Requestor" AutoPostBack="True" 
                    oncheckedchanged="chkCommentVisible_CheckedChanged" resourcekey="chkCommentVisible" />
            </td>
        </tr>
        <tr>
            <td colspan="2" nowrap="nowrap">
                &nbsp;<asp:Label ID="lblAttachFile1" runat="server" Font-Bold="True" 
                    resourcekey="lblAttachFile" />
                &nbsp;<asp:FileUpload ID="TicketFileUpload" runat="server" /><hr />
                &nbsp;<asp:Button ID="btnInsertComment" runat="server" Font-Bold="True" 
                    OnClick="btnInsertComment_Click" Text="Insert" resourcekey="btnInsertComment" />
                &nbsp;<asp:Button ID="btnInsertCommentAndEmail" runat="server" Font-Bold="True" 
                    OnClick="btnInsertCommentAndEmail_Click" Text="Insert and Email " resourcekey="btnInsertCommentAndEmail" />
                <br />
                <asp:Label ID="lblError" runat="server" EnableViewState="False" ForeColor="Red"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="pnlTableHeader" runat="server">
<style type="text/css">
    .style1
    {
        width: 43px;
    }
</style><br />
<table cellpadding="0" cellspacing="0" bgcolor="WhiteSmoke" width="540px">
    <tr>
        <td class="style1">
            <asp:Image ID="Image6" runat="server" 
                ImageUrl="~/DesktopModules/R7.HelpDesk/R7.HelpDesk/images/GridSpacer.gif" 
                Visible="False" Width="34px" />
        </td>
        <td style="border: 1px solid #CCCCCC" align="center" Width="23px">
            <asp:Image ID="Image1" runat="server" ImageUrl="~/DesktopModules/R7.HelpDesk/R7.HelpDesk/images/World.png"
                AlternateText="Visible to Requestor" />
        </td>
        <td style="border: 1px solid #CCCCCC" align="center">
            <asp:Label ID="lblDescription" runat="server" Font-Bold="True" 
                resourcekey="lblDescription" Style="margin-left: 0px" Text="Comment" 
                Width="147px" />
        </td>
        <td style="border: 1px solid #CCCCCC" align="center">
            <asp:Label ID="lblUser0" runat="server" Width="138px" Font-Bold="True" 
                resourcekey="lblUser" Text="User"></asp:Label>
        </td>
        <td style="border: 1px solid #CCCCCC" align="center">
            <asp:Label ID="lblTime0" runat="server" Width="157px" Font-Bold="True" 
                resourcekey="lblTime" Text="Time"></asp:Label>
        </td>
    </tr>
</table></asp:Panel>

<asp:Panel ID="pnlExistingComments" runat="server" Height="250px" ScrollBars="Vertical" Width="100%">
    <asp:GridView ID="gvComments" runat="server" AutoGenerateColumns="False" DataKeyNames="DetailID"
        DataSourceID="LDSComments" ShowHeader="False" 
        OnRowDataBound="gvComments_RowDataBound" 
        onrowcommand="gvComments_RowCommand" BorderStyle="None" 
        CellPadding="2" CellSpacing="2" GridLines="None">
        <Columns>
            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" Font-Underline="True" 
                        CommandArgument='<%# Bind("DetailID") %>' CommandName="Select" ResourceKey="LinkButton1"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField >
                <ItemTemplate>
                    <asp:Label ID="lblDetailType" runat="server" Text='<%# Bind("DetailType") %>' 
                        Visible="False" />
                    <asp:CheckBox ID="chkDetailType" runat="server" Checked="true" Enabled="False" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Description" SortExpression="Description">
                <ItemTemplate>
                    <asp:Label ID="lblComment" Font-Size="X-Small" runat="server" Text='<%# Bind("Description") %>'
                        Width="150px"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="UserID" SortExpression="UserID">
                <ItemTemplate>
                    <asp:Label ID="gvlblUser" Font-Size="X-Small" runat="server" Text='<%# Bind("UserID") %>'
                        Width="140px"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="InsertDate" SortExpression="InsertDate">
                <ItemTemplate>
                    <asp:Label ID="lblDate" runat="server" Font-Size="XX-Small" Text='<%# Bind("InsertDate") %>'
                        Width="140px"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Panel>
<asp:Panel ID="pnlEditComment" runat="server" Visible="False">
        <table>
        <tr>
            <td valign="top" nowrap="nowrap">
                <asp:Image ID="Image3" runat="server" 
                    ImageUrl="~/DesktopModules/R7.HelpDesk/R7.HelpDesk/images/application_side_contract.png" />
                <asp:LinkButton ID="lnkBack" runat="server" Font-Underline="True" 
                    onclick="lnkBack_Click" Text="Back" resourcekey="lnkBack" />&nbsp;<asp:Image ID="Image4" 
                    runat="server" ImageUrl="~/DesktopModules/R7.HelpDesk/R7.HelpDesk/images/page_add.png" />
                <asp:LinkButton ID="lnkUpdate" runat="server" Text="Update" 
                    Font-Underline="True" onclick="lnkUpdate_Click" resourcekey="lnkUpdate" />
                <asp:Image ID="ImgEmailUser" runat="server" ImageUrl="~/DesktopModules/R7.HelpDesk/R7.HelpDesk/images/user_suit.png" /><asp:LinkButton 
                    ID="lnkUpdateRequestor" runat="server" Font-Underline="True" 
                    onclick="lnkUpdateRequestor_Click" Text="Update and Email Requestor" resourcekey="lnkUpdateAndEmail" />
&nbsp;<asp:Image ID="Image5" runat="server" 
                    ImageUrl="~/DesktopModules/R7.HelpDesk/R7.HelpDesk/images/page_delete.png" />
                <asp:LinkButton ID="lnkDelete" runat="server" 
                    OnClientClick='if (!confirm("Are you sure you want to delete?") ){return false;}' 
                    Text="Delete" Font-Underline="True" onclick="lnkDelete_Click" resourcekey="lnkDelete" />
            </td>
            <td valign="top" align="right">
                <asp:CheckBox ID="chkCommentVisibleEdit" runat="server" Font-Size="X-Small" 
                    Text="Visible to Requestor" AutoPostBack="True" resourcekey="chkCommentVisible"
                    oncheckedchanged="chkCommentVisibleEdit_CheckedChanged" />
            </td>
        </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:TextBox ID="txtDescription" runat="server" Columns="60" Rows="10" 
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="lblUserFooter" runat="server" Font-Bold="True" 
                    resourcekey="lblUserFooter" Text="User:" />
&nbsp;<asp:Label ID="lblDisplayUser" runat="server"></asp:Label>
                &nbsp;<asp:Label ID="lblInsertDateFooter" runat="server" Font-Bold="True" 
                    resourcekey="lblInsertDateFooter" Text="Insert Date:" />
                &nbsp;<asp:Label ID="lblInsertDate" runat="server"></asp:Label>
                &nbsp;
                  <asp:Panel ID="pnlDisplayFile" runat="server" Visible="False">
                    <span style="font-size: x-small; font-weight: bold;" >
                      <br />
                      <asp:Label ID="lblAttachment" resourcekey="lblAttachment" runat="server" Font-Bold="True" 
                          Text="Attachment:" />
                      &nbsp;<asp:ImageButton ID="imgDelete" runat="server" 
                          ImageUrl="~/DesktopModules/R7.HelpDesk/R7.HelpDesk/images/cancel.png" 
                          ToolTip="Delete Attachment" 
                          OnClientClick='if (!confirm("Are you sure you want to delete?") ){return false;}' 
                          onclick="imgDelete_Click" />
                      &nbsp;</span> <asp:LinkButton ID="lnkFileAttachment" runat="server" Font-Underline="True" 
                          onclick="lnkFileAttachment_Click" 
                          ToolTip="Click here to download this file"></asp:LinkButton>
                         <%--<asp:HyperLink ID="lnkFileAttachment" runat="server">
                          </asp:HyperLink> --%>
                      &nbsp;</asp:Panel>
                <asp:Panel ID="pnlAttachFile" runat="server" Visible="False">
                    <span style="font-size: x-small; font-weight: bold;">
                    <br /> 
                    <asp:Label ID="lblAttachFile2" resourcekey="lblAttachFile2" runat="server" Font-Bold="True" />
                    </span>
                    &nbsp;
                    <asp:FileUpload ID="fuAttachment" runat="server" />
                    </asp:Panel>
                <br />
                <asp:Label ID="lblErrorEditComment" runat="server" EnableViewState="False" 
                    ForeColor="Red"></asp:Label>
                <asp:Label ID="lblDetailID" runat="server" Visible="False"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:LinqDataSource ID="LDSComments" runat="server" ContextTypeName="R7.HelpDesk.HelpDeskDALDataContext"
    OrderBy="InsertDate desc" TableName="HelpDesk_TaskDetails" OnSelecting="LDSComments_Selecting">
</asp:LinqDataSource>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Logs.ascx.cs" Inherits="R7.HelpDesk.Logs" %>
    <asp:LinqDataSource ID="LDSLogs" runat="server" 
        ContextTypeName="R7.HelpDesk.HelpDeskDALDataContext" 
        OrderBy="DateCreated desc" TableName="HelpDesk_Logs" 
    onselecting="LDSLogs_Selecting">
    </asp:LinqDataSource>
        <asp:GridView ID="gvLogs" runat="server" AllowPaging="True" 
        AutoGenerateColumns="False" DataKeyNames="LogID" DataSourceID="LDSLogs" 
        Width="100%" BorderStyle="None" PageSize="10">
        <Columns>
            <asp:TemplateField SortExpression="LogDescription">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("LogDescription") %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                	<asp:Label ID="Label2" runat="server" Text='<%# Bind("LogDescription") %>' />
                    <!-- <asp:TextBox ID="TextBox2" runat="server" Rows="2" 
                        Text='\<\%\# Bind("LogDescription") \%\>' TextMode="MultiLine" Width="350px"></asp:TextBox> -->
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="DateCreated"
                SortExpression="DateCreated" >
            <ItemStyle Wrap="False" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>

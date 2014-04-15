<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Tags.ascx.cs" Inherits="R7.HelpDesk.Tags" %>
<%-- <%@ Assembly Name="System.DirectoryServices, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" %> --%>
<asp:TreeView ID="tvCategories" runat="server" CssClass="HelpDesk_Categories" ExpandDepth="0" OnTreeNodeDataBound="tvCategories_TreeNodeDataBound">
    <SelectedNodeStyle BackColor="#CCCCCC" Font-Bold="False" Font-Underline="False" />
    <DataBindings>
        <asp:TreeNodeBinding DataMember="R7.HelpDesk.Categories" Depth="0"
            TextField="Value" ValueField="Value" />
    </DataBindings>
</asp:TreeView>

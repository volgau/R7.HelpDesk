using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using DotNetNuke.Common;
using DotNetNuke.Security.Roles;
using DotNetNuke.Entities.Users;
using System.Collections;
using System.Drawing;
using DotNetNuke.Services.Localization;

namespace R7.HelpDesk
{
    public partial class AdminSettings : DotNetNuke.Entities.Modules.PortalModuleBase
    {
        List<int> colProcessedCategoryIDs;

		protected override void OnInit (EventArgs e)
		{
			base.OnInit (e);

			linkReturn.NavigateUrl = Globals.NavigateURL ();
		}

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Get Admin Role
                string strAdminRoleID = GetAdminRole();
                // Only show if user is an Administrator
                if (!(UserInfo.IsInRole(strAdminRoleID) || UserInfo.IsInRole("Administrators") || UserInfo.IsSuperUser))
                {
                    pnlAdminSettings.Visible = false;
                    Response.Redirect(Globals.NavigateURL());
                }

                SetView("AdministratorRole");
                DisplayAdminRoleDropDown();

                btnAddNew.Text = Localization.GetString("btnAddNew.Text", LocalResourceFile);
                btnUpdate.Text = Localization.GetString("btnUpdateAdminRole.Text", LocalResourceFile);
            }
        }

        #region SetView
        private void SetView(string ViewName)
        {
            if (ViewName == "AdministratorRole")
            {
                pnlAdministratorRole.Visible = true;
                pnlUploFilesPath.Visible = false;
                pnlTagsAdmin.Visible = false;
                pnlRoles.Visible = false;

                lnkAdminRole.Font.Bold = true;
                lnkAdminRole.BackColor = Color.LightGray;
                lnkUploFilesPath.Font.Bold = false;
                lnkUploFilesPath.BackColor = Color.Transparent;
                lnkTagsAdmin.Font.Bold = false;
                lnkTagsAdmin.BackColor = Color.Transparent;
                lnkRoles.Font.Bold = false;
                lnkRoles.BackColor = Color.Transparent;
            }

            if (ViewName == "UploadedFilesPath")
            {
                pnlAdministratorRole.Visible = false;
                pnlUploFilesPath.Visible = true;
                pnlTagsAdmin.Visible = false;
                pnlRoles.Visible = false;

                lnkAdminRole.Font.Bold = false;
                lnkAdminRole.BackColor = Color.Transparent;
                lnkUploFilesPath.Font.Bold = true;
                lnkUploFilesPath.BackColor = Color.LightGray;
                lnkTagsAdmin.Font.Bold = false;
                lnkTagsAdmin.BackColor = Color.Transparent;
                lnkRoles.Font.Bold = false;
                lnkRoles.BackColor = Color.Transparent;
            }

            if (ViewName == "Roles")
            {
                pnlAdministratorRole.Visible = false;
                pnlUploFilesPath.Visible = false;
                pnlTagsAdmin.Visible = false;
                pnlRoles.Visible = true;

                lnkAdminRole.Font.Bold = false;
                lnkAdminRole.BackColor = Color.Transparent;
                lnkUploFilesPath.Font.Bold = false;
                lnkUploFilesPath.BackColor = Color.Transparent;
                lnkTagsAdmin.Font.Bold = false;
                lnkTagsAdmin.BackColor = Color.Transparent;
                lnkRoles.Font.Bold = true;
                lnkRoles.BackColor = Color.LightGray;
            }

            if (ViewName == "TagsAdministration")
            {
                pnlAdministratorRole.Visible = false;
                pnlUploFilesPath.Visible = false;
                pnlTagsAdmin.Visible = true;
                pnlRoles.Visible = false;

                lnkAdminRole.Font.Bold = false;
                lnkAdminRole.BackColor = Color.Transparent;
                lnkUploFilesPath.Font.Bold = false;
                lnkUploFilesPath.BackColor = Color.Transparent;
                lnkTagsAdmin.Font.Bold = true;
                lnkTagsAdmin.BackColor = Color.LightGray;
                lnkRoles.Font.Bold = false;
                lnkRoles.BackColor = Color.Transparent;
            }
        }
        #endregion

        #region GetAdminRole
        private string GetAdminRole()
        {
            List<HelpDesk_Setting> objHelpDesk_Settings = GetSettings();
            HelpDesk_Setting objHelpDesk_Setting = objHelpDesk_Settings.Where(x => x.SettingName == "AdminRole").FirstOrDefault();

            string strAdminRoleID = "Administrators";
            if (objHelpDesk_Setting != null)
            {
                strAdminRoleID = objHelpDesk_Setting.SettingValue;
            }

            return strAdminRoleID;
        }
        #endregion

        #region GetSettings
        private List<HelpDesk_Setting> GetSettings()
        {
            // Get Settings
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            List<HelpDesk_Setting> colHelpDesk_Setting = (from HelpDesk_Settings in objHelpDeskDALDataContext.HelpDesk_Settings
                                                                  where HelpDesk_Settings.PortalID == PortalId
                                                                  select HelpDesk_Settings).ToList();

            if (colHelpDesk_Setting.Count == 0)
            {
                // Create Default vaules
                HelpDesk_Setting objHelpDesk_Setting1 = new HelpDesk_Setting();

                objHelpDesk_Setting1.PortalID = PortalId;
                objHelpDesk_Setting1.SettingName = "AdminRole";
                objHelpDesk_Setting1.SettingValue = "Administrators";

                objHelpDeskDALDataContext.HelpDesk_Settings.InsertOnSubmit(objHelpDesk_Setting1);
                objHelpDeskDALDataContext.SubmitChanges();

                HelpDesk_Setting objHelpDesk_Setting2 = new HelpDesk_Setting();

                objHelpDesk_Setting2.PortalID = PortalId;
                objHelpDesk_Setting2.SettingName = "UploFilesPath";
				objHelpDesk_Setting2.SettingValue = Server.MapPath("~/DesktopModules/R7.HelpDesk/R7.HelpDesk/Upload");

                objHelpDeskDALDataContext.HelpDesk_Settings.InsertOnSubmit(objHelpDesk_Setting2);
                objHelpDeskDALDataContext.SubmitChanges();

                colHelpDesk_Setting = (from HelpDesk_Settings in objHelpDeskDALDataContext.HelpDesk_Settings
                                           where HelpDesk_Settings.PortalID == PortalId
                                           select HelpDesk_Settings).ToList();
            }

            // Upload Permission
            HelpDesk_Setting UploadPermissionHelpDesk_Setting = (from HelpDesk_Settings in objHelpDeskDALDataContext.HelpDesk_Settings
                                                                         where HelpDesk_Settings.PortalID == PortalId
                                                                         where HelpDesk_Settings.SettingName == "UploadPermission"
                                                                         select HelpDesk_Settings).FirstOrDefault();

            if (UploadPermissionHelpDesk_Setting != null)
            {
                // Add to collection
                colHelpDesk_Setting.Add(UploadPermissionHelpDesk_Setting);
            }
            else
            {
                // Add Default value
                HelpDesk_Setting objHelpDesk_Setting = new HelpDesk_Setting();
                objHelpDesk_Setting.SettingName = "UploadPermission";
                objHelpDesk_Setting.SettingValue = "All";
                objHelpDesk_Setting.PortalID = PortalId;
                objHelpDeskDALDataContext.HelpDesk_Settings.InsertOnSubmit(objHelpDesk_Setting);
                objHelpDeskDALDataContext.SubmitChanges();

                // Add to collection
                colHelpDesk_Setting.Add(objHelpDesk_Setting);
            }

            return colHelpDesk_Setting;
        }
        #endregion

        #region lnkAdminRole_Click
        protected void lnkAdminRole_Click(object sender, EventArgs e)
        {
            SetView("AdministratorRole");
            DisplayAdminRoleDropDown();
        }
        #endregion

        #region lnkUploFilesPath_Click
        protected void lnkUploFilesPath_Click(object sender, EventArgs e)
        {
            SetView("UploadedFilesPath");
            DisplayUploadedFilesPath();
        }
        #endregion

        #region MyRegion
        protected void lnkRoles_Click(object sender, EventArgs e)
        {
            SetView("Roles");
            DisplayRoles();
        }
        #endregion

        #region DisplayAdminRoleDropDown
        private void DisplayAdminRoleDropDown()
        {
            // Get all the Roles
            RoleController RoleController = new RoleController();
            ArrayList colArrayList = RoleController.GetRoles();

            // Create a ListItemCollection to hold the Roles 
            ListItemCollection colListItemCollection = new ListItemCollection();

            // Add the Roles to the List
            foreach (RoleInfo Role in colArrayList)
            {
                if (Role.PortalID == PortalId)
                {
                    ListItem RoleListItem = new ListItem();
                    RoleListItem.Text = Role.RoleName;
                    RoleListItem.Value = Role.RoleID.ToString();
                    colListItemCollection.Add(RoleListItem);
                }
            }

            // Add the Roles to the ListBox
            ddlAdminRole.DataSource = colListItemCollection;
            ddlAdminRole.DataBind();

            // Get Admin Role
            string strAdminRoleID = GetAdminRole();

            try
            {
                // Try to set the role
                ddlAdminRole.SelectedValue = strAdminRoleID;
            }
            catch
            {

            }
        }
        #endregion

        #region DisplayUploadedFilesPath
        private void DisplayUploadedFilesPath()
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            // Uploaded Files Path
            HelpDesk_Setting objHelpDesk_Setting = (from HelpDesk_Settings in objHelpDeskDALDataContext.HelpDesk_Settings
                                                            where HelpDesk_Settings.PortalID == PortalId
                                                            where HelpDesk_Settings.SettingName == "UploFilesPath"
                                                            select HelpDesk_Settings).FirstOrDefault();

            txtUploadedFilesPath.Text = objHelpDesk_Setting.SettingValue;

            // Upload Permissions
            HelpDesk_Setting UploadPermissionHelpDesk_Setting = (from HelpDesk_Settings in objHelpDeskDALDataContext.HelpDesk_Settings
                                                                         where HelpDesk_Settings.PortalID == PortalId
                                                                         where HelpDesk_Settings.SettingName == "UploadPermission"
                                                                         select HelpDesk_Settings).FirstOrDefault();

            ddlUploadPermission.SelectedValue = UploadPermissionHelpDesk_Setting.SettingValue;
        }
        #endregion

        #region lnkTagsAdmin_Click
        protected void lnkTagsAdmin_Click(object sender, EventArgs e)
        {
            SetView("TagsAdministration");
            DisplayHelpDesk();
            tvCategories.CollapseAll();
        }
        #endregion

        #region btnUpdateAdminRole_Click
        protected void btnUpdateAdminRole_Click(object sender, EventArgs e)
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            HelpDesk_Setting objHelpDesk_Setting = (from HelpDesk_Settings in objHelpDeskDALDataContext.HelpDesk_Settings
                                                            where HelpDesk_Settings.PortalID == PortalId
                                                            where HelpDesk_Settings.SettingName == "AdminRole"
                                                            select HelpDesk_Settings).FirstOrDefault();


            objHelpDesk_Setting.SettingValue = ddlAdminRole.SelectedValue;
            objHelpDeskDALDataContext.SubmitChanges();

            lblAdminRole.Text = Localization.GetString("Updated.Text", LocalResourceFile);
        }
        #endregion

        #region btnUploadedFiles_Click
        protected void btnUploadedFiles_Click(object sender, EventArgs e)
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            HelpDesk_Setting UploFilesHelpDesk_Setting = (from HelpDesk_Settings in objHelpDeskDALDataContext.HelpDesk_Settings
                                                                      where HelpDesk_Settings.PortalID == PortalId
                                                                      where HelpDesk_Settings.SettingName == "UploFilesPath"
                                                                      select HelpDesk_Settings).FirstOrDefault();

            UploFilesHelpDesk_Setting.SettingValue = txtUploadedFilesPath.Text.Trim();
            objHelpDeskDALDataContext.SubmitChanges();

            HelpDesk_Setting UploadPermissionHelpDesk_Setting = (from HelpDesk_Settings in objHelpDeskDALDataContext.HelpDesk_Settings
                                                                         where HelpDesk_Settings.PortalID == PortalId
                                                                         where HelpDesk_Settings.SettingName == "UploadPermission"
                                                                         select HelpDesk_Settings).FirstOrDefault();

            UploadPermissionHelpDesk_Setting.SettingValue = ddlUploadPermission.SelectedValue;
            objHelpDeskDALDataContext.SubmitChanges();

            lblUploadedFilesPath.Text = Localization.GetString("Updated.Text", LocalResourceFile);
        }
        #endregion

        // Tags

        #region DisplayHelpDesk
        private void DisplayHelpDesk()
        {
            HelpDeskTree colHelpDesk = new HelpDeskTree(PortalId, false);
            tvCategories.DataSource = colHelpDesk;

            TreeNodeBinding RootBinding = new TreeNodeBinding();
            RootBinding.DataMember = "ListItem";
            RootBinding.TextField = "Text";
            RootBinding.ValueField = "Value";

            tvCategories.DataBindings.Add(RootBinding);

            tvCategories.DataBind();
            tvCategories.CollapseAll();

            // If a node was selected previously select it again
            if (txtCategoryID.Text != "")
            {
                int intCategoryID = Convert.ToInt32(txtCategoryID.Text);
                TreeNode objTreeNode = (TreeNode)tvCategories.FindNode(GetNodePath(intCategoryID));
                objTreeNode.Select();
                objTreeNode.Expand();

                // Expand it's parent nodes
                // Get the value of each parent node
                string[] strParentNodes = objTreeNode.ValuePath.Split(Convert.ToChar("/"));
                // Loop through each parent node
                for (int i = 0; i < objTreeNode.Depth; i++)
                {
                    // Get the parent node
                    TreeNode objParentTreeNode = (TreeNode)tvCategories.FindNode(GetNodePath(Convert.ToInt32(strParentNodes[i])));
                    // Expand the parent node
                    objParentTreeNode.Expand();
                }
            }
            else
            {
                //If there is at least one existing category, select it
                if (tvCategories.Nodes.Count > 0)
                {
                    tvCategories.Nodes[0].Select();
                    txtCategoryID.Text = "0";
                    SelectTreeNode();
                }
                else
                {
                    // There is no data so set form to Add New
                    SetFormToAddNew();
                }
            }

            // If a node is selected, remove it from the BindDropDown drop-down
            int intCategoryNotToShow = -1;
            TreeNode objSelectedTreeNode = (TreeNode)tvCategories.SelectedNode;
            if (objSelectedTreeNode != null)
            {
                intCategoryNotToShow = Convert.ToInt32(tvCategories.SelectedNode.Value);
            }

            BindDropDown(intCategoryNotToShow);
        }
        #endregion

        #region BindDropDown
        private void BindDropDown(int intCategoryNotToShow)
        {
            // Bind drop-down
            CategoriesDropDown colCategoriesDropDown = new CategoriesDropDown(PortalId);
            ListItemCollection objListItemCollection = colCategoriesDropDown.Categories(intCategoryNotToShow);

            // Don't show the currently selected node
            foreach (ListItem objListItem in objListItemCollection)
            {
                if (objListItem.Value == intCategoryNotToShow.ToString())
                {
                    objListItemCollection.Remove(objListItem);
                    break;
                }
            }

            ddlParentCategory.DataSource = objListItemCollection;
            ddlParentCategory.DataTextField = "Text";
            ddlParentCategory.DataValueField = "Value";
            ddlParentCategory.DataBind();
        }
        #endregion

        #region GetNodePath
        private string GetNodePath(int intCategoryID)
        {
            string strNodePath = intCategoryID.ToString();

            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            var result = (from HelpDesk_Categories in objHelpDeskDALDataContext.HelpDesk_Categories
                          where HelpDesk_Categories.CategoryID == intCategoryID
                          select HelpDesk_Categories).FirstOrDefault();

            // Only build a node path if the current level is not the root
            if (result.Level > 1)
            {
                int intCurrentCategoryID = result.CategoryID;

                for (int i = 1; i < result.Level; i++)
                {
                    var CurrentCategory = (from HelpDesk_Categories in objHelpDeskDALDataContext.HelpDesk_Categories
                                           where HelpDesk_Categories.CategoryID == intCurrentCategoryID
                                           select HelpDesk_Categories).FirstOrDefault();

                    strNodePath = CurrentCategory.ParentCategoryID.ToString() + @"/" + strNodePath;

                    var ParentCategory = (from HelpDesk_Categories in objHelpDeskDALDataContext.HelpDesk_Categories
                                          where HelpDesk_Categories.CategoryID == CurrentCategory.ParentCategoryID
                                          select HelpDesk_Categories).FirstOrDefault();

                    intCurrentCategoryID = ParentCategory.CategoryID;
                }
            }

            return strNodePath;
        }
        #endregion

        #region tvCategories_SelectedNodeChanged
        protected void tvCategories_SelectedNodeChanged(object sender, EventArgs e)
        {
            SelectTreeNode();
            ResetForm();
        }
        #endregion

        #region SelectTreeNode
        private void SelectTreeNode()
        {
            if (tvCategories.SelectedNode != null)
            {
                if (tvCategories.SelectedNode.Value != "")
                {
                    var result = (from HelpDesk_Categories in CategoriesTable.GetCategoriesTable(PortalId, false)
                                  where HelpDesk_Categories.CategoryID == Convert.ToInt32(tvCategories.SelectedNode.Value)
                                  select HelpDesk_Categories).FirstOrDefault();

                    txtCategory.Text = result.CategoryName;
                    txtCategoryID.Text = result.CategoryID.ToString();
                    chkRequesterVisible.Checked = result.RequestorVisible;
                    chkSelectable.Checked = result.Selectable;

                    // Remove Node from the Bind DropDown drop-down
                    BindDropDown(result.CategoryID);

                    // Set the Parent drop-down
                    ddlParentCategory.SelectedValue = (result.ParentCategoryID == null) ? "0" : result.ParentCategoryID.ToString();
                    txtParentCategoryID.Text = (result.ParentCategoryID == null) ? "" : result.ParentCategoryID.ToString();
                }
            }
        }
        #endregion

        #region btnUpdate_Click
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            if (btnUpdate.CommandName == "Update")
            {
                var result = (from HelpDesk_Categories in objHelpDeskDALDataContext.HelpDesk_Categories
                              where HelpDesk_Categories.CategoryID == Convert.ToInt32(txtCategoryID.Text)
                              select HelpDesk_Categories).FirstOrDefault();

                result.CategoryName = txtCategory.Text.Trim();

                result.ParentCategoryID = (GetParentCategoryID(ddlParentCategory.SelectedValue) == "0") ? (int?)null : Convert.ToInt32(ddlParentCategory.SelectedValue);
                txtParentCategoryID.Text = (ddlParentCategory.SelectedValue == "0") ? "" : ddlParentCategory.SelectedValue;

                result.Level = (ddlParentCategory.SelectedValue == "0") ? 1 : GetLevelOfParent(Convert.ToInt32(ddlParentCategory.SelectedValue)) + 1;
                result.RequestorVisible = chkRequesterVisible.Checked;
                result.Selectable = chkSelectable.Checked;

                objHelpDeskDALDataContext.SubmitChanges();

                // Update levels off all the Children
                colProcessedCategoryIDs = new List<int>();
                UpdateLevelOfChildren(result);
            }
            else
            {
                // This is a Save for a new Node                

                HelpDesk_Category objHelpDesk_Category = new HelpDesk_Category();
                objHelpDesk_Category.PortalID = PortalId;
                objHelpDesk_Category.CategoryName = txtCategory.Text.Trim();
                objHelpDesk_Category.ParentCategoryID = (GetParentCategoryID(ddlParentCategory.SelectedValue) == "0") ? (int?)null : Convert.ToInt32(ddlParentCategory.SelectedValue);
                objHelpDesk_Category.Level = (ddlParentCategory.SelectedValue == "0") ? 1 : GetLevelOfParent(Convert.ToInt32(ddlParentCategory.SelectedValue)) + 1;
                objHelpDesk_Category.RequestorVisible = chkRequesterVisible.Checked;
                objHelpDesk_Category.Selectable = chkSelectable.Checked;

                objHelpDeskDALDataContext.HelpDesk_Categories.InsertOnSubmit(objHelpDesk_Category);
                objHelpDeskDALDataContext.SubmitChanges();

                // Set the Hidden CategoryID
                txtParentCategoryID.Text = (objHelpDesk_Category.ParentCategoryID == null) ? "" : ddlParentCategory.SelectedValue;
                txtCategoryID.Text = objHelpDesk_Category.CategoryID.ToString();
                ResetForm();
            }

            RefreshCache();
            DisplayHelpDesk();

            // Set the Parent drop-down
            if (txtParentCategoryID.Text != "")
            {
                ddlParentCategory.SelectedValue = txtParentCategoryID.Text;
            }
        }
        #endregion

        #region UpdateLevelOfChildren
        private void UpdateLevelOfChildren(HelpDesk_Category result)
        {
            int? intStartingLevel = result.Level;

            if (colProcessedCategoryIDs == null)
            {
                colProcessedCategoryIDs = new List<int>();
            }

            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            // Get the children of the current item
            // This method may be called from the top level or recuresively by one of the child items
            var CategoryChildren = from HelpDesk_Categories in objHelpDeskDALDataContext.HelpDesk_Categories
                                   where HelpDesk_Categories.ParentCategoryID == result.CategoryID
                                   where !colProcessedCategoryIDs.Contains(result.CategoryID)
                                   select HelpDesk_Categories;

            // Loop thru each item
            foreach (var objCategory in CategoryChildren)
            {
                colProcessedCategoryIDs.Add(objCategory.CategoryID);

                objCategory.Level = ((intStartingLevel) ?? 0) + 1;
                objHelpDeskDALDataContext.SubmitChanges();

                //Recursively call the UpdateLevelOfChildren method adding all children
                UpdateLevelOfChildren(objCategory);
            }
        }
        #endregion

        #region GetLevelOfParent
        private int? GetLevelOfParent(int? ParentCategoryID)
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            var result = (from HelpDesk_Categories in objHelpDeskDALDataContext.HelpDesk_Categories
                          where HelpDesk_Categories.CategoryID == ParentCategoryID
                          select HelpDesk_Categories).FirstOrDefault();

            return (result == null) ? 0 : result.Level;
        }
        #endregion

        #region GetParentCategoryID
        private string GetParentCategoryID(string strParentCategoryID)
        {
            // This is to ensure that the ParentCategoryID does exist and has not been deleted since the last time the form was loaded
            int ParentCategoryID = 0;
            if (strParentCategoryID != "0")
            {
                ParentCategoryID = Convert.ToInt32(strParentCategoryID);
            }

            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            var result = (from HelpDesk_Categories in objHelpDeskDALDataContext.HelpDesk_Categories
                          where HelpDesk_Categories.CategoryID == ParentCategoryID
                          select HelpDesk_Categories).FirstOrDefault();

            string strResultParentCategoryID = "0";
            if (result != null)
            {
                strResultParentCategoryID = result.CategoryID.ToString();
            }

            return strResultParentCategoryID;
        }
        #endregion

        #region btnAddNew_Click
        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            if (btnAddNew.CommandName == "AddNew")
            {
                SetFormToAddNew();
            }
            else
            {
                // This is a Cancel
                ResetForm();
                DisplayHelpDesk();
                SelectTreeNode();
            }
        }
        #endregion

        #region SetFormToAddNew
        private void SetFormToAddNew()
        {
            txtCategory.Text = "";
            chkRequesterVisible.Checked = true;
            chkSelectable.Checked = true;
            btnAddNew.CommandName = "Cancel";
            btnUpdate.CommandName = "Save";
            btnAddNew.Text = Localization.GetString("Cancel.Text", LocalResourceFile);
            btnUpdate.Text = Localization.GetString("Save.Text", LocalResourceFile);
            btnDelete.Visible = false;
            BindDropDown(-1);

            if (tvCategories.SelectedNode == null)
            {
                ddlParentCategory.SelectedValue = "0";
            }
            else
            {
                try
                {
                    ddlParentCategory.SelectedValue = tvCategories.SelectedNode.Value;
                }
                catch (Exception ex)
                {
                    lblTagError.Text = ex.Message;
                }
            }
        }
        #endregion

        #region ResetForm
        private void ResetForm()
        {
            btnUpdate.CommandName = "Update";
            btnAddNew.CommandName = "AddNew";
            btnAddNew.Text = Localization.GetString("btnAddNew.Text", LocalResourceFile);
            btnUpdate.Text = Localization.GetString("btnUpdateAdminRole.Text", LocalResourceFile);
            btnDelete.Visible = true;
        }
        #endregion

        #region btnDelete_Click
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            // Get the node
            var result = (from HelpDesk_Categories in objHelpDeskDALDataContext.HelpDesk_Categories
                          where HelpDesk_Categories.CategoryID == Convert.ToInt32(txtCategoryID.Text)
                          select HelpDesk_Categories).FirstOrDefault();

            // Make a Temp object to use to update the child nodes
            HelpDesk_Category TmpHelpDesk_Category = new HelpDesk_Category();
            TmpHelpDesk_Category.CategoryID = result.CategoryID;
            if (result.ParentCategoryID == null)
            {
                TmpHelpDesk_Category.Level = 0;
            }
            else
            {
                TmpHelpDesk_Category.Level = GetLevelOfParent(result.ParentCategoryID);
            }

            // Get all TaskCategories that use the Node
            var colTaskCategories = from HelpDesk_TaskCategories in objHelpDeskDALDataContext.HelpDesk_TaskCategories
                                    where HelpDesk_TaskCategories.CategoryID == Convert.ToInt32(txtCategoryID.Text)
                                    select HelpDesk_TaskCategories;

            // Delete them
            objHelpDeskDALDataContext.HelpDesk_TaskCategories.DeleteAllOnSubmit(colTaskCategories);
            objHelpDeskDALDataContext.SubmitChanges();

            // Delete the node
            objHelpDeskDALDataContext.HelpDesk_Categories.DeleteOnSubmit(result);
            objHelpDeskDALDataContext.SubmitChanges();

            // Update levels of all the Children            
            UpdateLevelOfChildren(TmpHelpDesk_Category);

            // Update all the children nodes to give them a new parent
            var CategoryChildren = from HelpDesk_Categories in objHelpDeskDALDataContext.HelpDesk_Categories
                                   where HelpDesk_Categories.ParentCategoryID == result.CategoryID
                                   select HelpDesk_Categories;

            // Loop thru each item
            foreach (var objCategory in CategoryChildren)
            {
                objCategory.ParentCategoryID = result.ParentCategoryID;
                objHelpDeskDALDataContext.SubmitChanges();
            }

            // Delete the Catagory from any Ticket that uses it
            var DeleteHelpDesk_TaskCategories = from HelpDesk_TaskCategories in objHelpDeskDALDataContext.HelpDesk_TaskCategories
                                                where HelpDesk_TaskCategories.CategoryID == TmpHelpDesk_Category.CategoryID
                                                select HelpDesk_TaskCategories;

            objHelpDeskDALDataContext.HelpDesk_TaskCategories.DeleteAllOnSubmit(DeleteHelpDesk_TaskCategories);
            objHelpDeskDALDataContext.SubmitChanges();

            RefreshCache();

            // Set the CategoryID
            txtCategoryID.Text = (result.ParentCategoryID == null) ? "" : result.ParentCategoryID.ToString();

            DisplayHelpDesk();
            SelectTreeNode();
        }
        #endregion

        #region RefreshCache
        private void RefreshCache()
        {
            // Get Table out of Cache
            object objCategoriesTable = HttpContext.Current.Cache.Get(String.Format("CategoriesTable_{0}", PortalId.ToString()));

            // Is the table in the cache?
            if (objCategoriesTable != null)
            {
                // Remove table from cache
                HttpContext.Current.Cache.Remove(String.Format("CategoriesTable_{0}", PortalId.ToString()));
            }

            // Get Table out of Cache
            object objRequestorCategoriesTable_ = HttpContext.Current.Cache.Get(String.Format("RequestorCategoriesTable_{0}", PortalId.ToString()));

            // Is the table in the cache?
            if (objRequestorCategoriesTable_ != null)
            {
                // Remove table from cache
                HttpContext.Current.Cache.Remove(String.Format("RequestorCategoriesTable_{0}", PortalId.ToString()));
            }
        }
        #endregion

        #region tvCategories_TreeNodeDataBound
        protected void tvCategories_TreeNodeDataBound(object sender, TreeNodeEventArgs e)
        {
            ListItem objListItem = (ListItem)e.Node.DataItem;
            e.Node.ShowCheckBox = Convert.ToBoolean(objListItem.Attributes["Selectable"]);
            if (Convert.ToBoolean(objListItem.Attributes["Selectable"]))
            {
                e.Node.ImageUrl = Convert.ToBoolean(objListItem.Attributes["RequestorVisible"]) ? "images/world.png" : "images/world_delete.png";
                e.Node.ToolTip = Convert.ToBoolean(objListItem.Attributes["RequestorVisible"]) ? "Requestor Visible" : "Requestor Not Visible";
            }
            else
            {
                e.Node.ImageUrl = "images/table.png";
                e.Node.ToolTip = Convert.ToBoolean(objListItem.Attributes["RequestorVisible"]) ? "Requestor Visible" : "Requestor Not Visible";
            }
        }
        #endregion

        // Roles

        #region ldsRoles_Selecting
        protected void ldsRoles_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.WhereParameters["PortalID"] = PortalId;
        }
        #endregion

        #region lvRoles_ItemDataBound
        protected void lvRoles_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            ListViewDataItem objListViewDataItem = (ListViewDataItem)e.Item;
            Label RoleIDLabel = (Label)e.Item.FindControl("RoleIDLabel");

            try
            {
                RoleController objRoleController = new RoleController();
                RoleIDLabel.Text = String.Format("{0}", objRoleController.GetRole(Convert.ToInt32(RoleIDLabel.Text), PortalId).RoleName);
            }
            catch (Exception)
            {
                RoleIDLabel.Text = Localization.GetString("DeletedRole.Text", LocalResourceFile);
            }
        }
        #endregion

        #region btnInsertRole_Click
        protected void btnInsertRole_Click(object sender, EventArgs e)
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            // See if Role already exists
            HelpDesk_Role colHelpDesk_Roles = (from HelpDesk_Roles in objHelpDeskDALDataContext.HelpDesk_Roles
                                                       where HelpDesk_Roles.PortalID == PortalId
                                                       where HelpDesk_Roles.RoleID == Convert.ToInt32(ddlRole.SelectedValue)
                                                       select HelpDesk_Roles).FirstOrDefault();
            if (colHelpDesk_Roles != null)
            {
                RoleController objRoleController = new RoleController();
                lblRoleError.Text = String.Format(Localization.GetString("RoleAlreadyAdded.Text", LocalResourceFile), objRoleController.GetRole(Convert.ToInt32(ddlRole.SelectedValue), PortalId).RoleName);
            }
            else
            {
                HelpDesk_Role objHelpDesk_Role = new HelpDesk_Role();
                objHelpDesk_Role.PortalID = PortalId;
                objHelpDesk_Role.RoleID = Convert.ToInt32(ddlRole.SelectedValue);

                objHelpDeskDALDataContext.HelpDesk_Roles.InsertOnSubmit(objHelpDesk_Role);
                objHelpDeskDALDataContext.SubmitChanges();

                lvRoles.DataBind();
            }
        }
        #endregion

        #region DisplayRoles
        private void DisplayRoles()
        {
            // Get all the Roles
            RoleController RoleController = new RoleController();
            ArrayList colArrayList = RoleController.GetRoles();

            // Create a ListItemCollection to hold the Roles 
            ListItemCollection colListItemCollection = new ListItemCollection();

            // Add the Roles to the List
            foreach (RoleInfo Role in colArrayList)
            {
                if (Role.PortalID == PortalId)
                {
                    ListItem RoleListItem = new ListItem();
                    RoleListItem.Text = Role.RoleName;
                    RoleListItem.Value = Role.RoleID.ToString();
                    colListItemCollection.Add(RoleListItem);
                }
            }

            // Add the Roles to the ListBox
            ddlRole.DataSource = colListItemCollection;
            ddlRole.DataBind();
        }
        #endregion
    }
}
//
// webserver.com
// Copyright (c) 2009
// by Michael Washington
//
// redhound.ru
// Copyright (c) 2013
// by Roman M. Yagodin
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//
//

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
using Microsoft.VisualBasic;
using System.Text;
using System.IO;
using DotNetNuke.Services.Localization;

namespace R7.HelpDesk
{
    public partial class EditTask : DotNetNuke.Entities.Modules.PortalModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            cmdtxtDueDateCalendar.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtDueDate);
            cmdtxtStartCalendar.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtStart);
            cmdtxtCompleteCalendar.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtComplete);
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["TaskID"] != null)
                {
                    CommentsControl.ViewOnly = true;
                    if (CheckSecurity())
                    {
                        LoadRolesDropDown();
                        DisplayCategoryTree();
                        DisplayTicketData();
                        CommentsControl.TaskID = Convert.ToInt32(lblTask.Text);
                        CommentsControl.ModuleID = ModuleId;
                        WorkControl.TaskID = Convert.ToInt32(lblTask.Text);
                        WorkControl.ModuleID = ModuleId;
                        LogsControl.TaskID = Convert.ToInt32(lblTask.Text);

                        // If at this point CommentsControl is in View Only mode
                        // Set main form in View Only mode
                        if (CommentsControl.ViewOnly == true)
                        {
                            SetViewOnlyMode();
                        }

                        // Insert Log
						Log.InsertLog(Convert.ToInt32(lblTask.Text), UserId, String.Format(Localization.GetString("ViewedTicket.Text", LocalResourceFile), (UserId == -1) ? Localization.GetString("Requester.Text", LocalResourceFile) : UserInfo.DisplayName));
                    }
                    else
                    {
                        pnlEditTask.Visible = false;
                        Response.Redirect(DotNetNuke.Common.Globals.NavigateURL());
                    }
                }
                else
                {
                    pnlEditTask.Visible = false;
                    Response.Redirect(DotNetNuke.Common.Globals.NavigateURL());
                }
            }
        }

        #region SetViewOnlyMode
        private void SetViewOnlyMode()
        {
            btnSave.Visible = false;
            btnComments.Visible = false;
            btnWorkItems.Visible = false;
            btnLogs.Visible = false;
            ddlAssigned.Enabled = false;
            ddlStatus.Enabled = false;
            ddlPriority.Enabled = false;
        }
        #endregion

        #region LoadRolesDropDown
        private void LoadRolesDropDown()
        {
            RoleController objRoleController = new RoleController();
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            List<HelpDesk_Role> colHelpDesk_Roles = (from HelpDesk_Roles in objHelpDeskDALDataContext.HelpDesk_Roles
                                                             where HelpDesk_Roles.PortalID == PortalId
                                                             select HelpDesk_Roles).ToList();

            // Create a ListItemCollection to hold the Roles 
            ListItemCollection colListItemCollection = new ListItemCollection();

            // Add the Roles to the List
            foreach (HelpDesk_Role objHelpDesk_Role in colHelpDesk_Roles)
            {
                try
                {
                    RoleInfo objRoleInfo = objRoleController.GetRole(Convert.ToInt32(objHelpDesk_Role.RoleID), PortalId);

                    ListItem RoleListItem = new ListItem();
                    RoleListItem.Text = objRoleInfo.RoleName;
                    RoleListItem.Value = objHelpDesk_Role.RoleID.ToString();
                    ddlAssigned.Items.Add(RoleListItem);
                }
                catch
                {
                    // Role no longer exists in Portal
                    ListItem RoleListItem = new ListItem();
                    RoleListItem.Text = Localization.GetString("DeletedRole.Text", LocalResourceFile);
                    RoleListItem.Value = objHelpDesk_Role.RoleID.ToString();
                    ddlAssigned.Items.Add(RoleListItem);
                }
            }

            // Add UnAssigned
            ListItem UnassignedRoleListItem = new ListItem();
            UnassignedRoleListItem.Text = Localization.GetString("Unassigned.Text", LocalResourceFile);
            UnassignedRoleListItem.Value = "-1";
            ddlAssigned.Items.Add(UnassignedRoleListItem);

        }
        #endregion

        #region CheckSecurity
        private bool CheckSecurity()
        {
            bool boolPassedSecurity = false;
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            HelpDesk_Task objHelpDesk_Tasks = (from HelpDesk_Tasks in objHelpDeskDALDataContext.HelpDesk_Tasks
                                                       where HelpDesk_Tasks.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                                                       select HelpDesk_Tasks).FirstOrDefault();
            if (objHelpDesk_Tasks == null)
            {
                pnlEditTask.Visible = false;
                Response.Redirect(DotNetNuke.Common.Globals.NavigateURL());
            }

            // User not logged in
            if (UserId == -1)
            {
                // Must have the valid password
                if (Request.QueryString["TP"] != null)
                {
                    // Check the password for this Ticket
                    if (objHelpDesk_Tasks.TicketPassword == Convert.ToString(Request.QueryString["TP"]))
                    {
                        boolPassedSecurity = true;
                    }
                    else
                    {
                        boolPassedSecurity = false;
                    }
                }
            }

            // User is logged in
            if (UserId > -1)
            {
                // Is user an Admin?
                string strAdminRoleID = GetAdminRole();
                if (UserInfo.IsInRole(strAdminRoleID) || UserInfo.IsInRole("Administrators") || UserInfo.IsSuperUser)
                {
                    boolPassedSecurity = true;
                    CommentsControl.ViewOnly = false;
                }

                // Is user the Requestor?
                if (UserId == objHelpDesk_Tasks.RequesterUserID)
                {
                    boolPassedSecurity = true;
                }

                //Is user in the Assigned Role?
                RoleController objRoleController = new RoleController();
                RoleInfo objRoleInfo = objRoleController.GetRole(objHelpDesk_Tasks.AssignedRoleID, PortalId);
                if (objRoleInfo != null)
                {
                    if (UserInfo.IsInRole(objRoleInfo.RoleName))
                    {
                        boolPassedSecurity = true;
                        CommentsControl.ViewOnly = false;
                    }
                }

                // Does user have a valid temporary password?
                if (Request.QueryString["TP"] != null)
                {
                    // Check the password for this Ticket
                    if (objHelpDesk_Tasks.TicketPassword == Convert.ToString(Request.QueryString["TP"]))
                    {
                        boolPassedSecurity = true;
                    }
                    else
                    {
                        boolPassedSecurity = false;
                    }
                }
            }

            return boolPassedSecurity;
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

            return colHelpDesk_Setting;
        }
        #endregion

        // Tags

        #region DisplayCategoryTree
        private void DisplayCategoryTree()
        {
            bool boolUserAssignedToTask = false;
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            HelpDesk_Task objHelpDesk_Tasks = (from HelpDesk_Tasks in objHelpDeskDALDataContext.HelpDesk_Tasks
                                                       where HelpDesk_Tasks.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                                                       select HelpDesk_Tasks).FirstOrDefault();

            //Is user in the Assigned Role?
            RoleController objRoleController = new RoleController();
            RoleInfo objRoleInfo = objRoleController.GetRole(objHelpDesk_Tasks.AssignedRoleID, PortalId);

            if (objRoleInfo != null)
            {
                if (UserInfo.IsInRole(objRoleInfo.RoleName))
                {
                    boolUserAssignedToTask = true;
                }
            }

            if (boolUserAssignedToTask || UserInfo.IsInRole(GetAdminRole()) || UserInfo.IsInRole("Administrators") || UserInfo.IsSuperUser)
            {
                // Show all Tags
                TagsTreeExistingTasks.Visible = true;
                TagsTreeExistingTasks.TagID = Convert.ToInt32(Request.QueryString["TaskID"]);
                TagsTreeExistingTasks.DisplayType = "Administrator";
                TagsTreeExistingTasks.Expand = false;
            }
            else
            {
                // Show only Visible Tags
                TagsTreeExistingTasks.Visible = true;
                TagsTreeExistingTasks.TagID = Convert.ToInt32(Request.QueryString["TaskID"]);
                TagsTreeExistingTasks.DisplayType = "Requestor";
                TagsTreeExistingTasks.Expand = false;
            }

            // Select Existing values
            if (objHelpDesk_Tasks.HelpDesk_TaskCategories.Select(x => x.CategoryID).ToArray<int>().Count() > 0)
            {
                int[] ArrStrCategories = objHelpDesk_Tasks.HelpDesk_TaskCategories.Select(x => x.CategoryID).ToArray<int>();
                int?[] ArrIntHelpDesk = Array.ConvertAll<int, int?>(ArrStrCategories, new Converter<int, int?>(ConvertToNullableInt));

                TagsTreeExistingTasks.SelectedCategories = ArrIntHelpDesk;
            }

            // Set visibility of Tags
            bool RequestorR7_HelpDesk = (TagsTreeExistingTasks.DisplayType == "Administrator") ? false : true;

            int CountOfHelpDesk = (from WebserverCategories in CategoriesTable.GetCategoriesTable(PortalId, RequestorR7_HelpDesk)
                                     where WebserverCategories.PortalID == PortalId
                                     where WebserverCategories.Level == 1
                                     select WebserverCategories).Count();

            imgTags.Visible = (CountOfHelpDesk > 0);
            lbltxtTags.Visible = (CountOfHelpDesk > 0);
        }
        #endregion

        #region ConvertToNullableInt
        private int? ConvertToNullableInt(int strParameter)
        {
            return Convert.ToInt32(strParameter);
        }
        #endregion

        // Display Ticket Data

        #region DisplayTicketData
        private void DisplayTicketData()
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            HelpDesk_Task objHelpDesk_Tasks = (from HelpDesk_Tasks in objHelpDeskDALDataContext.HelpDesk_Tasks
                                                       where HelpDesk_Tasks.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                                                       select HelpDesk_Tasks).FirstOrDefault();

            // Name is editable only if user is Anonymous
            if (objHelpDesk_Tasks.RequesterUserID == -1)
            {
                txtEmail.Visible = true;
                txtName.Visible = true;
                lblEmail.Visible = false;
                lblName.Visible = false;
                txtEmail.Text = objHelpDesk_Tasks.RequesterEmail;
                txtName.Text = objHelpDesk_Tasks.RequesterName;
            }
            else
            {
                txtEmail.Visible = false;
                txtName.Visible = false;
                lblEmail.Visible = true;
                lblName.Visible = true;


				UserInfo objRequester = //UserController.GetUser(PortalId, objHelpDesk_Tasks.RequesterUserID, false);
					UserController.GetUserById (PortalId, objHelpDesk_Tasks.RequesterUserID);

                if (objRequester != null)
                {
					lblEmail.Text = objRequester.Email; // UserController.GetUser(PortalId, objHelpDesk_Tasks.RequesterUserID, false).Email;
					lblName.Text = objRequester.DisplayName; // UserController.GetUser(PortalId, objHelpDesk_Tasks.RequesterUserID, false).DisplayName;
                }
                else
                {
                    lblName.Text = "[User Deleted]";
                }
            }

            lblTask.Text = objHelpDesk_Tasks.TaskID.ToString();
            lblCreated.Text = String.Format(Localization.GetString("Created.Text", LocalResourceFile), objHelpDesk_Tasks.CreatedDate.ToShortDateString(), objHelpDesk_Tasks.CreatedDate.ToShortTimeString());
            ddlStatus.SelectedValue = objHelpDesk_Tasks.Status;
            ddlPriority.SelectedValue = objHelpDesk_Tasks.Priority;
            txtDescription.Text = objHelpDesk_Tasks.Description;
            txtPhone.Text = objHelpDesk_Tasks.RequesterPhone;
            txtDueDate.Text = (objHelpDesk_Tasks.DueDate.HasValue) ? objHelpDesk_Tasks.DueDate.Value.ToShortDateString() : "";
            txtStart.Text = (objHelpDesk_Tasks.EstimatedStart.HasValue) ? objHelpDesk_Tasks.EstimatedStart.Value.ToShortDateString() : "";
            txtComplete.Text = (objHelpDesk_Tasks.EstimatedCompletion.HasValue) ? objHelpDesk_Tasks.EstimatedCompletion.Value.ToShortDateString() : "";
            txtEstimate.Text = (objHelpDesk_Tasks.EstimatedHours.HasValue) ? objHelpDesk_Tasks.EstimatedHours.Value.ToString() : "";

            ListItem TmpRoleListItem = ddlAssigned.Items.FindByValue(objHelpDesk_Tasks.AssignedRoleID.ToString());
            if (TmpRoleListItem == null)
            {
                // Value was not found so add it
                RoleController objRoleController = new RoleController();
                RoleInfo objRoleInfo = objRoleController.GetRole(objHelpDesk_Tasks.AssignedRoleID, PortalId);

                if (objRoleInfo != null)
                {
                    ListItem RoleListItem = new ListItem();
                    RoleListItem.Text = objRoleInfo.RoleName;
                    RoleListItem.Value = objHelpDesk_Tasks.AssignedRoleID.ToString();
                    ddlAssigned.Items.Add(RoleListItem);

                    ddlAssigned.SelectedValue = objHelpDesk_Tasks.AssignedRoleID.ToString();
                }
                else
                {
                    // Role no longer exists in Portal
                    ddlAssigned.SelectedValue = "-1";
                }
            }
            else
            {
                // The Value already exists so set it
                ddlAssigned.SelectedValue = objHelpDesk_Tasks.AssignedRoleID.ToString();
            }
        }
        #endregion

        // Save Form Data

        #region btnSave_Click
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateTicketForm())
                {
                    int intTaskID = SaveTicketForm();
                    SaveTags(intTaskID);
                    ShowUpdated();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }
        #endregion

        #region ValidateTicketForm
        private bool ValidateTicketForm()
        {
            List<string> ColErrors = new List<string>();

            // Only validate Name and email if Ticket is not for a DNN user
            // lblName will be hidden if it is not a DNN user
            if (lblName.Visible == false)
            {
                if (txtName.Text.Trim().Length < 1)
                {
                    ColErrors.Add(Localization.GetString("NameIsRequired.Text", LocalResourceFile));
                }

                if (txtEmail.Text.Trim().Length < 1)
                {
                    ColErrors.Add(Localization.GetString("EmailIsRequired.Text", LocalResourceFile));
                }
            }

            // Validate the date only if a date was entered
            if (txtDueDate.Text.Trim().Length > 1)
            {
                try
                {
                    DateTime tmpDate = Convert.ToDateTime(txtDueDate.Text.Trim());
                }
                catch
                {
                    ColErrors.Add(Localization.GetString("MustUseAValidDate.Text", LocalResourceFile));
                }
            }

            if (txtStart.Text.Trim().Length > 1)
            {
                try
                {
                    DateTime tmpDate = Convert.ToDateTime(txtStart.Text.Trim());
                }
                catch
                {
                    ColErrors.Add(Localization.GetString("MustUseAValidDate.Text", LocalResourceFile));
                }
            }

            if (txtComplete.Text.Trim().Length > 1)
            {
                try
                {
                    DateTime tmpDate = Convert.ToDateTime(txtComplete.Text.Trim());
                }
                catch
                {
                    ColErrors.Add(Localization.GetString("MustUseAValidDate.Text", LocalResourceFile));
                }
            }

            if (txtEstimate.Text.Trim().Length > 0)
            {
                try
                {
                    int tmpInt = Convert.ToInt32(txtEstimate.Text.Trim());
                }
                catch
                {
                    ColErrors.Add(Localization.GetString("MustUseAValidEstimateHours.Text", LocalResourceFile));
                }
            }

            // Display Validation Errors
            if (ColErrors.Count > 0)
            {
                foreach (string objError in ColErrors)
                {
                    lblError.Text = lblError.Text + String.Format("* {0}<br />", objError);
                }
            }

            return (ColErrors.Count == 0);
        }
        #endregion

        #region SaveTicketForm
        private int SaveTicketForm()
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            HelpDesk_Task objHelpDesk_Task = (from HelpDesk_Tasks in objHelpDeskDALDataContext.HelpDesk_Tasks
                                                      where HelpDesk_Tasks.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                                                      select HelpDesk_Tasks).FirstOrDefault();

            // Save original Assigned Group
            int intOriginalAssignedGroup = objHelpDesk_Task.AssignedRoleID; 

            // Save Task
            objHelpDesk_Task.Status = ddlStatus.SelectedValue;
            objHelpDesk_Task.Description = txtDescription.Text;
            objHelpDesk_Task.PortalID = PortalId;
            objHelpDesk_Task.Priority = ddlPriority.SelectedValue;
            objHelpDesk_Task.RequesterPhone = txtPhone.Text;
            objHelpDesk_Task.AssignedRoleID = Convert.ToInt32(ddlAssigned.SelectedValue);

            // Only validate Name and email if Ticket is not for a DNN user
            // lblName will be hidden if it is not a DNN user
            if (lblName.Visible == false)
            {
                // not a DNN user
                objHelpDesk_Task.RequesterEmail = txtEmail.Text;
                objHelpDesk_Task.RequesterName = txtName.Text;
                objHelpDesk_Task.RequesterUserID = -1;
            }

            // DueDate
            if (txtDueDate.Text.Trim().Length > 1)
            {
                objHelpDesk_Task.DueDate = Convert.ToDateTime(txtDueDate.Text.Trim());
            }
            else
            {
                objHelpDesk_Task.DueDate = null;
            }

            // EstimatedStart
            if (txtStart.Text.Trim().Length > 1)
            {
                objHelpDesk_Task.EstimatedStart = Convert.ToDateTime(txtStart.Text.Trim());
            }
            else
            {
                objHelpDesk_Task.EstimatedStart = null;
            }

            // EstimatedCompletion
            if (txtComplete.Text.Trim().Length > 1)
            {
                objHelpDesk_Task.EstimatedCompletion = Convert.ToDateTime(txtComplete.Text.Trim());
            }
            else
            {
                objHelpDesk_Task.EstimatedCompletion = null;
            }

            // EstimatedHours
            if (txtEstimate.Text.Trim().Length > 0)
            {
                objHelpDesk_Task.EstimatedHours = Convert.ToInt32(txtEstimate.Text.Trim());
            }
            else
            {
                objHelpDesk_Task.EstimatedHours = null;
            }

            objHelpDeskDALDataContext.SubmitChanges();

            // Notify Assigned Group
            if (Convert.ToInt32(ddlAssigned.SelectedValue) > -1)
            {
                // Only notify if Assigned group has changed
                if (intOriginalAssignedGroup != Convert.ToInt32(ddlAssigned.SelectedValue))
                {
                    NotifyAssignedGroupOfAssignment();                    
                }
            }

            // Insert Log
            Log.InsertLog(objHelpDesk_Task.TaskID, UserId, String.Format(Localization.GetString("UpdatedTicket.Text", LocalResourceFile), UserInfo.DisplayName));

            return objHelpDesk_Task.TaskID;
        }
        #endregion

        #region SaveTags
        private void SaveTags(int intTaskID)
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            var ExistingTaskCategories = from HelpDesk_TaskCategories in objHelpDeskDALDataContext.HelpDesk_TaskCategories
                                         where HelpDesk_TaskCategories.TaskID == intTaskID
                                         select HelpDesk_TaskCategories;

            // Delete all existing TaskCategories
            if (ExistingTaskCategories != null)
            {
                objHelpDeskDALDataContext.HelpDesk_TaskCategories.DeleteAllOnSubmit(ExistingTaskCategories);
                objHelpDeskDALDataContext.SubmitChanges();
            }

            // Add TaskCategories
            TreeView objTreeView = (TreeView)TagsTreeExistingTasks.FindControl("tvCategories");
            if (objTreeView.CheckedNodes.Count > 0)
            {
                // Iterate through the CheckedNodes collection 
                foreach (TreeNode node in objTreeView.CheckedNodes)
                {
                    HelpDesk_TaskCategory objHelpDesk_TaskCategory = new HelpDesk_TaskCategory();

                    objHelpDesk_TaskCategory.TaskID = intTaskID;
                    objHelpDesk_TaskCategory.CategoryID = Convert.ToInt32(node.Value);

                    objHelpDeskDALDataContext.HelpDesk_TaskCategories.InsertOnSubmit(objHelpDesk_TaskCategory);
                    objHelpDeskDALDataContext.SubmitChanges();
                }
            }
        }
        #endregion

        #region ShowUpdated
        private void ShowUpdated()
        {
            lblError.Text = Localization.GetString("Updated.Text", LocalResourceFile);

            // Provide a way for the user to see that a record has been updated
            // multiple times by changing the color each time
            lblError.ForeColor = (lblError.ForeColor == Color.Red) ? Color.Blue : Color.Red;

        }
        #endregion

        // Details

        #region DisableAllButtons
        private void DisableAllButtons()
        {
            btnComments.BorderStyle = BorderStyle.Outset;
            btnComments.BackColor = Color.WhiteSmoke;
            btnComments.Font.Bold = false;
            btnComments.ForeColor = Color.Black;
            pnlComments.Visible = false;

            btnWorkItems.BorderStyle = BorderStyle.Outset;
            btnWorkItems.BackColor = Color.WhiteSmoke;
            btnWorkItems.Font.Bold = false;
            btnWorkItems.ForeColor = Color.Black;
            pnlWorkItems.Visible = false;

            btnLogs.BorderStyle = BorderStyle.Outset;
            btnLogs.BackColor = Color.WhiteSmoke;
            btnLogs.Font.Bold = false;
            btnLogs.ForeColor = Color.Black;
            pnlLogs.Visible = false;
        }
        #endregion

        // Comments

        #region btnComments_Click
        protected void btnComments_Click(object sender, EventArgs e)
        {
            // If we are already on the Comments screen then switch Comments to Default mode
            if (pnlComments.Visible == true)
            {
                CommentsControl.SetView("Default");
            }

            DisableAllButtons();
            btnComments.BorderStyle = BorderStyle.Inset;
            btnComments.BackColor = Color.LightGray;
            btnComments.Font.Bold = true;
            btnComments.ForeColor = Color.Red;
            pnlComments.Visible = true;
        }
        #endregion

        // Work Items

        #region btnWorkItems_Click
        protected void btnWorkItems_Click(object sender, EventArgs e)
        {
            DisableAllButtons();
            btnWorkItems.BorderStyle = BorderStyle.Inset;
            btnWorkItems.BackColor = Color.LightGray;
            btnWorkItems.Font.Bold = true;
            btnWorkItems.ForeColor = Color.Red;
            pnlWorkItems.Visible = true;
        }
        #endregion

        // Emails

        #region NotifyAssignedGroupOfAssignment
        private void NotifyAssignedGroupOfAssignment()
        {
            RoleController objRoleController = new RoleController();
            string strAssignedRole = String.Format("{0}", objRoleController.GetRole(Convert.ToInt32(ddlAssigned.SelectedValue), PortalId).RoleName);
            string strLinkUrl = Utils.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}", Request.QueryString["TaskID"])), PortalSettings.PortalAlias.HTTPAlias);

            string strSubject = "[" + Localization.GetString(String.Format("ddlStatusAdmin{0}.Text", ddlStatus.SelectedValue), LocalResourceFile) + "] " + String.Format(Localization.GetString("HelpDeskTicketAtHasBeenAssigned.Text", LocalResourceFile), Request.QueryString["TaskID"], PortalSettings.PortalAlias.HTTPAlias, strAssignedRole);
            string strBody = String.Format(Localization.GetString("ANewHelpDeskTicketHasBeenAssigned.Text", LocalResourceFile), Request.QueryString["TaskID"], txtDescription.Text);
            strBody = strBody + Environment.NewLine;
            strBody = strBody + String.Format(Localization.GetString("YouMaySeeStatusHere.Text", LocalResourceFile), strLinkUrl);

            // Get all users in the AssignedRole Role
            ArrayList colAssignedRoleUsers = objRoleController.GetUsersByRoleName(PortalId, strAssignedRole);

            foreach (UserInfo objUserInfo in colAssignedRoleUsers)
            {
                DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, objUserInfo.Email, "", strSubject, strBody, "", "HTML", "", "", "", "");
            }

            Log.InsertLog(Convert.ToInt32(Request.QueryString["TaskID"]), UserId, String.Format(Localization.GetString("AssignedTicketTo.Text", LocalResourceFile), UserInfo.DisplayName, strAssignedRole));
        }
        #endregion

        // Logs

        #region btnLogs_Click
        protected void btnLogs_Click(object sender, EventArgs e)
        {
            DisableAllButtons();
            btnLogs.BorderStyle = BorderStyle.Inset;
            btnLogs.BackColor = Color.LightGray;
            btnLogs.Font.Bold = true;
            btnLogs.ForeColor = Color.Red;
            pnlLogs.Visible = true;

            LogsControl.RefreshLogs();
        }
        #endregion

    }
}
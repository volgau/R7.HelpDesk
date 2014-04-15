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
using System.Text;
using System.IO;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace R7.HelpDesk
{
    public partial class Comments : DotNetNuke.Entities.Modules.PortalModuleBase
    {
        #region Properties
        public int TaskID
        {
            get { return Convert.ToInt32(ViewState["TaskID"]); }
            set { ViewState["TaskID"] = value; }
        }

        public int ModuleID
        {
            get { return Convert.ToInt32(ViewState["ModuleID"]); }
            set { ViewState["ModuleID"] = value; }
        }

        public bool ViewOnly
        {
            get { return Convert.ToBoolean(ViewState["ViewOnly"]); }
            set { ViewState["ViewOnly"] = value; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            pnlInsertComment.GroupingText = Localization.GetString("pnlInsertComment.Text", LocalResourceFile);

            if (!Page.IsPostBack)
            {
                SetView("Default");

                if (ViewOnly)
                {
                    SetViewOnlyMode();
                }

                ShowFileUpload();
            }
        }

        #region ShowFileUpload
        private void ShowFileUpload()
        {
            string strAdminRoleID = GetAdminRole();
            if (!(UserInfo.IsInRole(strAdminRoleID) || UserInfo.IsInRole("Administrators") || UserInfo.IsSuperUser))
            {
                string strUploadPermission = GetUploadPermission();

                // Only supress Upload if permission is not set to All              
                if (strUploadPermission != "All")
                {
                    // Is user Logged in?
                    if (UserId > -1)
                    {
                        #region if (strUploadPermission != "Administrator/Registered Users")
                        // Only check this if security is set to "Administrator/Registered Users"
                        if (strUploadPermission != "Administrator/Registered Users")
                        {
                            // If User is not an Administrator so they cannot see upload
                            lblAttachFile1.Visible = false;
                            TicketFileUpload.Visible = false;
                            lblAttachFile2.Visible = false;
                            fuAttachment.Visible = false;
                        }
                        #endregion
                    }
                    else
                    {
                        // If User is not logged in they cannot see upload
                        lblAttachFile1.Visible = false;
                        TicketFileUpload.Visible = false;
                        lblAttachFile2.Visible = false;
                        fuAttachment.Visible = false;
                    }
                }
            }
        }
        #endregion

        #region SetView
        public void SetView(string ViewMode)
        {
            if (ViewMode == "Default")
            {
                pnlInsertComment.Visible = true;
                pnlTableHeader.Visible = true;
                pnlExistingComments.Visible = true;
                pnlEditComment.Visible = false;
            }

            if (ViewMode == "Edit")
            {
                pnlInsertComment.Visible = false;
                pnlTableHeader.Visible = false;
                pnlExistingComments.Visible = false;
                pnlEditComment.Visible = true;
            }
        }
        #endregion

        #region SetViewOnlyMode
        private void SetViewOnlyMode()
        {
            chkCommentVisible.Visible = false;
            chkCommentVisibleEdit.Visible = false;
            lnkDelete.Visible = false;
            Image5.Visible = false;
            lnkUpdate.Visible = false;
            Image4.Visible = false;
            pnlDisplayFile.Visible = false;
            pnlAttachFile.Visible = false;
            imgDelete.Visible = false;
            lnkUpdateRequestor.Visible = false;
            ImgEmailUser.Visible = false;
            btnInsertCommentAndEmail.Visible = false;
        }
        #endregion

        // Insert Comment

        #region btnInsertComment_Click
        protected void btnInsertComment_Click(object sender, EventArgs e)
        {
            InsertComment();
        }
        #endregion

        #region btnInsertCommentAndEmail_Click
        protected void btnInsertCommentAndEmail_Click(object sender, EventArgs e)
        {
            string strComment = txtComment.Text;
            InsertComment();
            NotifyRequestorOfComment(strComment);
        }
        #endregion

        #region InsertComment
        private void InsertComment()
        {
            // Validate file upload
            if (TicketFileUpload.HasFile)
            {
               /*if (
                    string.Compare(Path.GetExtension(TicketFileUpload.FileName).ToLower(), ".gif", true) != 0
                    & string.Compare(Path.GetExtension(TicketFileUpload.FileName).ToLower(), ".jpg", true) != 0
                    & string.Compare(Path.GetExtension(TicketFileUpload.FileName).ToLower(), ".jpeg", true) != 0
                    & string.Compare(Path.GetExtension(TicketFileUpload.FileName).ToLower(), ".doc", true) != 0
                    & string.Compare(Path.GetExtension(TicketFileUpload.FileName).ToLower(), ".docx", true) != 0
                    & string.Compare(Path.GetExtension(TicketFileUpload.FileName).ToLower(), ".xls", true) != 0
                    & string.Compare(Path.GetExtension(TicketFileUpload.FileName).ToLower(), ".xlsx", true) != 0
                    & string.Compare(Path.GetExtension(TicketFileUpload.FileName).ToLower(), ".pdf", true) != 0
                    )*/
				if(!Utils.IsFileAllowed(TicketFileUpload.FileName))
                {
					lblError.Text = Localization.GetString ("FileExtensionIsNotAllowed.Text", LocalResourceFile);
                    // lblError.Text = "Only .gif, .jpg, .jpeg, .doc, .docx, .xls, .xlsx, .pdf files may be used.";
                    return;
                }
            }

            if (txtComment.Text.Trim().Length > 0)
            {
                HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

                string strComment = txtComment.Text.Trim();

                // Save Task Details
                HelpDesk_TaskDetail objHelpDesk_TaskDetail = new HelpDesk_TaskDetail();

                objHelpDesk_TaskDetail.TaskID = TaskID;
                objHelpDesk_TaskDetail.Description = txtComment.Text.Trim();
                objHelpDesk_TaskDetail.InsertDate = DateTime.Now;
                objHelpDesk_TaskDetail.UserID = UserId;

                if (chkCommentVisible.Checked)
                {
                    objHelpDesk_TaskDetail.DetailType = "Comment-Visible";
                }
                else
                {
                    objHelpDesk_TaskDetail.DetailType = "Comment";
                }

                objHelpDeskDALDataContext.HelpDesk_TaskDetails.InsertOnSubmit(objHelpDesk_TaskDetail);
                objHelpDeskDALDataContext.SubmitChanges();
                txtComment.Text = "";

                // Insert Log
				Log.InsertLog(TaskID, UserId, String.Format(Localization.GetString ("InsertedComment.Text", LocalResourceFile), GetUserName()));

                // Upload the File
                if (TicketFileUpload.HasFile)
                {
                    UploadFile(objHelpDesk_TaskDetail.DetailID);
                    // Insert Log
					Log.InsertLog(TaskID, UserId, String.Format(Localization.GetString ("UploadedFile.Text", LocalResourceFile), GetUserName(), TicketFileUpload.FileName));
                }

                if (UserIsRequestor())
                {
                    NotifyAssignedGroupOfComment(strComment);
                }

                gvComments.DataBind();
            }
        }
        #endregion

        #region LDSComments_Selecting
        protected void LDSComments_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();
            var result = from HelpDesk_TaskDetails in objHelpDeskDALDataContext.HelpDesk_TaskDetails
                         where HelpDesk_TaskDetails.TaskID == TaskID
                         where (HelpDesk_TaskDetails.DetailType == "Comment" || HelpDesk_TaskDetails.DetailType == "Comment-Visible")
                         select HelpDesk_TaskDetails;

            // If View only mode
            if (ViewOnly)
            {
                result = from TaskDetails in result
                         where TaskDetails.DetailType == "Comment-Visible"
                         select TaskDetails;
            }

            e.Result = result;
        }
        #endregion

        #region gvComments_RowDataBound
        protected void gvComments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridViewRow objGridViewRow = (GridViewRow)e.Row;

                // Comment
                Label lblComment = (Label)objGridViewRow.FindControl("lblComment");
                if (lblComment.Text.Trim().Length > 100)
                {
                    lblComment.Text = String.Format("{0}...", Utils.StringLeft(lblComment.Text, 100));
                }

                // User
                Label gvlblUser = (Label)objGridViewRow.FindControl("gvlblUser");
                if (gvlblUser.Text != "-1")
                {
                    UserInfo objUser = //UserController.GetUser(PortalId, Convert.ToInt32(gvlblUser.Text), false);
						UserController.GetUserById(PortalId, Convert.ToInt32(gvlblUser.Text));

                    if (objUser != null)
                    {
                        string strDisplayName = objUser.DisplayName;

                        if (strDisplayName.Length > 25)
                        {
                            gvlblUser.Text = String.Format("{0}...", Utils.StringLeft(strDisplayName, 25));
                        }
                        else
                        {
                            gvlblUser.Text = strDisplayName;
                        }
                    }
                    else
                    {
                        gvlblUser.Text = "[User Deleted]";
                    }
                }
                else
                {
                    gvlblUser.Text = Localization.GetString("Requestor.Text", LocalResourceFile);
                }

                // Comment Visible checkbox
                CheckBox chkDetailType = (CheckBox)objGridViewRow.FindControl("chkDetailType");
                Label lblDetailType = (Label)objGridViewRow.FindControl("lblDetailType");
                // lblDetailType
                chkDetailType.Checked = (lblDetailType.Text == "Comment") ? false : true;
            }
        }
        #endregion

        // File upload

        #region UploadFile
        private void UploadFile(int intDetailID)
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            string strUploFilesPath = (from HelpDesk_Settings in objHelpDeskDALDataContext.HelpDesk_Settings
                                           where HelpDesk_Settings.PortalID == PortalId
                                           where HelpDesk_Settings.SettingName == "UploFilesPath"
                                           select HelpDesk_Settings).FirstOrDefault().SettingValue;

            EnsureDirectory(new System.IO.DirectoryInfo(strUploFilesPath));
            string strfilename = Convert.ToString(intDetailID) + "_" + GetRandomPassword() + Path.GetExtension(TicketFileUpload.FileName).ToLower();
            strUploFilesPath = strUploFilesPath + @"\" + strfilename;
            TicketFileUpload.SaveAs(strUploFilesPath);

            HelpDesk_Attachment objHelpDesk_Attachment = new HelpDesk_Attachment();
            objHelpDesk_Attachment.DetailID = intDetailID;
            objHelpDesk_Attachment.FileName = strfilename;
            objHelpDesk_Attachment.OriginalFileName = TicketFileUpload.FileName;
            objHelpDesk_Attachment.AttachmentPath = strUploFilesPath;
            objHelpDesk_Attachment.UserID = UserId;

            objHelpDeskDALDataContext.HelpDesk_Attachments.InsertOnSubmit(objHelpDesk_Attachment);
            objHelpDeskDALDataContext.SubmitChanges();
        }
        #endregion

        #region EnsureDirectory
        public static void EnsureDirectory(System.IO.DirectoryInfo oDirInfo)
        {
            if (oDirInfo.Parent != null)
                EnsureDirectory(oDirInfo.Parent);
            if (!oDirInfo.Exists)
            {
                oDirInfo.Create();
            }
        }
        #endregion

        #region GetRandomPassword
        public string GetRandomPassword()
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            int intElements = random.Next(10, 26);

            for (int i = 0; i < intElements; i++)
            {
                int intRandomType = random.Next(0, 2);
                if (intRandomType == 1)
                {
                    char ch;
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                    builder.Append(ch);
                }
                else
                {
                    builder.Append(random.Next(0, 9));
                }
            }
            return builder.ToString();
        }
        #endregion

        #region GetUserName
        private string GetUserName()
        {
            string strUserName = Localization.GetString("Anonymous.Text", LocalResourceFile);

            if (UserId > -1)
            {
                strUserName = UserInfo.DisplayName;
            }

            return strUserName;
        }

        private string GetUserName(int intUserID)
        {
            string strUserName = Localization.GetString("Anonymous.Text", LocalResourceFile);

            if (intUserID > -1)
            {
                UserInfo objUser = //UserController.GetUser(PortalId, intUserID, false);
					UserController.GetUserById(PortalId, intUserID);

                if (objUser != null)
                {
                    strUserName = objUser.DisplayName;
                }
                else
                {
                    strUserName = Localization.GetString("Anonymous.Text", LocalResourceFile);
                }
            }

            return strUserName;
        }
        #endregion

        // GridView

        #region gvComments_RowCommand
        protected void gvComments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                SetView("Edit");
                lblDetailID.Text = Convert.ToString(e.CommandArgument);
                DisplayComment();
            }
        }
        #endregion

        // Comment Edit

        #region lnkBack_Click
        protected void lnkBack_Click(object sender, EventArgs e)
        {
            SetView("Default");
        }
        #endregion

        #region DisplayComment
        private void DisplayComment()
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            var objHelpDesk_TaskDetail = (from HelpDesk_TaskDetails in objHelpDeskDALDataContext.HelpDesk_TaskDetails
                                              where HelpDesk_TaskDetails.DetailID == Convert.ToInt32(lblDetailID.Text)
                                              select HelpDesk_TaskDetails).FirstOrDefault();

            if (objHelpDesk_TaskDetail != null)
            {
                txtDescription.Text = objHelpDesk_TaskDetail.Description;
                lblDisplayUser.Text = GetUserName(objHelpDesk_TaskDetail.UserID);
                lblInsertDate.Text = String.Format("{0} {1}", objHelpDesk_TaskDetail.InsertDate.ToLongDateString(), objHelpDesk_TaskDetail.InsertDate.ToLongTimeString());
                chkCommentVisibleEdit.Checked = (objHelpDesk_TaskDetail.DetailType == "Comment") ? false : true;

                if (!ViewOnly)
                {
                    ImgEmailUser.Visible = (objHelpDesk_TaskDetail.DetailType == "Comment") ? false : true;
                    lnkUpdateRequestor.Visible = (objHelpDesk_TaskDetail.DetailType == "Comment") ? false : true;
                }

                // Only set the Display of the Email to Requestor link if it is already showing
                if (lnkUpdateRequestor.Visible)
                {
                    // Only Display Email to Requestor link if chkCommentVisibleEdit is checked
                    lnkUpdateRequestor.Visible = chkCommentVisibleEdit.Checked;
                    ImgEmailUser.Visible = chkCommentVisibleEdit.Checked;
                }

                if (objHelpDesk_TaskDetail.HelpDesk_Attachments.Count > 0)
                {
                    // There is a atachment
                    pnlAttachFile.Visible = false;
                    pnlDisplayFile.Visible = true;

                    lnkFileAttachment.Text = objHelpDesk_TaskDetail.HelpDesk_Attachments.FirstOrDefault().OriginalFileName;
					lnkFileAttachment.CommandArgument = objHelpDesk_TaskDetail.HelpDesk_Attachments.FirstOrDefault().AttachmentID.ToString();
					/*
					#region Attachment URL 


					var commandArgument  = objHelpDesk_TaskDetail.HelpDesk_Attachments.FirstOrDefault().AttachmentID.ToString();

					var objHelpDesk_Attachment = objHelpDesk_TaskDetail.HelpDesk_Attachments.FirstOrDefault();

					if (objHelpDesk_Attachment != null)
					{
						var strPath = objHelpDesk_Attachment.AttachmentPath;
						var strOriginalFileName = objHelpDesk_Attachment.OriginalFileName;


						lnkFileAttachment.Visible = true;
						lnkFileAttachment.NavigateUrl = "/DesktopModules/R7.HelpDesk/Upload/" + 
					}
					else
						lnkFileAttachment.Visible = false;
                    
					#endregion */
				}
                else
                {
                    // Only do this if not in View Only Mode
                    if (!ViewOnly)
                    {
                        // There is not a file attached
                        pnlAttachFile.Visible = true;
                        pnlDisplayFile.Visible = false;
                    }
                    else
                    {
                        pnlDisplayFile.Visible = false;
                    }
                }
            }
        }
        #endregion

        #region lnkFileAttachment_Click
        protected void lnkFileAttachment_Click(object sender, EventArgs e)
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            var objHelpDesk_Attachment = (from HelpDesk_Attachments in objHelpDeskDALDataContext.HelpDesk_Attachments
                                              where HelpDesk_Attachments.AttachmentID == Convert.ToInt32(lnkFileAttachment.CommandArgument)
                                              select HelpDesk_Attachments).FirstOrDefault();

            if (objHelpDesk_Attachment != null)
            {
                string strPath = objHelpDesk_Attachment.AttachmentPath;
                string strOriginalFileName = objHelpDesk_Attachment.OriginalFileName;

                try
                {
					Response.Clear();
					//Respons.ClearHeaders();
					//Response.HeaderEncoding = System.Text.Encoding.UTF8;

					// Patch info from here:
					// http://gordievskiy.blogspot.ru/2009/12/content-disposition-attachment-filename.html
					var attachmentFilename = strOriginalFileName;

					//attachmentFilename = Request.Browser.Browser;

					if (Request.Browser.Browser.Contains("MSIE") || Request.Browser.Browser.StartsWith("IE"))
					{
						attachmentFilename = Server.UrlEncode(attachmentFilename);
						if (!string.IsNullOrEmpty(attachmentFilename)) 
							attachmentFilename = attachmentFilename.Replace("+", "%20");
					}   
					Response.AddHeader("content-disposition", string.Format("attachment; filename=\"{0}\"", attachmentFilename));

                    //Response.ClearContent();
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.ContentType = Utils.GetMimeType(Path.GetExtension(strPath).ToLower());

                    var sourceFile = new FileStream(strPath, FileMode.Open, FileAccess.Read, FileShare.Read);
					var fileSize = (int)sourceFile.Length;
					//var reader = new BinaryReader(sourceFile);
                    
					/*
					// read by 4k blocks
					const int blockSize = 4096;
					var buffer = new byte[blockSize];
					int bytesRead;
					var bytesLeft = (int)sourceFile.Length;

					do
					{
						bytesRead =	sourceFile.Read(buffer, 0, (bytesLeft > blockSize)? blockSize : bytesLeft);
						if (bytesRead > 0)
						{
							bytesLeft -= bytesRead;
							if (bytesRead < blockSize)
								Array.Resize<byte>(ref buffer, bytesRead);
							Response.BinaryWrite(buffer);
						}
					} while (bytesLeft >= 0);
 					sourceFile.Close();
					*/

					var data = new byte[fileSize];
					sourceFile.Read(data, 0, fileSize);
                    sourceFile.Close();

                    Response.BinaryWrite(data);
					Response.Flush();
                    Response.Close();

                }
                catch (Exception ex)
                {
					// lblError.Text = ex.Message + "<br />" + ex.StackTrace; 
					Response.Clear();
					Response.Write (ex.Message + "<br />" + ex.StackTrace);
					Response.Flush();
					Response.Close();
                }
            }
        }
        #endregion

        #region lnkDelete_Click
        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            var objHelpDesk_TaskDetail = (from HelpDesk_TaskDetails in objHelpDeskDALDataContext.HelpDesk_TaskDetails
                                              where HelpDesk_TaskDetails.DetailID == Convert.ToInt32(lblDetailID.Text)
                                              select HelpDesk_TaskDetails).FirstOrDefault();

            // Delete any Attachments
            if (objHelpDesk_TaskDetail.HelpDesk_Attachments.Count > 0)
            {
                HelpDesk_Attachment objHelpDesk_Attachment = objHelpDesk_TaskDetail.HelpDesk_Attachments.FirstOrDefault();
                string strOriginalFileName = objHelpDesk_Attachment.OriginalFileName;
                string strFile = objHelpDesk_Attachment.AttachmentPath;

                try
                {
                    // Delete file
                    if (strFile != "")
                    {
                        File.Delete(strFile);
                    }
                }
                catch (Exception exc)
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                }

                objHelpDeskDALDataContext.HelpDesk_Attachments.DeleteOnSubmit(objHelpDesk_Attachment);
                objHelpDeskDALDataContext.SubmitChanges();

                // Insert Log
				Log.InsertLog(TaskID, UserId, String.Format(Localization.GetString ("DeletedFile.Text", LocalResourceFile), GetUserName(), strOriginalFileName));
            }

            // Delete the Record
            objHelpDeskDALDataContext.HelpDesk_TaskDetails.DeleteOnSubmit(objHelpDesk_TaskDetail);
            objHelpDeskDALDataContext.SubmitChanges();

            // Insert Log
			Log.InsertLog(TaskID, UserId, String.Format(Localization.GetString ("DeletedComment.Text", LocalResourceFile), GetUserName(), txtDescription.Text));

            SetView("Default");
            gvComments.DataBind();
        }
        #endregion

        #region imgDelete_Click
        protected void imgDelete_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            var objHelpDesk_TaskDetail = (from HelpDesk_TaskDetails in objHelpDeskDALDataContext.HelpDesk_TaskDetails
                                              where HelpDesk_TaskDetails.DetailID == Convert.ToInt32(lblDetailID.Text)
                                              select HelpDesk_TaskDetails).FirstOrDefault();

            // Delete Attachment
            if (objHelpDesk_TaskDetail.HelpDesk_Attachments.Count > 0)
            {
                HelpDesk_Attachment objHelpDesk_Attachment = objHelpDesk_TaskDetail.HelpDesk_Attachments.FirstOrDefault();
                string strOriginalFileName = objHelpDesk_Attachment.OriginalFileName;
                string strFile = objHelpDesk_Attachment.AttachmentPath;

                try
                {
                    // Delete file
                    if (strFile != "")
                    {
                        File.Delete(strFile);
                    }
                }
                catch (Exception exc)
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                }

                objHelpDeskDALDataContext.HelpDesk_Attachments.DeleteOnSubmit(objHelpDesk_Attachment);
                objHelpDeskDALDataContext.SubmitChanges();

                // Insert Log
				Log.InsertLog(TaskID, UserId, String.Format(Localization.GetString ("DeletedFile.Text", LocalResourceFile), GetUserName(), strOriginalFileName));

                pnlAttachFile.Visible = true;
                pnlDisplayFile.Visible = false;
            }
        }
        #endregion

        #region lnkUpdate_Click
        protected void lnkUpdate_Click(object sender, EventArgs e)
        {
            UpdateComment();
        }
        #endregion

        #region lnkUpdateRequestor_Click
        protected void lnkUpdateRequestor_Click(object sender, EventArgs e)
        {
            string strComment = txtDescription.Text;
            UpdateComment();
            NotifyRequestorOfComment(strComment);
        }
        #endregion

        #region UpdateComment
        private void UpdateComment()
        {
            // Validate file upload
            if (fuAttachment.HasFile)
            {
                /*if (
                    string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".gif", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".jpg", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".jpeg", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".doc", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".docx", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".xls", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".xlsx", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".pdf", true) != 0
                    )*/

				if(!Utils.IsFileAllowed(TicketFileUpload.FileName))
				{
					lblError.Text = Localization.GetString ("FileExtensionIsNotAllowed.Text", LocalResourceFile);
					// lblError.Text = "Only .gif, .jpg, .jpeg, .doc, .docx, .xls, .xlsx, .pdf files may be used.";
					return;
				}
            }

			if (!string.IsNullOrWhiteSpace(txtDescription.Text))
		    // if(txtDescription.Text.Trim().Length > 0)
            {
                HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

                string strComment = txtDescription.Text.Trim();

                // Save Task Details
                var objHelpDesk_TaskDetail = (from HelpDesk_TaskDetails in objHelpDeskDALDataContext.HelpDesk_TaskDetails
                                                  where HelpDesk_TaskDetails.DetailID == Convert.ToInt32(lblDetailID.Text)
                                                  select HelpDesk_TaskDetails).FirstOrDefault();

                if (objHelpDesk_TaskDetail != null)
                {

                    objHelpDesk_TaskDetail.TaskID = TaskID;
                    objHelpDesk_TaskDetail.Description = txtDescription.Text.Trim();
                    objHelpDesk_TaskDetail.UserID = UserId;

                    if (chkCommentVisibleEdit.Checked)
                    {
                        objHelpDesk_TaskDetail.DetailType = "Comment-Visible";
                    }
                    else
                    {
                        objHelpDesk_TaskDetail.DetailType = "Comment";
                    }

                    objHelpDeskDALDataContext.SubmitChanges();
                    txtDescription.Text = "";

                    // Insert Log
					Log.InsertLog(TaskID, UserId, String.Format(Localization.GetString ("UpdatedComment.Text", LocalResourceFile), GetUserName()));

                    // Upload the File
                    if (fuAttachment.HasFile)
                    {
                        UploadFileCommentEdit(objHelpDesk_TaskDetail.DetailID);
                    }

                    SetView("Default");
                    gvComments.DataBind();
                }
            }
        }
        #endregion

        #region UploadFileCommentEdit
        private void UploadFileCommentEdit(int intDetailID)
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            string strUploFilesPath = (from HelpDesk_Settings in objHelpDeskDALDataContext.HelpDesk_Settings
                                           where HelpDesk_Settings.PortalID == PortalId
                                           where HelpDesk_Settings.SettingName == "UploFilesPath"
                                           select HelpDesk_Settings).FirstOrDefault().SettingValue;

            EnsureDirectory(new System.IO.DirectoryInfo(strUploFilesPath));
            string strfilename = Convert.ToString(intDetailID) + "_" + GetRandomPassword() + Path.GetExtension(fuAttachment.FileName).ToLower();
            strUploFilesPath = strUploFilesPath + @"\" + strfilename;
            fuAttachment.SaveAs(strUploFilesPath);

            HelpDesk_Attachment objHelpDesk_Attachment = new HelpDesk_Attachment();
            objHelpDesk_Attachment.DetailID = intDetailID;
            objHelpDesk_Attachment.FileName = strfilename;
            objHelpDesk_Attachment.OriginalFileName = fuAttachment.FileName;
            objHelpDesk_Attachment.AttachmentPath = strUploFilesPath;
            objHelpDesk_Attachment.UserID = UserId;

            objHelpDeskDALDataContext.HelpDesk_Attachments.InsertOnSubmit(objHelpDesk_Attachment);
            objHelpDeskDALDataContext.SubmitChanges();

            // Insert Log
			Log.InsertLog(TaskID, UserId, String.Format(Localization.GetString ("UploadedFile.Text.", LocalResourceFile), GetUserName(), fuAttachment.FileName));
        }
        #endregion

        // Emails

        #region NotifyAssignedGroupOfComment
        private void NotifyAssignedGroupOfComment(string strComment)
        {
            RoleController objRoleController = new RoleController();
            string strDescription = GetDescriptionOfTicket();

            // Send to Administrator Role
            string strAssignedRole = "Administrators";
            int intRole = GetAssignedRole();
            if (intRole > -1)
            {
                strAssignedRole = String.Format("{0}", objRoleController.GetRole(intRole, PortalId).RoleName);
            }
            else
            {
                strAssignedRole = GetAdminRole();
            }

            string strLinkUrl = Utils.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleID.ToString(), String.Format(@"&TaskID={0}", TaskID)), PortalSettings.PortalAlias.HTTPAlias);

            string strSubject = String.Format(Localization.GetString("HelpDeskTicketAtHasBeenupdated.Text", LocalResourceFile), Request.QueryString["TaskID"], PortalSettings.PortalAlias.HTTPAlias);
            string strBody = String.Format(Localization.GetString("HelpDeskTicketHasBeenupdated.Text", LocalResourceFile), Request.QueryString["TaskID"], strDescription);
            strBody = strBody + Environment.NewLine + Environment.NewLine;
            strBody = strBody + Localization.GetString("Comments.Text", LocalResourceFile) + Environment.NewLine;
            strBody = strBody + strComment;
            strBody = strBody + Environment.NewLine + Environment.NewLine;
            strBody = strBody + String.Format(Localization.GetString("YouMaySeeFullStatusHere.Text", LocalResourceFile), strLinkUrl);

            // Get all users in the AssignedRole Role
            ArrayList colAssignedRoleUsers = objRoleController.GetUsersByRoleName(PortalId, strAssignedRole);

            foreach (UserInfo objUserInfo in colAssignedRoleUsers)
            {
                DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, objUserInfo.Email, "", strSubject, strBody, "", "HTML", "", "", "", "");
            }

            Log.InsertLog(Convert.ToInt32(Request.QueryString["TaskID"]), UserId, String.Format(Localization.GetString("SentCommentTo.Text", LocalResourceFile), UserInfo.DisplayName, strAssignedRole));
        }
        #endregion

        #region NotifyRequestorOfComment
        private void NotifyRequestorOfComment(string strComment)
        {
            string strEmail = GetEmailOfRequestor();

            if (strEmail != "")
            {
                HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

                var result = (from HelpDesk_TaskDetails in objHelpDeskDALDataContext.HelpDesk_Tasks
                              where HelpDesk_TaskDetails.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                              select HelpDesk_TaskDetails).FirstOrDefault();

                if (result != null)
                {
                    string strLinkUrl = "";
                    if (result.RequesterUserID > -1)
                    {
                        // This is a registred User / Provide link to ticket
                        strLinkUrl = Utils.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleID.ToString(), String.Format(@"&TaskID={0}", TaskID)), PortalSettings.PortalAlias.HTTPAlias);
                    }
                    else
                    {
                        // This is NOT a registred User / Provide link to ticket with a password
                        strLinkUrl = Utils.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleID.ToString(), String.Format(@"&TaskID={0}&TP={1}", TaskID, result.TicketPassword)), PortalSettings.PortalAlias.HTTPAlias);
                    }

                    string strDescription = result.Description;
                    string strSubject = String.Format(Localization.GetString("HelpDeskTicketAtHasBeenupdated.Text", LocalResourceFile), Request.QueryString["TaskID"], PortalSettings.PortalAlias.HTTPAlias);
                    string strBody = String.Format(Localization.GetString("HelpDeskTicketHasBeenupdated.Text", LocalResourceFile), Request.QueryString["TaskID"], strDescription);
                    strBody = strBody + Environment.NewLine + Environment.NewLine;
                    strBody = strBody + Localization.GetString("Comments.Text", LocalResourceFile) + Environment.NewLine;
                    strBody = strBody + strComment;
                    strBody = strBody + Environment.NewLine + Environment.NewLine;
                    strBody = strBody + String.Format(Localization.GetString("YouMaySeeFullStatusHere.Text", LocalResourceFile), strLinkUrl);
                                        
                    DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, strEmail, "", strSubject, strBody, "", "HTML", "", "", "", "");

                    Log.InsertLog(Convert.ToInt32(Request.QueryString["TaskID"]), UserId, String.Format(Localization.GetString("RequestorWasEmailed.Text", LocalResourceFile), strEmail, strComment));

                }
            }
        }
        #endregion

        #region GetAssignedRole
        private int GetAssignedRole()
        {
            int intRole = -1;

            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();
            var result = from HelpDesk_TaskDetails in objHelpDeskDALDataContext.HelpDesk_Tasks
                         where HelpDesk_TaskDetails.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                         select HelpDesk_TaskDetails;

            if (result != null)
            {
                intRole = result.FirstOrDefault().AssignedRoleID;
            }

            return intRole;
        }
        #endregion

        #region UserIsRequestor
        private bool UserIsRequestor()
        {
            bool isRequestor = false;

            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();
            var result = from HelpDesk_TaskDetails in objHelpDeskDALDataContext.HelpDesk_Tasks
                         where HelpDesk_TaskDetails.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                         select HelpDesk_TaskDetails;

            if (result != null)
            {
                if (UserId == result.FirstOrDefault().RequesterUserID)
                {
                    isRequestor = true;
                }
            }

            return isRequestor;
        }
        #endregion

        // Visible to Requestor CheckBox

        #region chkCommentVisibleEdit_CheckedChanged
        protected void chkCommentVisibleEdit_CheckedChanged(object sender, EventArgs e)
        {
            // Only Display Email to Requestor link if chkCommentVisibleEdit is checked
            lnkUpdateRequestor.Visible = chkCommentVisibleEdit.Checked;
            ImgEmailUser.Visible = chkCommentVisibleEdit.Checked;
        }
        #endregion

        #region chkCommentVisible_CheckedChanged
        protected void chkCommentVisible_CheckedChanged(object sender, EventArgs e)
        {
            // Only Display Email link if chkCommentVisibleEdit is checked
            btnInsertCommentAndEmail.Visible = chkCommentVisible.Checked;
        }
        #endregion

        // Utility

        #region GetEmailOfRequestor
        private string GetEmailOfRequestor()
        {
            string strEmail = "";
            int intTaskId = Convert.ToInt32(Request.QueryString["TaskID"]);

            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();
            var result = (from HelpDesk_TaskDetails in objHelpDeskDALDataContext.HelpDesk_Tasks
                          where HelpDesk_TaskDetails.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                          select HelpDesk_TaskDetails).FirstOrDefault();

            if (result != null)
            {
                if (result.RequesterUserID == -1)
                {
                    try
                    {
                        strEmail = result.RequesterEmail;
                    }
                    catch (Exception)
                    {
                        // User no longer exists
                        strEmail = "";
                    }
                }
                else
                {
                    try
                    {
                        strEmail = //UserController.GetUser(PortalId, result.RequesterUserID, false).Email;
							UserController.GetUserById(PortalId, result.RequesterUserID).Email;
                    }
                    catch (Exception)
                    {
                        // User no longer exists
                        strEmail = "";
                    }
                }
            }

            return strEmail;
        }
        #endregion

        #region GetDescriptionOfTicket
        private string GetDescriptionOfTicket()
        {
            string strDescription = "";
            int intTaskId = Convert.ToInt32(Request.QueryString["TaskID"]);

            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();
            var result = (from HelpDesk_TaskDetails in objHelpDeskDALDataContext.HelpDesk_Tasks
                          where HelpDesk_TaskDetails.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                          select HelpDesk_TaskDetails).FirstOrDefault();

            if (result != null)
            {
                strDescription = result.Description;
            }

            return strDescription;
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
                objHelpDesk_Setting2.SettingValue = Server.MapPath("~/DesktopModules/R7.HelpDesk/Upload");

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

        #region GetAdminRole
        private string GetAdminRole()
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            List<HelpDesk_Setting> colHelpDesk_Setting = (from HelpDesk_Settings in objHelpDeskDALDataContext.HelpDesk_Settings
                                                                  where HelpDesk_Settings.PortalID == PortalId
                                                                  select HelpDesk_Settings).ToList();

            HelpDesk_Setting objHelpDesk_Setting = colHelpDesk_Setting.Where(x => x.SettingName == "AdminRole").FirstOrDefault();

            string strAdminRoleID = "Administrators";
            if (objHelpDesk_Setting != null)
            {
                strAdminRoleID = objHelpDesk_Setting.SettingValue;
            }

            return strAdminRoleID;
        }
        #endregion

        #region GetUploadPermission
        private string GetUploadPermission()
        {
            List<HelpDesk_Setting> objHelpDesk_Settings = GetSettings();
            HelpDesk_Setting objHelpDesk_Setting = objHelpDesk_Settings.Where(x => x.SettingName == "UploadPermission").FirstOrDefault();

            string strUploadPermission = "All";
            if (objHelpDesk_Setting != null)
            {
                strUploadPermission = objHelpDesk_Setting.SettingValue;
            }

            return strUploadPermission;
        }
        #endregion
    }
}
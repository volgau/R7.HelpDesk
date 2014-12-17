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
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace R7.HelpDesk
{
    public partial class Logs : DotNetNuke.Entities.Modules.PortalModuleBase
    {
        #region Properties
        public int TaskID
        {
            get { return Convert.ToInt32(ViewState["TaskID"]); }
            set { ViewState["TaskID"] = value; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
			gvLogs.Columns [0].HeaderText = Localization.GetString ("Description.Text", LocalResourceFile);
			gvLogs.Columns [1].HeaderText = Localization.GetString ("Date.Text");
        }

        #region LDSLogs_Selecting
        protected void LDSLogs_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();
            var result = from HelpDesk_Logs in objHelpDeskDALDataContext.HelpDesk_Logs
                         where HelpDesk_Logs.TaskID == TaskID
                         select HelpDesk_Logs;

            e.Result = result;
        } 
        #endregion

        #region RefreshLogs
        public void RefreshLogs()
        {
            gvLogs.DataBind();
        } 
        #endregion
}
}
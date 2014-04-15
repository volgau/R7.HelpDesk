//
// webserver.com
// Copyright (c) 2009
// by Michael Washington
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
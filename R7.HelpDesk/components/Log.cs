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
using System.Web.UI;
using System.Collections;
using System.Web.UI.WebControls;
//using Microsoft.VisualBasic;

namespace R7.HelpDesk
{
    public class Log
    {
        public Log()
        {
        }

        #region InsertLog
        public static void InsertLog(int TaskID, int UserID, string LogDescription)
        {
            HelpDeskDALDataContext objHelpDeskDALDataContext = new HelpDeskDALDataContext();

            HelpDesk_Log objHelpDesk_Log = new HelpDesk_Log();
            objHelpDesk_Log.DateCreated = DateTime.Now;
            objHelpDesk_Log.LogDescription = Utils.StringLeft(LogDescription, 499);
            objHelpDesk_Log.TaskID = TaskID;
            objHelpDesk_Log.UserID = UserID;

            objHelpDeskDALDataContext.HelpDesk_Logs.InsertOnSubmit(objHelpDesk_Log);
            objHelpDeskDALDataContext.SubmitChanges();
        } 
        #endregion
    }
}

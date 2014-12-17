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

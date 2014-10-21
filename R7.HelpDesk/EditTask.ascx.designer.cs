using System;
using System.Web.UI.WebControls;

namespace R7.HelpDesk
{
	public partial class EditTask
	{
		protected HyperLink cmdtxtDueDateCalendar;
		protected HyperLink cmdtxtStartCalendar;
		protected HyperLink cmdtxtCompleteCalendar;

		protected TextBox txtDueDate;
		protected TextBox txtComplete;
		protected TextBox txtStart;

		protected Label lblTask;

		protected Comments CommentsControl;
		protected Work WorkControl;
		protected Logs LogsControl;
		protected Panel pnlEditTask;

		protected Button btnSave;
		protected Button btnComments;
		protected Button btnWorkItems;
		protected Button btnLogs;
		protected DropDownList ddlAssigned;
		protected DropDownList ddlStatus;
		protected DropDownList ddlPriority;

		protected Tags TagsTreeExistingTasks;
		protected Image imgTags;
		protected Label lbltxtTags;

		protected TextBox txtEmail;
		protected TextBox txtName;
		protected Label lblEmail;
		protected Label lblName;
		protected Label lblCreated;
		protected TextBox txtDescription;
		protected TextBox txtPhone;
		protected TextBox txtEstimate;
		protected Label lblError;

		protected Panel pnlComments;
		protected Panel pnlWorkItems;
		protected Panel pnlLogs;
	}
}


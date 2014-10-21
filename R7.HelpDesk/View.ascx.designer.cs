using System;
using System.Web.UI.WebControls;

namespace R7.HelpDesk
{
	public partial class View
	{
		//protected Panel pnlAdminSettings;
		protected HyperLink cmdStartCalendar;
		protected TextBox txtUserID;
		protected DropDownList ddlPageSize;
		protected LinkButton lnkExistingTickets;
		protected ListView lvTasks;
		protected HyperLink lnkTaskID;

		protected TextBox txtDueDate;
		protected HyperLink lnkAdministratorSettings;
		protected FileUpload TicketFileUpload;
		protected Label lblAttachFile;
		protected Panel pnlAdminUserSelection;
		protected Panel pnlAdminTicketStatus;

		protected DropDownList ddlAssignedAdmin;
		protected Tags TagsTree;
		protected Tags TagsTreeExistingTasks;

		protected Image imgTags;
		protected Image	img2Tags;
		protected Label lblCheckTags;
		protected Label lblSearchTags;

		protected TextBox txtName;
		protected TextBox txtEmail;
		protected TextBox txtPhone;
		protected TextBox txtDescription;
		protected TextBox txtDetails;
		protected DropDownList ddlPriority;

		protected Panel pnlNewTicket;
		protected Panel pnlExistingTickets;
		protected Panel pnlConfirmAnonymousUserEntry;
		protected Image imgMagnifier;
		protected LinkButton lnkResetSearch;
		protected LinkButton	lnkNewTicket;
		protected Label lblName;
		protected Label lblEmail;

		protected Button btnClearUser;
		protected Label lblConfirmAnonymousUser;
		protected Label lblError;
		protected DropDownList ddlStatusAdmin;
		protected TextBox txtSearchForUser;

		protected DropDownList ddlSearchForUserType;
		protected Label lblCurrentProcessorNotFound;
		protected GridView gvCurrentProcessor;
		protected LinkButton lnkFirst;
		protected LinkButton lnkPrevious;
		protected LinkButton lnkNext;
		protected LinkButton lnkLast;
		protected Panel pnlPaging;
		protected DataList PagingDataList;




	}
}


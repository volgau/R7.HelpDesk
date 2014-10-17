using System;
using System.Web.UI.WebControls;

namespace R7.HelpDesk
{
	public partial class Comments
	{
		protected Panel pnlInsertComment;
		protected Label lblAttachFile1;
		protected FileUpload TicketFileUpload;
		protected Label lblAttachFile2;
		protected FileUpload fuAttachment;
		protected Panel pnlTableHeader;
		protected Panel pnlExistingComments;
		protected Panel pnlEditComment;

		protected CheckBox chkCommentVisible;
		protected CheckBox chkCommentVisibleEdit;
		protected LinkButton lnkDelete;
		protected Image Image5;
		protected LinkButton lnkUpdate;
		protected Image Image4;
		protected Panel pnlDisplayFile;
		protected Panel pnlAttachFile;
		protected Image imgDelete;
		protected LinkButton lnkUpdateRequestor;
		protected Image ImgEmailUser;
		protected Button btnInsertCommentAndEmail;

		protected TextBox txtComment;
		protected Label lblError;
		protected GridView gvComments;
		protected Label lblDetailID;
		protected TextBox txtDescription;
		protected Label lblDisplayUser;
		protected Label lblInsertDate;
		protected LinkButton lnkFileAttachment;
		//protected HyperLink lnkFileAttachment;

		protected Label lblErrorEditComment;

	}
}


using System;
using System.Web.UI.WebControls;

namespace R7.HelpDesk
{
	public partial class AdminSettings
	{
		protected Panel pnlAdminSettings;
		protected Panel pnlAdministratorRole;
		protected Panel pnlUploFilesPath;
		protected Panel pnlTagsAdmin;
		protected Panel pnlRoles;

		protected Button btnAddNew;
		protected Button btnUpdate;
		protected HyperLink lnkAdminRole;
		protected HyperLink lnkUploFilesPath;
		protected HyperLink lnkTagsAdmin;
		protected HyperLink lnkRoles;

		protected DropDownList ddlAdminRole;
		protected TextBox txtUploadedFilesPath;
		protected DropDownList ddlUploadPermission;

		protected TreeView tvCategories;
		protected Label lblAdminRole;
		protected Label lblUploadedFilesPath;
		protected TextBox txtCategoryID;

		protected DropDownList ddlParentCategory;
		protected TextBox txtCategory;
		protected CheckBox chkRequesterVisible;
		protected CheckBox chkSelectable;
		protected TextBox txtParentCategoryID;

		protected Button btnDelete;
		protected Label lblTagError;
		protected Label lblRoleError;
		protected DropDownList ddlRole;
		protected ListView lvRoles;

	}
}


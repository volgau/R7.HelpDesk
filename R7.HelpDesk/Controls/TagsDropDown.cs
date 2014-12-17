using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Collections;
using System.Web.UI.WebControls;

namespace R7.HelpDesk
{
    public class CategoriesDropDown
    {
        private IQueryable<HelpDesk_Category> EntireTable;

        private int _PortalID;
        public int PortalID
        {
            get { return _PortalID; }
            set { _PortalID = value; }
        }
        public CategoriesDropDown(int intPortalID)
        {
            _PortalID = intPortalID;
            GetEntireTable();
        }

        #region GetEntireTable
        private void GetEntireTable()
        {
            // Get the entire table only once from cache
            EntireTable = CategoriesTable.GetCategoriesTable(_PortalID, false);
        }
        #endregion

        #region Categories
        public ListItemCollection Categories(int BranchNotToShow)
        {
            // Create a Collection to hold the final results
            ListItemCollection colListItemCollection = new ListItemCollection();

            // Get the top level
            var results = from WebserverCategories in EntireTable
                          where WebserverCategories.PortalID == PortalID
                          where WebserverCategories.Level == 1
                          where WebserverCategories.CategoryID != BranchNotToShow
                          orderby WebserverCategories.CategoryName
                          select WebserverCategories;

            // Create a none item
            ListItem objDefaultListItem = new ListItem();
            objDefaultListItem.Text = "[None]";
            objDefaultListItem.Value = "0";
            colListItemCollection.Add(objDefaultListItem);

            // Loop thru the top level
            foreach (HelpDesk_Category objWebserverCategories in results)
            {
                // Create a top level item
                ListItem objListItem = new ListItem();
                objListItem.Text = objWebserverCategories.CategoryName;
                objListItem.Value = objWebserverCategories.CategoryID.ToString();
                // Add a top level item to the final collection
                colListItemCollection.Add(objListItem);

                // Add the children of the top level item
                // Pass the current collection and the current top level item
                AddChildren(colListItemCollection, objWebserverCategories, BranchNotToShow);
            }

            return colListItemCollection;
        }
        #endregion

        #region AddChildren
        private void AddChildren(ListItemCollection colListItemCollection, HelpDesk_Category objHelpDesk_Category, int BranchNotToShow)
        {

            // Get the children of the current item
            // This method may be called from the top level or recuresively by one of the child items
            var ChildResults = from WebserverCategories in EntireTable
                               where WebserverCategories.ParentCategoryID == objHelpDesk_Category.CategoryID
                               where WebserverCategories.CategoryID != BranchNotToShow
                               select WebserverCategories;

            // Loop thru each item
            foreach (HelpDesk_Category objCategory in ChildResults)
            {
                // Create a new list item to add to the collection
                ListItem objChildListItem = new ListItem();
                // AddDots method is used to add the dots to indicate the item is a sub item
                objChildListItem.Text = String.Format("{0}{1}", AddDots(objCategory.Level), objCategory.CategoryName);
                objChildListItem.Value = objCategory.CategoryID.ToString();
                colListItemCollection.Add(objChildListItem);

                //Recursively call the AddChildren method adding all children
                AddChildren(colListItemCollection, objCategory, BranchNotToShow);
            }
        }
        #endregion

        #region AddDots
        private static string AddDots(int? intDots)
        {
            String strDots = "";

            for (int i = 0; i < intDots; i++)
            {
                strDots += ". ";
            }

            return strDots;
        }
        #endregion
    }
}
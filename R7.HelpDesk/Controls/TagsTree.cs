using System;
using System.Linq;
using System.Web.UI;
using System.Collections;
using System.Web.UI.WebControls;

namespace R7.HelpDesk
{
    public class HelpDeskTree : HierarchicalDataSourceControl, IHierarchicalDataSource
    {
        private int _PortalID;
        private bool _RequestorR7_HelpDesk;

        public HelpDeskTree(int PortalID, bool RequestorR7_HelpDesk)
            : base()
        {
            _PortalID = PortalID;
            _RequestorR7_HelpDesk = RequestorR7_HelpDesk;
        }

        // Return a strongly typed view for the current data source control.
        private HelpDeskView view = null;

        #region GetHierarchicalView
        protected override HierarchicalDataSourceView GetHierarchicalView(string viewPath)
        {
            if (null == view)
            {
                view = new HelpDeskView(viewPath, _PortalID, _RequestorR7_HelpDesk);
            }
            return view;
        }
        #endregion

        #region CreateControlCollection
        // The DataSource can be used declaratively. To enable
        // declarative use, override the default implementation of
        // CreateControlCollection to return a ControlCollection that
        // you can add to.
        protected override ControlCollection CreateControlCollection()
        {
            return new ControlCollection(this);
        }
        #endregion
    }

    #region HelpDeskView
    public class HelpDeskView : HierarchicalDataSourceView
    {
        private string _viewPath;
        private int _PortalID;
        private bool _RequestorR7_HelpDesk;

        public HelpDeskView(string viewPath, int PortalID, bool RequestorR7_HelpDesk)
        {
            // This implementation of HierarchicalDataSourceView does not
            // use the viewPath parameter but other implementations
            // could make use of it for retrieving values.
            _viewPath = viewPath;
            _PortalID = PortalID;
            _RequestorR7_HelpDesk = RequestorR7_HelpDesk;
        }

        #region Select
        // Starting with the rootNode, recursively build a list of
        // nodes, create objects, add them all to the collection,
        // and return the list.
        public override IHierarchicalEnumerable Select()
        {
            HelpDeskEnumerable CHE = new HelpDeskEnumerable();

            // Get the top level
            var results = from WebserverCategories in CategoriesTable.GetCategoriesTable(_PortalID, _RequestorR7_HelpDesk)
                          where WebserverCategories.PortalID == _PortalID
                          where WebserverCategories.Level == 1
                          orderby WebserverCategories.CategoryName
                          select WebserverCategories;

            // Loop thru the top level
            foreach (HelpDesk_Category objHelpDesk_Category in results)
            {
                // Create a top level item
                ListItem objListItem = new ListItem();
                objListItem.Text = objHelpDesk_Category.CategoryName;
                objListItem.Value = objHelpDesk_Category.CategoryID.ToString();
                objListItem.Attributes.Add("PortalId", _PortalID.ToString());
                objListItem.Attributes.Add("Selectable", objHelpDesk_Category.Selectable.ToString());
                objListItem.Attributes.Add("RequestorVisible", objHelpDesk_Category.RequestorVisible.ToString());
                objListItem.Attributes.Add("RequestorR7_HelpDesk", _RequestorR7_HelpDesk.ToString());

                // Add a top level item to the final collection
                CHE.Add(new HelpDeskHierarchyData(objListItem));
            }

            return CHE;
        }
        #endregion

        #region HelpDeskEnumerable
        // A collection of HelpDeskHierarchyData objects
        public class HelpDeskEnumerable : ArrayList, IHierarchicalEnumerable
        {
            public HelpDeskEnumerable()
                : base()
            {
            }

            public IHierarchyData GetHierarchyData(object enumeratedItem)
            {
                return enumeratedItem as IHierarchyData;
            }
        }
        #endregion

        #region HelpDeskHierarchyData
        public class HelpDeskHierarchyData : IHierarchyData
        {
            public HelpDeskHierarchyData(ListItem obj)
            {
                objListItem = obj;
            }

            private ListItem objListItem = new ListItem();

            public override string ToString()
            {
                return objListItem.Text;
            }

            #region HasChildren
            // IHierarchyData implementation.
            public bool HasChildren
            {
                get
                {
                    // Get the children of the current item
                    AttributeCollection objAttributeCollection = objListItem.Attributes;
                    int intPortalID = Convert.ToInt32(objAttributeCollection["PortalID"]);
                    bool boolRequestorR7_HelpDesk = Convert.ToBoolean(objAttributeCollection["RequestorR7_HelpDesk"]);

                    var ChildResults = from WebserverCategories in CategoriesTable.GetCategoriesTable(intPortalID, boolRequestorR7_HelpDesk)
                                       where WebserverCategories.ParentCategoryID == Convert.ToInt32(objListItem.Value)
                                       select WebserverCategories;

                    return ChildResults.Count() > 0;
                }
            }
            #endregion

            #region string Path
            public string Path
            {
                get
                {
                    return objListItem.Value;
                }
            }
            #endregion

            #region object Item
            public object Item
            {
                get
                {
                    return objListItem;
                }
            }
            #endregion

            #region string Type
            public string Type
            {
                get
                {
                    return "ListItem";
                }
            }
            #endregion

            #region string Text
            public string Text
            {
                get
                {
                    return objListItem.Text;
                }
            }
            #endregion

            #region string Value
            public string Value
            {
                get
                {
                    return objListItem.Value;
                }
            }
            #endregion

            #region GetChildren
            public IHierarchicalEnumerable GetChildren()
            {
                AttributeCollection objAttributeCollection = objListItem.Attributes;
                int intPortalID = Convert.ToInt32(objAttributeCollection["PortalID"]);
                bool boolRequestorR7_HelpDesk = Convert.ToBoolean(objAttributeCollection["RequestorR7_HelpDesk"]);

                var ChildResults = from WebserverCategories in CategoriesTable.GetCategoriesTable(intPortalID, boolRequestorR7_HelpDesk)
                                   where WebserverCategories.ParentCategoryID == Convert.ToInt32(objListItem.Value)
                                   orderby WebserverCategories.CategoryName
                                   select WebserverCategories;

                HelpDeskEnumerable children = new HelpDeskEnumerable();

                // Loop thru each item
                foreach (HelpDesk_Category objCategory in ChildResults)
                {
                    // Create a new list item to add to the collection
                    ListItem objChildListItem = new ListItem();
                    // AddDots method is used to add the dots to indicate the item is a sub item
                    objChildListItem.Text = String.Format("{0}", objCategory.CategoryName);
                    objChildListItem.Value = objCategory.CategoryID.ToString();
                    objChildListItem.Attributes.Add("PortalID", intPortalID.ToString());
                    objChildListItem.Attributes.Add("Selectable", objCategory.Selectable.ToString());
                    objChildListItem.Attributes.Add("RequestorVisible", objCategory.RequestorVisible.ToString());
                    objListItem.Attributes.Add("RequestorR7_HelpDesk", boolRequestorR7_HelpDesk.ToString());

                    children.Add(new HelpDeskHierarchyData(objChildListItem));
                }

                return children;
            }
            #endregion

            #region GetParent
            public IHierarchyData GetParent()
            {
                HelpDeskEnumerable parentContainer = new HelpDeskEnumerable();

                AttributeCollection objAttributeCollection = objListItem.Attributes;
                int intPortalID = Convert.ToInt32(objAttributeCollection["PortalID"]);
                bool boolRequestorR7_HelpDesk = Convert.ToBoolean(objAttributeCollection["RequestorR7_HelpDesk"]);

                var ParentResult = (from WebserverCategories in CategoriesTable.GetCategoriesTable(intPortalID, boolRequestorR7_HelpDesk)
                                    where WebserverCategories.ParentCategoryID == Convert.ToInt32(objListItem.Value)
                                    select WebserverCategories).FirstOrDefault();

                // Create a new list item to add to the collection
                ListItem objChildListItem = new ListItem();
                // AddDots method is used to add the dots to indicate the item is a sub item
                objChildListItem.Text = String.Format("{0}", ParentResult.CategoryName);
                objChildListItem.Value = ParentResult.CategoryID.ToString();
                objChildListItem.Attributes.Add("PortalID", intPortalID.ToString());
                objChildListItem.Attributes.Add("Selectable", ParentResult.Selectable.ToString());
                objChildListItem.Attributes.Add("RequestorVisible", ParentResult.RequestorVisible.ToString());

                return new HelpDeskHierarchyData(objChildListItem);
            }
            #endregion
        }
        #endregion
    }
    #endregion
}
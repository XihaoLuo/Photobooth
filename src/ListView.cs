using System;
using Gtk;

namespace Photobooth
{
    class ListView : ScrolledWindow
    {
        ListStore _store;
        TreeView _view;
        int _selected = -1;

        /// <summary>
        /// Implements a list view of string objects, identifiable by id
        /// e.g., the first element has id = 0 and the last has id = NumItems - 1
        /// </summary>
        /// <param name="headingLabel">The title to show above the list</param>
        public ListView(string headingLabel) 
        {

            _store = new ListStore(typeof(string), typeof(int));

            _view = new TreeView(_store);
            _view.Selection.Changed += OnRowActivated;

            Add(_view);
            AddColumn(_view, headingLabel);
            SetSizeRequest(200,600);
        }


        /// <summary>
        /// Read-only property containing the number of items in the list
        /// </summary>
        /// <value> Read-only property containing the number of items in the list</value>
        public int NumItems
        {
            get { return _store.IterNChildren(); }
        }

        /// <summary>
        /// The ID of currently selected layer if NumLayers > 0; -1 otherwise
        /// </summary>
        /// <value>ID of currently selected layer if NumLayers > 0; -1 otherwise</value>
        public int Selected
        {
            get 
            {
                return _selected;
            }

            set
            {
                _view.Selection.SelectPath(new TreePath(value.ToString()));
            }
        }

        /// <summary>
        /// Adds an item with the given label to the end of the list
        /// </summary>
        /// <param name="label">A non-empty string</param>
        public void AddItem(string label)
        {
            _store.AppendValues(label, NumItems);
        }

        /// <summary>
        /// Clears the list of all items and sets the selection to -1
        /// </summary>
        public void Clear()
        {
            _store.Clear();
            _selected = -1;
        }

        void OnRowActivated(object sender, EventArgs args)
        {
            TreeIter iter;
            ITreeModel model;

            TreeSelection selection = (TreeSelection) sender;
            if (selection.GetSelected (out model, out iter))
            {
                _selected = (int) model.GetValue (iter, 1);
            }
        }

        void AddColumn(TreeView treeView, string heading)
        {
            CellRendererText rendererText = new CellRendererText();
            TreeViewColumn column = new TreeViewColumn(heading, rendererText, "text", 0);
            column.SortColumnId = 0;
            treeView.AppendColumn(column);
        }
    }
}
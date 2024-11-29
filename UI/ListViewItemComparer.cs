using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WA_Send_API.UI
{
    public class ListViewItemComparer : IComparer
    {
        private int _column;
        private SortOrder _order;

        public ListViewItemComparer(int column, SortOrder order)
        {
            this._column = column;
            this._order = order;
        }

        #region IComparer Members

        public int Compare(object x, object y)
        {
            int result = -1;
            if (this._column == 0)
            {
                DateTime a = DateTime.Parse((x as ListViewItem).SubItems[this._column].Text);
                DateTime b = DateTime.Parse((y as ListViewItem).SubItems[this._column].Text);
                result = DateTime.Compare(a, b);
            }
            else
            {
                result = string.Compare((x as ListViewItem).SubItems[this._column].Text, (y as ListViewItem).SubItems[this._column].Text);
            }
            if (this._order == SortOrder.Descending)
                result *= -1;
            return result;
        }

        #endregion
    }
}

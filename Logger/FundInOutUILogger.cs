using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WA_Send_API.Logger
{
    public class FundInOutUILogger : LoggerBase
    {
        private ListView _listView;

        public FundInOutUILogger(ListView LvwList)
        {
            this._listView = LvwList;
        }

        private void CrossThreadClearItem()
        {
            try
            {
                if (this._listView.InvokeRequired)
                    this._listView.Invoke(new CrossThreadListViewClearItemHandler(CrossThreadClearItem));
                else
                {
                    lock (this._listView)
                    {
                        if (this._listView.Items.Count > 500)
                            this._listView.Items.Clear();
                    }
                }
            }
            catch { }
        }

        private void CrossThreadAddItem(ListViewItem lvi)
        {
            try
            {
                if (this._listView.InvokeRequired)
                    this._listView.Invoke(new CrossThreadListViewAddItemHandler(CrossThreadAddItem), lvi);
                else
                {
                    lock (this._listView)
                    {
                        this._listView.BeginUpdate();
                        this._listView.Items.Add(lvi);
                        this._listView.EndUpdate();
                        this._listView.EnsureVisible(this._listView.Items.Count - 1);
                    }
                }
            }
            catch { }
        }

        public override void Log(DateTime time, LogType type, string message)
        {
            if (this._listView == null)
                return;

            ListViewItem lvi = new ListViewItem(time.ToString(CultureInfo.InvariantCulture));
            lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, type.ToString()));
            lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, message));

            switch (type)
            {
                case LogType.ERROR:
                    lvi.ForeColor = Color.Red;
                    break;
                case LogType.INFO:
                    lvi.ForeColor = Color.Black;
                    break;
                case LogType.WARN:
                    lvi.ForeColor = Color.Orange;
                    break;
                case LogType.EXEC:
                    lvi.ForeColor = Color.Maroon;
                    break;
                case LogType.COMP:
                    lvi.ForeColor = Color.Maroon;
                    break;
            }

            CrossThreadClearItem();
            CrossThreadAddItem(lvi);
        }

        public override void LogException(DateTime time, LogType type, string message, string stacktrace)
        {
            if (this._listView == null)
                return;

            ListViewItem lvi = new ListViewItem(time.ToString(CultureInfo.InvariantCulture));
            lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, type.ToString()));
            lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, message));
            lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, stacktrace));

            switch (type)
            {
                case LogType.ERROR:
                    lvi.ForeColor = Color.Red;
                    break;
                case LogType.INFO:
                    lvi.ForeColor = Color.Black;
                    break;
                case LogType.WARN:
                    lvi.ForeColor = Color.Orange;
                    break;
            }

            CrossThreadClearItem();
            CrossThreadAddItem(lvi);
        }
    }

}

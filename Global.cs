using System.Windows.Forms;

namespace WA_Send_API
{
    public delegate void CrossThreadListViewClearItemHandler();
    public delegate void CrossThreadListViewAddItemHandler(ListViewItem item);
    public delegate void DbConnectionStateChangedHandler();
    public delegate void SortStatusLogHandler(object column);
    public delegate void CrossThreadLabelQueueHandler(double i);
    public delegate void CrossThreadLabelIntradayHandler(string i);
}

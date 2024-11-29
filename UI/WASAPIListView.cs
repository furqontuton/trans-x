using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WA_Send_API.UI
{
    public class WASAPIListView : ListView
    {
        public WASAPIListView() 
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}

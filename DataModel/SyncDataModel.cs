using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WA_Send_API.DataModel
{
    public class SyncDataModel : ColumnBase
    {
        public string CountCC { get; set; }
        public string CountClient { get; set; }
        public string CountStock { get; set; }



        public SyncDataModel()
        {
            DataType = ColumnType.GetSyncData;
        }
    }
}

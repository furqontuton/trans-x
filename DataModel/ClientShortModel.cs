using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WA_Send_API.DataModel
{
    internal class ClientShortModel : ColumnBase
    {
        public string CustID { get; set; }

        public string StockCode { get; set; }
        public string Short { get; set; }

        public ClientShortModel()
        {
            DataType = ColumnType.OrderCheckType;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WA_Send_API.DataModel
{
    internal class OrderCheckModel : ColumnBase
    {
        public string Orderstatus_Preop_Open { get; set; }

        public string Orderstatus_Preop_Reject { get; set; }

        public string Orderstatus_Opening_Open { get; set; }

        public string Orderstatus_Opening_Reject { get; set; }

        public OrderCheckModel()
        {
            DataType = ColumnType.OrderCheckType;
        }
    }
}

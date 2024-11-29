using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WA_Send_API.DataModel
{
    public class GetOrderModel : ColumnBase
    {
        public string Total_Order {  get; set; }



        public GetOrderModel()
        {
            DataType = ColumnType.GetOrderType;
        }
    }
}

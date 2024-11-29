using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WA_Send_API.DataModel
{
    public class FundInOutModel : ColumnBase
    {
        public DateTime TimeStamp { get; set; }
        public string ClientID { get; set; }
        public double Amount { get; set; }
        public string InOut { get; set; }
        public int Fundinoutnid { get; set; }
        public string ClientName { get; set; }
        public string MobilePhone { get; set; }
        public string ApproveTime { get; set; }
        public string RejectTime { get; set; }
        public bool Rejected { get; set; }
        public bool Approved { get; set; }



        public FundInOutModel()
        {
            DataType = ColumnType.FundInOutModelType;
        }
    }
}

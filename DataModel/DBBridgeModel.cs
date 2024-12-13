using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WA_Send_API.DataModel
{
    public class DBBridgeModel : ColumnBase
    {

        public string CountDBBridgeCC { get; set; }
        public string CountDBBridgeCS { get; set; }
        public string CountDBBridgeClient { get; set; }
        public string CountDBBridgeUser { get; set; }
        public string CountDBBridgeUser2 { get; set; }
        public string CountDBBridgeCC2 { get; set; }





        public DBBridgeModel()
        {
            DataType = ColumnType.DBBridgeModelType;
        }
    }
}

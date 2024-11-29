using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WA_Send_API.DataModel
{
    public class DBFOModel : ColumnBase
    {

        public string CountDBFOCC { get; set; }
        public string CountDBFOCS { get; set; }
        public string CountDBFOClient { get; set; }
        public string CountDBFOUser { get; set; }





        public DBFOModel()
        {
            DataType = ColumnType.DBFOModelType;
        }
    }
}

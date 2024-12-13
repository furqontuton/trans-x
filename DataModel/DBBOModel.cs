using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WA_Send_API.DataModel
{
    public class DBBOModel : ColumnBase
    {
        //Trus
        public string CountDBBOCCTrus { get; set; }
        public string CountDBBOCSTrus { get; set; }
        public string CountDBBOClientTrus { get; set; }
        public string CountDBBOUserTrus { get; set; }

        //S21
        public string CountDBBOCCS21 { get; set; }
        public string CountDBBOCSS21 { get; set; }
        public string CountDBBOClientS21 { get; set; }
        public string CountDBBOUserS21 { get; set; }

        //OUCH
        public string CountDBBOCCOUCH { get; set; }
        public string CountDBBOCSOUCH { get; set; }
        public string CountDBBOClientOUCH { get; set; }
        public string CountDBBOUserOUCH { get; set; }



        public DBBOModel()
        {
            DataType = ColumnType.DBBOModelType;
        }
    }
}

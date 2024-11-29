using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA_Send_API.Logger;

namespace WA_Send_API.Log
{
    public class LogError : LogBase
    {
        public string Message { get; set; }
        public string Stacktrace { get; set; }

        public LogError()
        {
            DataType = LogType.ERROR;
        }
    }
}

using System;
using WA_Send_API.Logger;

namespace WA_Send_API.Log
{
    public class LogInfo : LogBase
    {
        public string Message { get; set; }

        public LogInfo()
        {
            DataType = LogType.INFO;
        }
    }
}

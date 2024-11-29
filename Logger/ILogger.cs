using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WA_Send_API.Logger
{
    public interface ILogger
    {
        void Log(DateTime time, LogType type, string message);
        void LogException(DateTime time, LogType type, string message, string stacktrace);
    }
}

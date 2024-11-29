using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA_Send_API.Log;

namespace WA_Send_API.Logger
{
    public abstract class LoggerBase : ILogger
    {
        protected object _syncRoot = new object();
        protected StringBuilder _logInformation;

        protected void CleanupResources()
        {
            lock (_syncRoot)
            {
                _logInformation.Remove(0, _logInformation.Length);
                //_logInformation = null;
            }
        }

        #region ILogger Members

        public virtual void Log(DateTime time, LogType type, string message)
        {
            lock (_syncRoot)
            {
                _logInformation = new StringBuilder();
                _logInformation.Append(time.ToString("dd/MM/yyyy HH:mm:ss"))
                    .Append(" [").Append(type.ToString()).Append("]")
                    .Append(" ").Append(message);
                _logInformation.AppendLine();
            }
        }

        public virtual void LogException(DateTime time, LogType type, string message, string stacktrace)
        {
            lock (_syncRoot)
            {
                _logInformation = new StringBuilder();
                _logInformation.Append(time.ToString("dd/MM/yyyy HH:mm:ss"))
                    .Append(" [").Append(type.ToString()).Append("]")
                    .Append(" ").Append(message)
                    .Append(" ").Append(stacktrace);
                _logInformation.AppendLine();
            }
        }
        #endregion
    }
}


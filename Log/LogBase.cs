using System;

using System.Reflection;
using System.Text;
using WA_Send_API.Logger;


namespace WA_Send_API.Log
{
    public abstract class LogBase
    {
        public LogType DataType { get; protected set; }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            PropertyInfo[] pi = GetType().GetProperties();
            foreach (PropertyInfo p in pi)
                s.Append(p.Name).Append(":").Append(p.GetValue(this, null)).Append("|");

            return s.ToString();
        }
    }
}

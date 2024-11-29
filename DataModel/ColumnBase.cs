using System.Reflection;
using System.Text;


namespace WA_Send_API.DataModel
{
    public abstract class ColumnBase
    {
        public ColumnType DataType { get; protected set; }
        public long Timestamp { get; set; }

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

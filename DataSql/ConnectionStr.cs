using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WA_Send_API.DataSql
{
    public class ConnectionStr
    {
        private const string connectionString = "Data Source=10.1.5.162;Initial Catalog=S21Plus_CP;USER ID=s21+;Password=diehards21+;MultipleActiveResultSets=true;";
        private const string connectionString2 = @"DSN=tibero_trusRT;UID=CP_REMOTE;PWD=CP_REMOTE;";
        public static bool ProbeConnectionString()
        {
            bool result = false;
            SqlConnectionStringBuilder csBuilder = new SqlConnectionStringBuilder(connectionString) { ConnectTimeout = 3 };
            OdbcConnectionStringBuilder odbcBuilder = new OdbcConnectionStringBuilder(connectionString2) ;
            SqlConnection connection = new SqlConnection(csBuilder.ToString());
            OdbcConnection connection2 = new OdbcConnection(odbcBuilder.ToString());
            try
            {
                connection.Open();
                connection2.Open();
                result = true;
            }
            catch { result = false; }
            finally 
            { 
                connection.Dispose();
                connection2.Dispose();
            }

            return result;
        }
    }
}

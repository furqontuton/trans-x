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
        private const string connectionString = "Data Source=10.1.2.100;Initial Catalog=S21Plus_CP;USER ID=s21+;Password=diehards21+;MultipleActiveResultSets=true;";
        private const string connectionString2 = @"DSN=tibero_trusRT;UID=CP_REMOTE;PWD=CP_REMOTE;";
        private const string connectionString3 = @"DSN=tibero_trusRTOUCH;UID=CP_REMOTE;PWD=cpremote123prod;";

        public static bool ProbeConnectionString()
        {
            bool result = false;
            SqlConnectionStringBuilder csBuilder = new SqlConnectionStringBuilder(connectionString) { ConnectTimeout = 3 };
            OdbcConnectionStringBuilder odbcBuilder = new OdbcConnectionStringBuilder(connectionString2) ;
            OdbcConnectionStringBuilder odbcBuilder2 = new OdbcConnectionStringBuilder(connectionString3);
            SqlConnection connection = new SqlConnection(csBuilder.ToString());
            OdbcConnection connection2 = new OdbcConnection(odbcBuilder.ToString());
            OdbcConnection connection3 = new OdbcConnection(odbcBuilder2.ToString());
            try
            {
                connection.Open();
                connection2.Open();
                connection3.Open();
                result = true;
            }
            catch { result = false; }
            finally 
            { 
                connection.Dispose();
                connection2.Dispose();
                connection3.Dispose();
            }

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Data.Odbc;

namespace WA_Send_API.DataSql
{
    public class SqlWrapper : IDisposable
    {
        private string _connectionString, _connectionStringTibero, _connectionStringDBBridge, _connectionStringDBS21, _connectionStringOUCH;
        private SqlConnection _connection;
        private OdbcConnection _connection2, _connection3, _connection4, _connection5;
        private SqlCommand _command;
        private OdbcCommand _command2, _command3, _command4, _command5;
        private const string connectionString = "Data Source=10.1.2.100;Initial Catalog=S21Plus_CP;USER ID=s21+;Password=diehards21+;MultipleActiveResultSets=true;";
        private const string connectionStringTibero = @"DSN=tibero_trusRT;UID=CP_REMOTE;PWD=CP_REMOTE;";
        private const string connectionStringDBBridge = @"DSN=tibero_trus;UID=S21_RT_PLUS_CP;PWD=S21_RT_PLUS_CP;";
        private const string connectionStringDBS21 = @"DSN=tibero_prod;UID=S21_RT_PLUS_CP;PWD=S21_RT_PLUS_CP;";
        private const string connectionStringOUCH = @"DSN=tibero_trusRTOUCH;UID=CP_REMOTE;PWD=cpremote123prod;";
        
        public SqlWrapper()
        {
            this._connectionString = connectionString;
            this._connection = new SqlConnection(connectionString);

            this._connectionStringTibero = connectionStringTibero;
            this._connection2 = new OdbcConnection(connectionStringTibero);

            this._connectionStringDBBridge = connectionStringDBBridge;
            this._connection3 = new OdbcConnection(connectionStringDBBridge);

            this._connectionStringDBS21 = connectionStringDBS21;
            this._connection4 = new OdbcConnection(connectionStringDBS21);

            this._connectionStringOUCH = connectionStringOUCH;
            this._connection5 = new OdbcConnection(this._connectionStringOUCH);
        }

        public void AddParameter(string name, DbType type, object value)
        {
            AddParameter(name, type, value, ParameterDirection.Input);
        }

        public void AddParameter(string name, DbType type, object value, ParameterDirection direction)
        {
            if (this._command == null)
                throw new NullReferenceException("SqlCommand is null.");

            SqlParameter param = new SqlParameter();
            param.ParameterName = name;
            param.DbType = type;
            param.Value = value;
            param.Direction = direction;
            
            this._command.Parameters.Add(param);
        }

        public void AddParameterTibero(string name, DbType type, object value, ParameterDirection direction)
        {
            if (this._command2 == null)
                throw new NullReferenceException("OdbcCommand is null.");

            OdbcParameter param = new OdbcParameter();
            param.ParameterName = name;
            param.DbType = type;
            param.Value = value;
            param.Direction = direction;

            this._command.Parameters.Add(param);
        }

        public void AddParameterOUCH(string name, DbType type, object value, ParameterDirection direction)
        {
            if (this._command5 == null)
                throw new NullReferenceException("OdbcCommand is null.");
            OdbcParameter param = new OdbcParameter();
            param.ParameterName = name;
            param.DbType = type;
            param.Value = value;
            param.Direction = direction;

            this._command.Parameters.Add(param);
        }

        public void PrepareSqlStatement(string sqlStatement)
        {
            if (string.IsNullOrEmpty(sqlStatement))
                throw new ArgumentNullException("sqlStatement is null.");
            if (this._connection == null)
                this._connection = new SqlConnection(this._connectionString);

            this._command = new SqlCommand(sqlStatement, this._connection);
            this._command.CommandType = CommandType.Text;
            this._command.CommandTimeout = 3;
        }

        public void PrepareOdbcStatementTibero(string odbcStatement)
        {
            if (string.IsNullOrEmpty(odbcStatement))
                throw new ArgumentNullException("OdbcStatement is null.");
            if (this._connection2 == null)
                this._connection2 = new OdbcConnection(this._connectionStringTibero);

            this._command2 = new OdbcCommand(odbcStatement, this._connection2);
            this._command2.CommandType = CommandType.Text;
            this._command2.CommandTimeout = 3;
        }

        public void PrepareOdbcStatementDBBridge(string odbcStatement)
        {
            if (string.IsNullOrEmpty(odbcStatement))
                throw new ArgumentNullException("OdbcStatement is null.");
            if (this._connection3 == null)
                this._connection3 = new OdbcConnection(this._connectionStringDBBridge);

            this._command3 = new OdbcCommand(odbcStatement, this._connection3);
            this._command3.CommandType = CommandType.Text;
            this._command3.CommandTimeout = 3;

        }

        public void PrepareOdbcStatementDBFO(string odbcStatement)
        {
            if (string.IsNullOrEmpty(odbcStatement))
                throw new ArgumentNullException("OdbcStatement is null.");
            if (this._connection4 == null)
                this._connection4 = new OdbcConnection(this._connectionStringDBS21);

            this._command4 = new OdbcCommand(odbcStatement, this._connection4);
            this._command4.CommandType = CommandType.Text;
            this._command4.CommandTimeout = 3;

        }

        //public void PrepareOdbcStatementDBOUCH(string odbcStatement)
        //{
        //    if (string.IsNullOrEmpty(odbcStatement))
        //        throw new ArgumentNullException("OdbcStatementOuch is null.");
        //    if (this._connection5 == null)
        //    {
        //        Console.WriteLine("_connection5 is null");
        //        this._connection5 = new OdbcConnection(this._connectionStringOUCH);
        //    }
        //
        //    this._command5 = new OdbcCommand(odbcStatement, this._connection5);
        //    this._command5.CommandType = CommandType.Text;
        //    this._command5.CommandTimeout = 3;
        //}

        public void PrepareOdbcStatementDBOUCH(string odbcStatement)
        {
            Console.WriteLine("PrepareOdbcStatementDBOUCH");
            try
            {
                Console.WriteLine("sebelum if isnull");
                if (string.IsNullOrEmpty(odbcStatement))
                    throw new ArgumentNullException("OdbcStatementOuch is null.");

                Console.WriteLine("sebelum if connection5 isnull");
                if (this._connection5 == null)
                {
                    Console.WriteLine("_connection5 is null");
                    this._connection5 = new OdbcConnection(this._connectionStringOUCH);
                }

                Console.WriteLine("sebelum _command5 new ");
                this._command5 = new OdbcCommand(odbcStatement, this._connection5);

                Console.WriteLine("sebelum _command5 CommandType ");
                this._command5.CommandType = CommandType.Text;

                Console.WriteLine("sebelum _command5 CommandTimeout ");
                this._command5.CommandTimeout = 3;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error preparing ODBC statement: " + ex.Message);
                throw; // Re-throw the exception after logging
            }
        }


        public void PrepareStoredProcedure(string procedureName, int commandTimeout)
        {
            if (string.IsNullOrEmpty(procedureName))
                throw new ArgumentNullException("procedureName is null.");
            if (this._connection == null)
                this._connection = new SqlConnection(this._connectionString);

            this._command = new SqlCommand(procedureName, this._connection);
            this._command.CommandType = CommandType.StoredProcedure;
            this._command.CommandTimeout = commandTimeout;
        }

        protected bool GetBooleanValue(string value)
        {
            if (value.Trim().Equals("0") || value.ToLowerInvariant().Trim().Equals("false"))
                return false;
            return true;
        }

        public SqlDataReader ExecuteReader()
        {
            if (this._connection.State == ConnectionState.Open)
                this._connection.Close();

            this._connection.Open();
            return this._command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public OdbcDataReader ExecuteReaderODBC()
        {
            if (this._connection2.State == ConnectionState.Open)
                this._connection2.Close();

            this._connection2.Open();
            return this._command2.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public OdbcDataReader ExecuteReaderODBCBridge()
        {
            if (this._connection3.State == ConnectionState.Open)
                this._connection3.Close();

            this._connection3.Open();
            return this._command3.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public OdbcDataReader ExecuteReaderODBCDBFO()
        {
            if (this._connection4.State == ConnectionState.Open)
                this._connection4.Close();

            this._connection4.Open();
            return this._command4.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public OdbcDataReader ExecuteReaderODBCOUCH()
        {
            try
            {
                if (this._connection5 == null)
                {
                    throw new InvalidOperationException("Connection object (_connection5) is not initialized.");
                }

                if (this._command5 == null)
                {
                    throw new InvalidOperationException("Command object (_command5) is not initialized.");
                }

                // Log the connection state before opening
                
                if (this._connection5.State == ConnectionState.Open)
                {
                    this._connection5.Close();
                }

                this._connection5.Open();

                // Return the reader
                return this._command5.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (OdbcException ex)
            {
                throw;
            }
            catch (InvalidOperationException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public int ExecuteNonQuery()
        {
            if (this._connection.State == ConnectionState.Open)
                this._connection.Close();

            this._connection.Open();
            return this._command.ExecuteNonQuery();
        }

        public object ExecuteScalar()
        {
            if (this._connection.State == ConnectionState.Open)
                this._connection.Close();

            this._connection.Open();
            return this._command.ExecuteScalar();
        }

        public object ExecuteScalarODBC()
        {
            if (this._connection2.State == ConnectionState.Open)
                this._connection2.Close();

            this._connection2.Open();
            return this._command2.ExecuteScalar();
        }

        public SqlDataAdapter ExecuteAdapter()
        {
            if (this._connection.State == ConnectionState.Open)
                this._connection.Close();

            this._connection.Open();
            return new SqlDataAdapter(this._command);
        }

        public void ClearPool()
        {
            SqlConnection.ClearPool(this._connection);
        }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                if (this._connection != null)
                    this._connection.Close();
            }
            catch { }
            finally
            {
                this._command = null;
                this._connection = null;
            }
        }

        #endregion

    }
}
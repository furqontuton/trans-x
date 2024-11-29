using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using System.Data.Odbc;
//using System.Data.SqlClient;
using Oracle.ManagedDataAccess;
using System.Xml.Schema;
using System.Text.RegularExpressions;
using System.Timers;
using System.Net.Security;
using System.IO;
using System.Data.SqlTypes;
using System.Xml;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using WA_Send_API.DataModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Security.Policy;
using WA_Send_API.Function;
using WA_Send_API.DataSql;
using WA_Send_API.UI;


namespace WA_Send_API
{
    public partial class Main : Form
    {
        private int _sortColumn;
        private bool _isUiRunning, _isDbConnected,_isProcessingSort;
        private FunctionContext _context;
        public SqlWrapper _CCSqlReader, _fioSqlReader;
        protected TimeStampBuffer _CCtimeStampBuffer;
     

        public struct TimeStampBuffer
        {
            public long CCTimestamp;
        }
        private bool IsDbConnected
        {
            get { return this._isDbConnected; }
            set
            {
                this._isDbConnected = value;
            }
        }
        
        public FunctionContext FunctionContext
        {
            get { return this._context; }
            set { this._context = value; }
        }

        
        //private SqlConnection conn3 = new SqlConnection(connection_stringdsn3);

        public Main()
        {
            InitializeComponent();
            this._isUiRunning = true;
            
        }

        public ListView ListViewStatus
        {
            get { return this.LvwList; }
        }
        private void ConnectionStatus()
        {
            new Thread(() =>
            {
                while (this._isUiRunning)
                {
                    this.IsDbConnected = ConnectionStr.ProbeConnectionString();
                    Thread.Sleep(5000);
                }
            }).Start();

        }
        private void SortStatusLog(object column)
        {
            if (this.InvokeRequired)
                this.Invoke(new SortStatusLogHandler(SortStatusLog), column);
            else
            {
                this._isProcessingSort = true;
                if ((int)column != this._sortColumn)
                {
                    this._sortColumn = (int)column;
                    this.LvwList.Sorting = SortOrder.Ascending;
                }
                else
                {
                    if (this.LvwList.Sorting == SortOrder.Ascending)
                        this.LvwList.Sorting = SortOrder.Descending;
                    else
                        this.LvwList.Sorting = SortOrder.Ascending;
                }

                this.LvwList.ListViewItemSorter = new ListViewItemComparer(this._sortColumn, this.LvwList.Sorting);
                this.LvwList.Sort();
                this._isProcessingSort = false;
            }
        }

       

        private void Main_Load(object sender, EventArgs e)
        {
            
            new Thread(() => { ConnectionStatus(); }).Start();
           
        }

        public ToolStripStatusLabel labelQueue
        {
            get { return this.toolStripSttsLblQueue; }
        }

        public ToolStrip toolStrip
        {
            get { return this.statusStrip1; }
        }

        /*private void testSendDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._context.GetDBCompare();
        }
        */

       
        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.Shutdown();
            this._isUiRunning = false;
        }

        private void LvwList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (!this._isProcessingSort)
                new Thread(SortStatusLog).Start(e.Column);
        }

        public void aLLCountDataToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        
        
    }

}

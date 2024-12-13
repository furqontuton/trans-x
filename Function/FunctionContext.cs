using System;

using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using WA_Send_API.DataModel;
using WA_Send_API.DataSql;
using WA_Send_API.Log;
using WA_Send_API.Logger;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using RestSharp;
using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using System.Web.UI;
using System.Runtime.InteropServices;
using System.Data.Odbc;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Collections;
using System.Threading;

namespace WA_Send_API.Function
{
    public class FunctionContext
    {
        protected ILogger[] _logger;
        protected TimeStampBuffer _CCtimeStampBuffer;
        protected DateTime _currentDateTime;

        private SqlWrapper _fioSqlReader, _dbCompareSqlReader, _orderStatusPreopODBCReader, _orderStatusOpenODBCReader, _clientShortODBCReader, _getOrderODBCReader;

        protected Queue<ColumnBase> _fioDataQueue, _fioDataFailedQueue;
        protected Queue<LogBase> _logQueue;

        protected FIOQueueUILogger _fiolblQueue;

        protected int _timeCheck;
        protected long QueueCount, FioCount;
        protected bool _Rejected, _Approved;
        protected object _timestampLock = new object();
        protected string _setInOut, _querySetPreop, _querySetOpen, _querySetShort, _querySetOrder, _querySyncData, _querySetDBCompare, _formattedDateTime, _mobileStr;
        protected string _ClientID, _Amount, _InOut, _FioNID, _ClientName, _ApproveTime, _RejectTime, _ClientIDShort, _StockIDShort, _TotalShort, _TotalOrder, _CountCC, _CountClient, _CountStock;
        protected string _OrderStatusPreop_Open, _OrderStatusPreop_Reject, _OrderstatusOpening_Open, _OrderstatusOpening_Reject;
        protected string _dbBOCountCCTrus, _dbBOCountCSTrus, _dbBOCountClientTrus, _dbBOCountUserTrus, _dbBridgeCountCC, _dbBridgeCountCS, _dbBridgeCountClient, _dbBridgeCountUser, _dbBridgeCountUser2, _dbBridgeCountCC2;
        protected string _dbBOCountCCS21, _dbBOCountCSS21, _dbBOCountClientS21, _dbBOCountUserS21, _dbFOCountCC, _dbFOCountCS, _dbFOCountClient, _dbFOCountUser;


        private SqlWrapper _dbCompareSqlReaderOuch, _orderStatusPreopODBCReaderOuch, _orderStatusOpenODBCReaderOuch, _clientShortODBCReaderOuch, _getOrderODBCReaderOuch;
        //fatta 2024/12/09 //Added OUCH 
        protected string _OrderStatusPreop_Open_OUCH, _OrderStatusPreop_Reject_OUCH, _OrderstatusOpening_Open_OUCH, _OrderstatusOpening_Reject_OUCH;
        //fatta 2024/12/03 //Added OUCH 
        protected string _dbBOCountCCOUCH, _dbBOCountCSOUCH, _dbBOCountClientOUCH, _dbBOCountUserOUCH, _dbBridgeCountCCOUCH, _dbBridgeCountCSOUCH, _dbBridgeCountClientOUCH, _dbBridgeCountUserOUCH;
        //fatta 2024/12/09 //Added OUCH 
        protected string _ClientIDShortOUCH, _StockIDShortOUCH, _TotalShortOUCH, _TotalOrderOUCH;

        double statusCheckPre, statusCheckPreOUCH;
        int statusCheck, statusCheckOUCH;

        public FunctionContext()
             : this(null)
        {
        }

        public FunctionContext(FIOQueueUILogger labelQueue, params ILogger[] logger)
        {
            Initialize();
            if (logger != null)
            {
                _logger = new ILogger[logger.Length];
                for (int i = 0; i < logger.Length; i++)
                    _logger[i] = logger[i];
            }
            if (labelQueue != null)
            {
                _fiolblQueue = labelQueue;
            }

        }
        public struct TimeStampBuffer
        {
            public long CCTimestamp;
        }
        public void InitFIOSqlWrapper()
        {
            _fioSqlReader = new SqlWrapper();
            _orderStatusPreopODBCReader = new SqlWrapper();
            _orderStatusOpenODBCReader = new SqlWrapper();
            _clientShortODBCReader = new SqlWrapper();
            _getOrderODBCReader = new SqlWrapper();
            _dbCompareSqlReader = new SqlWrapper();
        }

        //GetTimeStamp Function
        public void GetDBTimestamp()
        {

            lock (_fioSqlReader)
            {
                try
                {
                    _fioSqlReader.PrepareSqlStatement("SELECT b.ClientName, b.ClientID as clientidinout, b.mobilephone, a.amount, a.InOut, a.Fundinoutnid, a.checked, a.approved, a.approvetime, a.rejected, a.rejecttime, CAST(a.TimeStamp AS BIGINT) AS Timestamp from FundInOut a left join client b on b.clientnid = a.ClientNID WHERE CAST(a.TimeStamp AS BIGINT) > @Timestamp and a.checked = 1 and a.approved = 1\r\nunion all\r\nselect b.ClientName, b.ClientID as clientidinout, b.mobilephone, a.amount, a.InOut, a.Fundinoutnid, a.checked, a.approved, a.approvetime,a.rejected, a.rejecttime, CAST(a.TimeStamp AS BIGINT) AS Timestamp from FundInOut a left join client b on b.clientnid = a.ClientNID WHERE CAST(a.TimeStamp AS BIGINT) > @Timestamp and a.Rejected = 1 order by timestamp asc;");
                    _fioSqlReader.AddParameter("@Timestamp", DbType.Int64, _CCtimeStampBuffer.CCTimestamp);
                    SqlDataReader r = _fioSqlReader.ExecuteReader();

                    if (r.HasRows)

                        AddLog(LogType.INFO, "Executing Select Data Fundinout");

                    while (r.Read())
                    {

                        FundInOutModel o = new FundInOutModel();
                        o.ClientID = r["clientidinout"].ToString();
                        o.Amount = double.Parse(r["amount"].ToString());
                        o.InOut = r["InOut"].ToString();
                        o.Fundinoutnid = int.Parse(r["Fundinoutnid"].ToString());
                        o.ClientName = r["ClientName"].ToString();
                        _CCtimeStampBuffer.CCTimestamp = long.Parse(r["TimeStamp"].ToString());
                        o.MobilePhone = r["mobilephone"].ToString();
                        o.ApproveTime = r["approvetime"].ToString();
                        o.RejectTime = r["rejecttime"].ToString();
                        o.Rejected = GetBooleanValue(r["rejected"].ToString());
                        o.Approved = GetBooleanValue(r["approved"].ToString());

                        //MessageBox.Show(o.Rejected);

                        string pattern = "[^a-zA-Z0-9 ]";



                        _mobileStr = Regex.Replace(o.MobilePhone, pattern, "");

                        QueueFundInOutData(o);
                        System.Threading.Interlocked.Increment(ref QueueCount);

                        AddLog(LogType.INFO, "--> Send To WA: " + o.DataType.ToString() + " | ClientID: " + o.ClientID + " | ClientName: " + o.ClientName + " | Amount: " + o.Amount.ToString() + " | InOut: " + o.InOut + " | FundInOutID: " + o.Fundinoutnid + " | MobilePhone: " + _mobileStr);

                        _ClientID = o.ClientID;
                        _Amount = o.Amount.ToString();
                        _InOut = o.InOut;
                        _FioNID = o.Fundinoutnid.ToString();
                        _ClientName = o.ClientName;
                        _ApproveTime = o.ApproveTime;
                        _RejectTime = o.RejectTime;
                        _Rejected = o.Rejected;
                        _Approved = o.Approved;

                        //MessageBox.Show(_Rejected);

                        if (o != null)
                        {
                            //MessageBox.Show(_ClientID);
                            //createApi();

                            //GetApi();
                        }


                    }

                    this._fiolblQueue.addQueue(QueueCount);
                    //await _restHelper.Post("hsfvXBi91oPj2QHMuY8I", "6285174377897", "120363195609109582", _querySet.ToString());

                }



                catch (Exception e)
                {
                    AddLog(LogType.ERROR, e.Message, e.StackTrace);
                }

            }

        }

        //DB BO Compared
        public void GetDBCompare()
        {
            lock (_dbCompareSqlReader)
            {
                try
                {
                    _dbCompareSqlReader.PrepareStoredProcedure("KB_Notif_GetDBBO", 0);
                    SqlDataReader r = _dbCompareSqlReader.ExecuteReader();

                    if (r.HasRows)
                        AddLog(LogType.INFO, "Executing Select Data DB Back Office");

                    while (r.Read())
                    {
                        DBBOModel o = new DBBOModel();
                        o.CountDBBOCCTrus = r["CountCCTrus"].ToString();
                        o.CountDBBOCSTrus = r["CountCSTrus"].ToString();
                        o.CountDBBOClientTrus = r["CountClientTrus"].ToString();
                        o.CountDBBOUserTrus = r["CountUserTrus"].ToString();
                        o.CountDBBOCCS21 = r["CountCCS21"].ToString();
                        o.CountDBBOCSS21 = r["CountCSS21"].ToString();
                        o.CountDBBOClientS21 = r["CountClientS21"].ToString();
                        o.CountDBBOUserS21 = r["CountUserS21"].ToString();

                        QueueFundInOutData(o);
                        System.Threading.Interlocked.Increment(ref QueueCount);

                        AddLog(LogType.INFO, "--> Send To WA Unofficial DBBO Trus: ClientCash Count: " + o.CountDBBOCCTrus + " | ClientStock Count: " + o.CountDBBOCSTrus + " | Client Count: " + o.CountDBBOClientTrus + " | User Count: " + o.CountDBBOUserTrus);
                        AddLog(LogType.INFO, "--> Send To WA Unofficial DBBO S21: ClientCash Count: " + o.CountDBBOCCS21 + " | ClientStock Count: " + o.CountDBBOCSS21 + " | Client Count: " + o.CountDBBOClientS21 + " | User Count: " + o.CountDBBOUserS21);

                        _dbBOCountCCTrus = o.CountDBBOCCTrus;
                        _dbBOCountCSTrus = o.CountDBBOCSTrus;
                        _dbBOCountClientTrus = o.CountDBBOClientTrus;
                        _dbBOCountUserTrus = o.CountDBBOUserTrus;
                        _dbBOCountCCS21 = o.CountDBBOCCS21;
                        _dbBOCountCSS21 = o.CountDBBOCSS21;
                        _dbBOCountClientS21 = o.CountDBBOClientS21;
                        _dbBOCountUserS21 = o.CountDBBOUserS21;

                        if (o != null)
                        {
                            GetDBBridge();
                            GetDBS21();
                            GetDBBridgeOuch();
                            GetApiDbCompare();
                        }
                        else
                        {
                            MessageBox.Show("Error");
                        }
                    }
                }
                catch (Exception e)
                {
                    AddLog(LogType.ERROR, e.Message, e.StackTrace);
                }
            }
        }


        public void GetDBBridge()
        {
            lock (_dbCompareSqlReader)
            {
                try
                {
                    _dbCompareSqlReader.PrepareOdbcStatementDBBridge("select (SELECT COUNT(clientid) CLIENTCASH from CLIENT_CASH where to_char(Date_,'yyyymmdd') = to_char(sysdate,'yyyymmdd')) CountClientcash,  (SELECT COUNT(stockid) CLIENTSTOCK from CLIENT_STOCK where to_char(Date_,'yyyymmdd') = to_char(sysdate,'yyyymmdd')) countClientStock,  (SELECT COUNT(clientid) TOTALCLIENT from Client) countclient, (SELECT COUNT(userid) TOTALUSER from USER_) countuser, (SELECT * from COUNT_CLIENT ) CountUserOuch, (select count(clientid) from(select a.clientid, a.clientname from client a left join client_cash b on a.clientid = b.clientid)) CountCashUserOuch from dual");
                    OdbcDataReader r = _dbCompareSqlReader.ExecuteReaderODBCBridge();


                    if (r.HasRows)
                        AddLog(LogType.INFO, "Executing Select Data Dount DBBridge");

                    while (r.Read())
                    {
                        DBBridgeModel o = new DBBridgeModel();
                        o.CountDBBridgeCC = r["CountClientCash"].ToString();
                        o.CountDBBridgeCS = r["CountClientStock"].ToString();
                        o.CountDBBridgeClient = r["CountClient"].ToString();
                        o.CountDBBridgeUser = r["CountUser"].ToString();
                        o.CountDBBridgeUser2 = r["CountUserOuch"].ToString();
                        o.CountDBBridgeCC2 = r["CountCashUserOuch"].ToString();

                        QueueFundInOutData(o);
                        System.Threading.Interlocked.Increment(ref QueueCount);

                        AddLog(LogType.INFO, "--> Send To WA Unofficial DBBridge: ClientCash Count: " + o.CountDBBridgeCC + " | ClientStock Count: " + o.CountDBBridgeCS + " | Client Count: " + o.CountDBBridgeClient + " | User Count: " + o.CountDBBridgeUser);

                        _dbBridgeCountCC = o.CountDBBridgeCC.ToString();
                        _dbBridgeCountCS = o.CountDBBridgeCS.ToString();
                        _dbBridgeCountClient = o.CountDBBridgeClient.ToString();
                        _dbBridgeCountUser = o.CountDBBridgeUser.ToString();
                        _dbBridgeCountUser2 = o.CountDBBridgeUser2.ToString();
                        _dbBridgeCountCC2 = o.CountDBBridgeCC2.ToString();
                    }
                }
                catch (Exception e)
                {
                    AddLog(LogType.ERROR, e.Message, e.StackTrace);
                }

            }
        }

        public void GetDBS21()
        {
            lock (_dbCompareSqlReader)
            {
                try
                {
                    _dbCompareSqlReader.PrepareOdbcStatementDBFO("select (SELECT COUNT(stockid) CLIENTSTOCK from CLIENT_STOCK where to_char(Date_,'yyyymmdd') = to_char(sysdate,'yyyymmdd')) countClientStock,(SELECT COUNT(clientid) CLIENTCASH from CLIENT_CASH where to_char(Date_,'yyyymmdd') = to_char(sysdate,'yyyymmdd')) CountClientcash,(SELECT COUNT(clientid) TOTALCLIENT from Client) countclient,(SELECT COUNT(userid) TOTALUSER from USER_) countuser from dual;");
                    OdbcDataReader r = _dbCompareSqlReader.ExecuteReaderODBCDBFO();

                    if (r.HasRows)
                        AddLog(LogType.INFO, "Executing Select Sync Data ");

                    while (r.Read())
                    {
                        DBFOModel o = new DBFOModel();
                        o.CountDBFOCC = r["CountClientCash"].ToString();
                        o.CountDBFOCS = r["CountClientStock"].ToString();
                        o.CountDBFOClient = r["CountClient"].ToString();
                        o.CountDBFOUser = r["CountUser"].ToString();

                        //MessageBox.Show(o.Orderstatus);

                        QueueFundInOutData(o);
                        System.Threading.Interlocked.Increment(ref QueueCount);

                        AddLog(LogType.INFO, "--> Send To WA Unofficial DBFO : Client Cash : " + o.CountDBFOCC + " Client Stock : " + o.CountDBFOCS + " Client : " + o.CountDBFOClient + " User : " + o.CountDBFOUser);

                        _dbFOCountCC = o.CountDBFOCC;
                        _dbFOCountCS = o.CountDBFOCS;
                        _dbFOCountClient = o.CountDBFOClient;
                        _dbFOCountUser = o.CountDBFOUser;
                    }
                }
                catch (Exception e)
                {
                    AddLog(LogType.ERROR, e.Message, e.StackTrace);
                }
            }
        }


        //All TIBERO Request
        public void GetOrderStatusPreop()
        {
            lock (_orderStatusPreopODBCReader)
            { 
                try
                {
                    _orderStatusPreopODBCReader.PrepareOdbcStatementTibero("select (SELECT COUNT(orderstatus) FROM TEXCHANGEMARKETORDER where orderstatus = 0 and to_char(stime, 'hh24:MI:ss') >= '08:45:00') Orderstatus_Preop_Open,  (SELECT COUNT(orderstatus)   FROM TEXCHANGEMARKETORDER where orderstatus = 8 and to_char(stime, 'hh24:MI:ss') >= '08:45:00') Orderstatus_Preop_Reject from dual");
                    OdbcDataReader r = _orderStatusPreopODBCReader.ExecuteReaderODBC();

                    if (r.HasRows)
                        AddLog(LogType.INFO, "Executing Select Data OrderStatusOpen");

                    while (r.Read())
                    {
                        OrderCheckModel o = new OrderCheckModel();
                        o.Orderstatus_Preop_Open = r["Orderstatus_Preop_Open"].ToString();
                        o.Orderstatus_Preop_Reject = r["Orderstatus_Preop_Reject"].ToString();
                        //MessageBox.Show(o.Orderstatus);

                        QueueFundInOutData(o);
                        System.Threading.Interlocked.Increment(ref QueueCount);

                        AddLog(LogType.INFO, "--> Send To WA Open Status Count: " + o.Orderstatus_Preop_Open + " Rows Open | " + o.Orderstatus_Preop_Reject + " Rows Rejected");

                        _OrderStatusPreop_Open = o.Orderstatus_Preop_Open.ToString();
                        _OrderStatusPreop_Reject = o.Orderstatus_Preop_Reject.ToString();

                        if (o != null)
                        {
                            statusCheckPre = double.Parse(_OrderStatusPreop_Open);
                        }
                        else
                        {
                            MessageBox.Show("Error");
                        }
                    }
                }
                catch (Exception e)
                {
                    AddLog(LogType.ERROR, e.Message, e.StackTrace);
                }

            }
            
        }

        public void GetOrderStatusOpen()
        {
            lock(_orderStatusOpenODBCReader)
            { 
                try
                {
                    _orderStatusOpenODBCReader.PrepareOdbcStatementTibero("select (SELECT COUNT(orderstatus)  FROM TEXCHANGEMARKETORDER where orderstatus is null and to_char(stime, 'hh24:MI:ss') >= '09:00:00')  Orderstatus_Opening_Open,  (SELECT COUNT(orderstatus)  FROM TEXCHANGEMARKETORDER where orderstatus = 8 and to_char(stime, 'hh24:MI:ss') >= '09:00:00')  Orderstatus_Opening_Reject from dual");
                    OdbcDataReader r = _orderStatusOpenODBCReader.ExecuteReaderODBC();

                    if (r.HasRows)
                        AddLog(LogType.INFO, "Executing Select Data OrderStatusOpen");

                    while (r.Read())
                    {
                        OrderCheckModel o = new OrderCheckModel();
                        o.Orderstatus_Opening_Open = r["Orderstatus_Opening_Open"].ToString();
                        o.Orderstatus_Opening_Reject = r["Orderstatus_Opening_Reject"].ToString();
                        //MessageBox.Show(o.Orderstatus_Opening_Open + " - " + o.Orderstatus_Opening_Reject);

                        QueueFundInOutData(o);
                        System.Threading.Interlocked.Increment(ref QueueCount);

                        AddLog(LogType.INFO, "--> Send To WA Basket Status Count: " + o.Orderstatus_Opening_Open + " Rows Open | " + o.Orderstatus_Opening_Reject + " Rows Rejected");

                        _OrderstatusOpening_Open = o.Orderstatus_Opening_Open.ToString();
                        _OrderstatusOpening_Reject = o.Orderstatus_Opening_Reject.ToString();

                        if (o != null)
                        {
                            statusCheck = int.Parse(_OrderstatusOpening_Open);
                            //GetApiOpen();
                        }
                        else
                        {
                            MessageBox.Show("Error GetApiOpen");
                        }
                    }
                }
                catch (Exception e)
                {
                    AddLog(LogType.ERROR, e.Message, e.StackTrace);
                }
            }
        }

        public void GetClientShort()
        {
            lock (_clientShortODBCReader)
            {
                try
                {
                    _clientShortODBCReader.PrepareOdbcStatementTibero("select y.custid CUSTID, y.stockcode STOCKCODE, y.ovol, y.rvol, y.tvol, y.ovol+y.tvol-y.rvol short from (select x.custid, x.stockcode, sum(x.volume) ovol, sum(x.working) rvol, sum( x.trade) tvol FROM (select custid, stockcode, volume, 0 working, 0 trade from tcustomerstockbalance UNION ALL SELECT custid, stockcode ,0 volume, case when bs<2 then 0 else 1 end * coalesce(rvolume, ovolume)  working , case when bs<2 then 1 else -1 end * coalesce(tvolume,0) trade from TEXCHANGEMARKETORDER  where coalesce(orderstatus,0)<>5) x group by x.custid, x.stockcode) y where  y.ovol+y.tvol-y.rvol<0");
                    OdbcDataReader r = _clientShortODBCReader.ExecuteReaderODBC();

                    if (r.HasRows)
                        AddLog(LogType.INFO, "Executing Select Data Short Client");
                    else
                        AddLog(LogType.INFO, "No Data Short Client");

                    while (r.Read())
                    {
                        ClientShortModel o = new ClientShortModel();
                        o.CustID = r["CUSTID"].ToString();
                        o.StockCode = r["STOCKCODE"].ToString();
                        o.Short = r["SHORT"].ToString();
                        //MessageBox.Show(o.Orderstatus);

                        QueueFundInOutData(o);
                        System.Threading.Interlocked.Increment(ref QueueCount);

                        AddLog(LogType.INFO, "--> Send To WA Client Short: ClientID = " + o.CustID + " | StockID = " + o.StockCode + " | Short = " + o.Short);

                        _ClientIDShort = o.CustID.ToString();
                        _StockIDShort = o.StockCode.ToString();
                        _TotalShort = o.Short;

                        if (o != null)
                        {
                            GetApiShort();
                        }
                        else
                        {
                            MessageBox.Show("Error GetApiShort");
                        }
                    }
                }
                catch (Exception e)
                {
                    AddLog(LogType.ERROR, e.Message, e.StackTrace);
                }
            }
        }

        public void GetOrderData()
        {
            lock(_getOrderODBCReader)
            { 
                try
                {
                    _getOrderODBCReader.PrepareOdbcStatementTibero("Select count(orderid) Total_Order from texchangemarketorder ");
                    OdbcDataReader r = _getOrderODBCReader.ExecuteReaderODBC();

                    if (r.HasRows)
                        AddLog(LogType.INFO, "Executing Select Data Short Client");
                    else
                        AddLog(LogType.INFO, "No Data Short Client");

                    while (r.Read())
                    {
                        GetOrderModel o = new GetOrderModel();
                        o.Total_Order = r["Total_Order"].ToString();

                        QueueFundInOutData(o);
                        System.Threading.Interlocked.Increment(ref QueueCount);

                        AddLog(LogType.INFO, "--> Send To WA Total Order: " + o.Total_Order);
                        _TotalOrder = o.Total_Order.ToString();

                        if (o != null)
                        {
                            //GetApiOrderCheck();
                        }
                        else
                        {
                            MessageBox.Show("Error GetApiShort");
                        }

                    }

                }
                catch (Exception e)
                {
                    AddLog(LogType.ERROR, e.Message, e.StackTrace);
                }
            }
        }

        //===================================================================================================================================================================//
        //                                                                                   OUCH                                                                            //
        //===================================================================================================================================================================//

        public void GetOrderStatusPreOpOUCH() //pre op for ouch
        {
            lock (_orderStatusPreopODBCReader)
            {
                try
                {
                    _orderStatusPreopODBCReader.PrepareOdbcStatementDBOUCH("select (SELECT COUNT(status)  FROM tborder where status = 'O' and to_char(senttime, 'hh24:MI:ss') >= '08:45:00')  OrderStatus_Preop_Open_OUCH,  (SELECT COUNT(status)  FROM TBORDER where status = 'R' and to_char(senttime, 'hh24:MI:ss') >= '08:45:00')  OrderStatus_Preop_Reject_OUCH from dual");
                    OdbcDataReader r = _orderStatusPreopODBCReader.ExecuteReaderODBCOUCH();

                    if (r.HasRows)
                        AddLog(LogType.INFO, "Executing select data orderstatusopen");

                    while (r.Read())
                    {
                        OrderCheckModel o = new OrderCheckModel();
                        o.Orderstatus_Preop_Open = r["OrderStatus_Preop_Open_OUCH"].ToString();
                        o.Orderstatus_Preop_Reject = r["OrderStatus_Preop_Reject_OUCH"].ToString();

                        QueueFundInOutData(o);
                        System.Threading.Interlocked.Increment(ref QueueCount);

                        AddLog(LogType.INFO, "--> Send To WA Pre Open Status Count: " + o.Orderstatus_Preop_Open + " Rows Open | " + o.Orderstatus_Preop_Reject + " Rows Rejected");

                        _OrderStatusPreop_Open_OUCH = o.Orderstatus_Preop_Open.ToString();
                        _OrderStatusPreop_Reject_OUCH = o.Orderstatus_Preop_Reject.ToString();

                        if (o != null)
                            statusCheckPreOUCH = double.Parse(_OrderStatusPreop_Open_OUCH);
                        else
                            MessageBox.Show("Error");
                    }
                }
                catch (Exception e)
                {
                    AddLog(LogType.ERROR, e.Message, e.StackTrace);
                }
            }
        }

        public void GetOrderStatusOpenOUCH()
        {
            lock (_orderStatusOpenODBCReader)
            {
                try
                {
                    _orderStatusOpenODBCReader.PrepareOdbcStatementDBOUCH("select (SELECT COUNT(status)  FROM TBORDER where status = 'P' and to_char(senttime, 'hh24:MI:ss') >= '09:00:00')  Orderstatus_Open_Ouch,  (SELECT COUNT(status)  FROM tborder where status = 'R' and to_char(senttime, 'hh24:MI:ss') >= '09:00:00')  Orderstatus_Reject_Ouch from dual");
                    OdbcDataReader r = _orderStatusOpenODBCReader.ExecuteReaderODBCOUCH();

                    if (r.HasRows)
                        AddLog(LogType.INFO, "Executing select data orderstatusopen");

                    while (r.Read())
                    {
                        OrderCheckModel o = new OrderCheckModel();
                        o.Orderstatus_Opening_Open = r["OrderStatus_Open_OUCH"].ToString();
                        o.Orderstatus_Opening_Reject = r["OrderStatus_Reject_OUCH"].ToString();

                        QueueFundInOutData(o);
                        System.Threading.Interlocked.Increment(ref QueueCount);

                        AddLog(LogType.INFO, "--> Send To WA Open Status Count: " + o.Orderstatus_Opening_Open + " Rows Open | " + o.Orderstatus_Opening_Reject + " Rows Rejected");

                        _OrderstatusOpening_Open_OUCH = o.Orderstatus_Opening_Open.ToString();
                        _OrderstatusOpening_Reject_OUCH = o.Orderstatus_Opening_Reject.ToString();

                        if (o != null)
                            statusCheckOUCH = int.Parse(_OrderstatusOpening_Open_OUCH);
                        //GetApiOpenOuch();
                        else
                            MessageBox.Show("Error");
                    }

                }
                catch (Exception e)
                {
                    AddLog(LogType.ERROR, e.Message, e.StackTrace);
                }
            }
        }

        public void GetClientShortOUCH()
        {
            lock (_clientShortODBCReader)
            {
                try
                {
                    _clientShortODBCReader.PrepareOdbcStatementDBOUCH("SELECT X.accountcode as CUSTID, X.productcode as STOCKCODE, sum(X.OpenVol) OpenQty, sum(X.tradevolume) TradedQty, sum(NVL(X.WorkingSell,0)) WorkingSell, sum(X.openvol) + sum(X.tradevolume) - sum(NVL(X.WorkingSell,0)) as SHORT FROM ( SELECT accountcode, productcode, NVL(volume,0) as OpenVol, settled , 0 as WorkingSell, 0 as tradevolume FROM customersecurityposition UNION ALL SELECT A.accountcode, A.productcode, 0 as OpenVol, 0 as settled,  sum(A.WorkingSell) as WorkingSell, sum(A.tradevolume) as tradevolume FROM  ( SELECT accountcode, productcode, case when Side not in ('B','M') then NVL(remainvolume,ordervolume) else 0 end WorkingSell, case when Side in ('B','M') then 1 else -1 end * NVL(tradevolume,0) as tradevolume FROM tborder )  A  GROUP BY A.accountcode, A.productcode  ) X GROUP BY X.accountcode, X.productcode HAVING sum(X.openvol)+sum(X.tradevolume) - sum(NVL(X.WorkingSell,0))<0  ");
                    OdbcDataReader r = _clientShortODBCReader.ExecuteReaderODBCOUCH();

                    if (r.HasRows)
                        AddLog(LogType.INFO, "Excuting Select Data Short Client");
                    else
                        AddLog(LogType.INFO, "No Data Short Client");

                    while (r.Read())
                    {
                        ClientShortModel o = new ClientShortModel();
                        o.CustID = r["CUSTID"].ToString();
                        o.StockCode = r["STOCKCODE"].ToString();
                        o.Short = r["SHORT"].ToString();

                        QueueFundInOutData(o);
                        System.Threading.Interlocked.Increment(ref QueueCount);

                        AddLog(LogType.INFO, "--> Send To WA Client Short: ClientID = " + o.CustID + " | StockID = " + o.StockCode + " | Short = " + o.Short);

                        _ClientIDShortOUCH = o.CustID.ToString();
                        _StockIDShortOUCH = o.StockCode.ToString();
                        _TotalShortOUCH = o.Short;

                        if (o != null)
                            GetApiShortOUCH();
                        else
                            MessageBox.Show("Error GetApiShort");
                    }
                }
                catch (Exception e)
                {
                    AddLog(LogType.ERROR, e.Message, e.StackTrace);
                }
            }
        }

        public void GetOrderDataOuch()
        {
            lock (_getOrderODBCReader)
            {
                try
                {
                    _getOrderODBCReader.PrepareOdbcStatementDBOUCH("Select count(exchangeorderid) Total_Order from tborder ");
                    OdbcDataReader r = _getOrderODBCReader.ExecuteReaderODBCOUCH();

                    if (r.HasRows)
                        AddLog(LogType.INFO, "Executing Select Data Short Client");
                    else
                        AddLog(LogType.INFO, "No Data Short Client");

                    while (r.Read())
                    {
                        GetOrderModel o = new GetOrderModel();
                        o.Total_Order = r["Total_Order"].ToString();

                        QueueFundInOutData(o);
                        System.Threading.Interlocked.Increment(ref QueueCount);

                        AddLog(LogType.INFO, "--> Send To WA Total Order: " + o.Total_Order);

                        _TotalOrderOUCH = o.Total_Order.ToString();

                        if (o != null)
                        {
                            // GetApiOrderCheck(); 
                        }
                        else
                            MessageBox.Show("Error GetApiOrder");
                    }
                }
                catch (Exception e)
                {
                    AddLog(LogType.ERROR, e.Message, e.StackTrace);
                }
            }
        }

        public void GetDBBridgeOuch()
        {
            lock (_dbCompareSqlReader)
            {
                try
                {
                    _dbCompareSqlReader.PrepareOdbcStatementDBOUCH("SELECT (SELECT COUNT(*) FROM msaccount) AS CountClientCash, (SELECT COUNT(*) FROM CUSTOMERSECURITYPOSITION) AS CountClientStock, (SELECT COUNT(*) FROM msaccount) AS CountClient, (select count(1) from MSUSER where userid not in ('ARA', 'KOENTO_DEV', 'BAYU_DEV', 'HARTANTO_DEV','IVAN_TRUS', 'REINER_DEV', 'YATNA_DEV', 'RADMIN', 'HARTANTO_TRUS')) as CountUser FROM dual"); //query
                    OdbcDataReader r = _dbCompareSqlReader.ExecuteReaderODBCOUCH();

                    if (r.HasRows) 
                        AddLog(LogType.INFO, "Executing Select Data Count DBBRidge");

                    while (r.Read())
                    {
                        DBBridgeModel o = new DBBridgeModel();
                        o.CountDBBridgeCC = r["CountClientCash"].ToString();
                        o.CountDBBridgeCS = r["CountClientStock"].ToString();
                        o.CountDBBridgeClient = r["CountClient"].ToString();
                        o.CountDBBridgeUser = r["CountUser"].ToString();

                        QueueFundInOutData(o);
                        System.Threading.Interlocked.Increment(ref QueueCount);

                        AddLog(LogType.INFO, "--> Send To WA Unofficial DBBridge OUCH: ClientCash Count: " + o.CountDBBridgeCC + " | ClientStock Count: " + o.CountDBBridgeCS + " | Client Count: " + o.CountDBBridgeClient + " | User Count: " + o.CountDBBridgeUser);

                        _dbBridgeCountCCOUCH = o.CountDBBridgeCC.ToString();
                        _dbBridgeCountCSOUCH = o.CountDBBridgeCS.ToString();
                        _dbBridgeCountClientOUCH = o.CountDBBridgeClient.ToString();
                        _dbBridgeCountUserOUCH = o.CountDBBridgeUser.ToString();
                    }
                }
                catch (Exception e)
                {
                    AddLog(LogType.ERROR, e.Message, e.StackTrace);
                }
            }
        }

        //===================================================================================================================================================================//
        //                                                                   End of OUCH part                                                                                //
        //===================================================================================================================================================================//




        //API Official Function
        private void createApi()
        {
            string JClientname = _ClientName.ToString();
            string JClientID = _ClientID.ToString();
            string JAmount = string.Format(new CultureInfo("id-ID"), "{0:C0}", Convert.ToDouble(_Amount));

            string JFioNID = _FioNID.ToString();
            string JreceiveInfo = string.Empty;
            string greetingStr = string.Empty;
            string JInOut = string.Empty;
            string JsonQString = string.Empty;
            string JsonLString = string.Empty;


            if (_InOut == "I")
            {
                _setInOut = "Deposit";
                JInOut = _setInOut.ToString();
                if (_Rejected == false && _Approved == true)
                {
                    JreceiveInfo = "received";
                    JsonQString = "{\"to\":\"" + _mobileStr + "\",\"type\":\"template\",\"template\":{\"namespace\":\"dc6b771d_f9c5_48e7_8b6a_93602eac6bd5\",\"name\":\"fundinout_broadcast\",\"language\":{\"policy\":\"deterministic\",\"code\":\"id\"},\"components\":[{\"type\":\"header\",\"parameters\":[{\"type\":\"text\",\"text\":\"" + JClientname + "\"}]},{\"type\":\"body\",\"parameters\":[{\"type\":\"text\",\"text\":\"" + JInOut + "\"},{\"type\":\"text\",\"text\":\"" + JreceiveInfo + "\"},{\"type\":\"text\",\"text\":\"" + _ApproveTime + "\"},{\"type\":\"text\",\"text\":\"" + JFioNID + "\"},{\"type\":\"text\",\"text\":\"" + JClientID + "\"},{\"type\":\"text\",\"text\":\"" + JAmount + "\"}]}]}}";
                }
                else

                {
                    JreceiveInfo = "rejected";
                    JsonQString = "{\"to\":\"" + _mobileStr + "\",\"type\":\"template\",\"template\":{\"namespace\":\"dc6b771d_f9c5_48e7_8b6a_93602eac6bd5\",\"name\":\"fundinout_broadcast\",\"language\":{\"policy\":\"deterministic\",\"code\":\"id\"},\"components\":[{\"type\":\"header\",\"parameters\":[{\"type\":\"text\",\"text\":\"" + JClientname + "\"}]},{\"type\":\"body\",\"parameters\":[{\"type\":\"text\",\"text\":\"" + JInOut + "\"},{\"type\":\"text\",\"text\":\"" + JreceiveInfo + "\"},{\"type\":\"text\",\"text\":\"" + _RejectTime + "\"},{\"type\":\"text\",\"text\":\"" + JFioNID + "\"},{\"type\":\"text\",\"text\":\"" + JClientID + "\"},{\"type\":\"text\",\"text\":\"" + JAmount + "\"}]}]}}";

                }

            }

            else if (_InOut == "O")
            {
                _setInOut = "Withdrawal request";
                JInOut = _setInOut.ToString();
                if (_Rejected == false && _Approved == true)
                {
                    JreceiveInfo = "approved";
                    JsonQString = "{\"to\":\"" + _mobileStr + "\",\"type\":\"template\",\"template\":{\"namespace\":\"dc6b771d_f9c5_48e7_8b6a_93602eac6bd5\",\"name\":\"fundinout_broadcast\",\"language\":{\"policy\":\"deterministic\",\"code\":\"id\"},\"components\":[{\"type\":\"header\",\"parameters\":[{\"type\":\"text\",\"text\":\"" + JClientname + "\"}]},{\"type\":\"body\",\"parameters\":[{\"type\":\"text\",\"text\":\"" + JInOut + "\"},{\"type\":\"text\",\"text\":\"" + JreceiveInfo + "\"},{\"type\":\"text\",\"text\":\"" + _ApproveTime + "\"},{\"type\":\"text\",\"text\":\"" + JFioNID + "\"},{\"type\":\"text\",\"text\":\"" + JClientID + "\"},{\"type\":\"text\",\"text\":\"" + JAmount + "\"}]}]}}";
                }
                else
                {
                    JreceiveInfo = "rejected";
                    JsonQString = "{\"to\":\"" + _mobileStr + "\",\"type\":\"template\",\"template\":{\"namespace\":\"dc6b771d_f9c5_48e7_8b6a_93602eac6bd5\",\"name\":\"fundinout_broadcast\",\"language\":{\"policy\":\"deterministic\",\"code\":\"id\"},\"components\":[{\"type\":\"header\",\"parameters\":[{\"type\":\"text\",\"text\":\"" + JClientname + "\"}]},{\"type\":\"body\",\"parameters\":[{\"type\":\"text\",\"text\":\"" + JInOut + "\"},{\"type\":\"text\",\"text\":\"" + JreceiveInfo + "\"},{\"type\":\"text\",\"text\":\"" + _RejectTime + "\"},{\"type\":\"text\",\"text\":\"" + JFioNID + "\"},{\"type\":\"text\",\"text\":\"" + JClientID + "\"},{\"type\":\"text\",\"text\":\"" + JAmount + "\"}]}]}}";
                }
            };

            JsonLString = JsonQString;


            //_currentDateTime = DateTime.Now;
            // _formattedDateTime = _ApproveTime.ToString("dd/MM/yyyy HH:mm:ss");


            var client = new RestClient("https://multichannel.qiscus.com/whatsapp/v1/drrws-ujjpbqjeqvhqo2l/4785/");
            var request = new RestRequest("messages", Method.Post);
            request.AddHeader("Qiscus-App-Id", "drrws-ujjpbqjeqvhqo2l");
            request.AddHeader("Qiscus-Secret-Key", "64f16ddbf847c48ff9731cb4e519da37");
            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("application/json", JsonLString, ParameterType.RequestBody);

            var response = client.Execute(request);

        }

        //API UnOfficial Function
        public async void GetApiPreop()
        {

            _currentDateTime = DateTime.Now;
            _formattedDateTime = _currentDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");

            var string1 = "";
            var string2 = "";

            //double statusCheckPre = double.Parse(_OrderStatusPreop_Open);
            //MessageBox.Show(statusCheckPre.ToString());
            string Status = string.Empty;
            if (statusCheckPre > 0)
            {
                string1 = _formattedDateTime.ToString() + System.Environment.NewLine + System.Environment.NewLine + "*FIX5*" + System.Environment.NewLine + "-------------------------------------------------------------" + System.Environment.NewLine + "Preopening Status Order are CLEARED with Total : " + System.Environment.NewLine + "Open Order " + _OrderStatusPreop_Open + " rows." + System.Environment.NewLine + "Rejected Order " + _OrderStatusPreop_Reject + " rows";
            }
            else if (statusCheckPre <= 0)
            {
                string1 = _formattedDateTime.ToString() + System.Environment.NewLine + System.Environment.NewLine + "*FIX5*" + System.Environment.NewLine + "-------------------------------------------------------------" + System.Environment.NewLine + "Preopening Status Order are ERROR with Total : " + System.Environment.NewLine + "Open Order " + _OrderStatusPreop_Open + " rows." + System.Environment.NewLine + "Rejected Order " + _OrderStatusPreop_Reject + " rows";
            }

            if (statusCheckPreOUCH > 0)
            {
                string2 =  System.Environment.NewLine + System.Environment.NewLine + "*OUCH*" + System.Environment.NewLine + "-------------------------------------------------------------" +  System.Environment.NewLine + "Preopening Status Order are CLEARED with Total : " + System.Environment.NewLine + "Open Order " + _OrderStatusPreop_Open_OUCH + " rows." + System.Environment.NewLine + "Rejected Order " + _OrderStatusPreop_Reject_OUCH + " rows";
            }
            else if (statusCheckPre <= 0)
            {
                string2 =  System.Environment.NewLine + System.Environment.NewLine + "*OUCH*" + System.Environment.NewLine + "-------------------------------------------------------------" +  System.Environment.NewLine + "Preopening Status Order are ERROR with Total : " + System.Environment.NewLine + "Open Order " + _OrderStatusPreop_Open_OUCH + " rows." + System.Environment.NewLine + "Rejected Order " + _OrderStatusPreop_Reject_OUCH + " rows";
            }

            _querySetPreop = string1 + string2;

            //AddLog(LogType.INFO, _querySet.ToString());
            //await RestHelper.Post("hsfvXBi91oPj2QHMuY8I", "6281110000665", "6287845016747", _querySetPreop);
            await RestHelper.Post("hsfvXBi91oPj2QHMuY8I", "6281110000665", "6281213076997", _querySetPreop);
            await RestHelper.Post("hsfvXBi91oPj2QHMuY8I", "6281110000665", "120363195609109582", _querySetPreop);

            //"120363195609109582"
        }


        public async void GetApiOpen()
        {
            _currentDateTime = DateTime.Now;
            _formattedDateTime = _currentDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");

            var string1 = "";
            var string2 = "";

            if (statusCheck < 5)
            {
                string1 = _formattedDateTime.ToString() + System.Environment.NewLine + System.Environment.NewLine + "*FIX5*" + System.Environment.NewLine + "-------------------------------------------------------------" + System.Environment.NewLine + "Opening Status Order are CLEARED with Total : " + System.Environment.NewLine + "Basket Order " + _OrderstatusOpening_Open + " rows." + System.Environment.NewLine + "Rejected Order " + _OrderstatusOpening_Reject + " rows";
            }
            else if (statusCheck > 10)
            {
                string1 = _formattedDateTime.ToString() + System.Environment.NewLine + System.Environment.NewLine + "*FIX5*" + System.Environment.NewLine + "-------------------------------------------------------------" + System.Environment.NewLine + "Opening Status Order are ERROR with Total : " + System.Environment.NewLine + "Basket Order " + _OrderstatusOpening_Open + " rows." + System.Environment.NewLine + "Rejected Order " + _OrderstatusOpening_Reject + " rows";
            }

            if (statusCheckOUCH < 5)
            {
                string2 = System.Environment.NewLine + System.Environment.NewLine + "*OUCH*" + System.Environment.NewLine + "-------------------------------------------------------------" + System.Environment.NewLine + "Opening Status Order are CLEARED with Total : " + System.Environment.NewLine + "Basket Order " + _OrderstatusOpening_Open + " rows." + System.Environment.NewLine + "Rejected Order " + _OrderstatusOpening_Reject + " rows";
            }
            else if (statusCheckOUCH > 10)
            {
                string2 = System.Environment.NewLine + System.Environment.NewLine + "*OUCH*" + System.Environment.NewLine + "-------------------------------------------------------------" + System.Environment.NewLine + "Opening Status Order are ERROR with Total : " + System.Environment.NewLine + "Basket Order " + _OrderstatusOpening_Open + " rows." + System.Environment.NewLine + "Rejected Order " + _OrderstatusOpening_Reject + " rows";
            }

            _querySetOpen = string1 + string2;
            //MessageBox.Show(Statusa);

            //AddLog(LogType.INFO, _querySet.ToString());
            //await RestHelper.Post("hsfvXBi91oPj2QHMuY8I", "6281110000665", "6287845016747", _querySetOpen);
            await RestHelper.Post("hsfvXBi91oPj2QHMuY8I", "6281110000665", "6281213076997", _querySetOpen);
            await RestHelper.Post("hsfvXBi91oPj2QHMuY8I", "6281110000665", "120363195609109582", _querySetOpen);
            //"120363195609109582"
        }

        public async void GetApiShort()
        {


            _currentDateTime = DateTime.Now;
            _formattedDateTime = _currentDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");


            _querySetShort = _formattedDateTime.ToString() + System.Environment.NewLine + System.Environment.NewLine + "*FIX5*" + System.Environment.NewLine + "Short ClientID = " + _ClientIDShort + System.Environment.NewLine + "StockID = " + _StockIDShort + System.Environment.NewLine + "Total Short = " + _TotalShort;
            //AddLog(LogType.INFO, _querySet.ToString());

            //await RestHelper.Post("hsfvXBi91oPj2QHMuY8I", "6281110000665", "6287845016747", _querySetShort);
            await RestHelper.Post("hsfvXBi91oPj2QHMuY8I", "6281110000665", "120363195609109582", _querySetShort);
        }

        public async void GetApiShortOUCH()
        {
            _currentDateTime = DateTime.Now;
            _formattedDateTime = _currentDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");

            _querySetShort = _formattedDateTime.ToString() + System.Environment.NewLine + System.Environment.NewLine + "*OUCH*" + System.Environment.NewLine + "Short ClientID = " + _ClientIDShortOUCH + System.Environment.NewLine + "StockID = " + _StockIDShortOUCH + System.Environment.NewLine + "Total Short = " + _TotalShortOUCH;

            //await RestHelper.Post("hsfvXBi91oPj2QHMuY8I", "6281110000665", "6287845016747", _querySetShort);
            await RestHelper.Post("hsfvXBi91oPj2QHMuY8I", "6281110000665", "120363195609109582", _querySetShort);
        }

        public async void GetApiOrderCheck()
        {
            _currentDateTime = DateTime.Now;
            _formattedDateTime = _currentDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");

            var string1 = "";
            var string2 = "";

            string1 = _formattedDateTime.ToString() + System.Environment.NewLine + System.Environment.NewLine + "*FIX5*" + System.Environment.NewLine + "Total Order = " + _TotalOrder;
            string2 = System.Environment.NewLine + System.Environment.NewLine + "*OUCH*" + System.Environment.NewLine + "Total Order = " + _TotalOrderOUCH;

            //AddLog(LogType.INFO, _querySet.ToString());

            _querySetOrder = string1 + string2;
            //await RestHelper.Post("hsfvXBi91oPj2QHMuY8I", "6281110000665", "6287845016747", _querySetOrder);
            await RestHelper.Post("hsfvXBi91oPj2QHMuY8I", "6281110000665", "120363195609109582", _querySetOrder);


        }

        public async void GetApiDbCompare()
        {
            _currentDateTime = DateTime.Now;
            _formattedDateTime = _currentDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");


            var string1 = _formattedDateTime.ToString() + System.Environment.NewLine + System.Environment.NewLine + "*TRUS*" + System.Environment.NewLine + "---------------------------------------------------------------------------" + System.Environment.NewLine + "| *Type*   |  *DBBO*    |  *DBBridge* | " + System.Environment.NewLine + "---------------------------------------------------------------------------" + System.Environment.NewLine + "| CC      |  " + _dbBOCountCCTrus + "    |  " + _dbBridgeCountCC + "      |" + System.Environment.NewLine + "---------------------------------------------------------------------------" + System.Environment.NewLine + "| CS      |  " + _dbBOCountCSTrus + "    |  " + _dbBridgeCountCS + "      |  " + System.Environment.NewLine + "---------------------------------------------------------------------------" + System.Environment.NewLine + "| Client |  " + _dbBOCountClientTrus + "    |  " + _dbBridgeCountClient + "      |  " + System.Environment.NewLine + "---------------------------------------------------------------------------" + System.Environment.NewLine + "| User   |  " + _dbBOCountUserTrus + "    |  " + _dbBridgeCountUser + "      |  " + System.Environment.NewLine + "---------------------------------------------------------------";
            var string2 = System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + "*S21*" + System.Environment.NewLine + "---------------------------------------------------------------------------" + System.Environment.NewLine + "| *Type*   |  *DBBO*    |     *DBFO*     | " + System.Environment.NewLine + "---------------------------------------------------------------------------" + System.Environment.NewLine + "| CC      |  " + _dbBOCountCCS21 + "    |  " + _dbFOCountCC + "      |" + System.Environment.NewLine + "---------------------------------------------------------------------------" + System.Environment.NewLine + "| CS      |  " + _dbBOCountCSS21 + "    |  " + _dbFOCountCS + "      |  " + System.Environment.NewLine + "---------------------------------------------------------------------------" + System.Environment.NewLine + "| Client |  " + _dbBOCountClientS21 + "    |  " + _dbFOCountClient + "      |  " + System.Environment.NewLine + "---------------------------------------------------------------------------" + System.Environment.NewLine + "| User   |  " + _dbBOCountUserS21 + "    |  " + _dbFOCountUser + "      |  " + System.Environment.NewLine + "---------------------------------------------------------------";
            var string3 = System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + "*OUCH*" + System.Environment.NewLine + "---------------------------------------------------------------------------" + System.Environment.NewLine + "| *Type*   |  *DBBridge*    |  *DBFO* | " + System.Environment.NewLine + "---------------------------------------------------------------------------" + System.Environment.NewLine + "| CC      |  " + _dbBridgeCountCC2 + "    |  " + _dbBridgeCountCCOUCH + "      |" + System.Environment.NewLine + "---------------------------------------------------------------------------" + System.Environment.NewLine + "| CS      |  " + _dbBridgeCountCS + "    |  " + _dbBridgeCountCSOUCH + "      |  " + System.Environment.NewLine + "---------------------------------------------------------------------------" + System.Environment.NewLine + "| Client |  " + _dbBridgeCountClient + "    |  " + _dbBridgeCountClientOUCH + "      |  " + System.Environment.NewLine + "---------------------------------------------------------------------------" + System.Environment.NewLine + "| User   |  " + _dbBridgeCountUser2 + "    |  " + _dbBridgeCountUserOUCH + "      |  " + System.Environment.NewLine + "---------------------------------------------------------------";
            
            _querySetDBCompare = string1 + string2 + string3;
            //AddLog(LogType.INFO, _querySet.ToString());

            //await RestHelper.Post("hsfvXBi91oPj2QHMuY8I", "6281110000665", "6287845016747", _querySetDBCompare);
            await RestHelper.Post("hsfvXBi91oPj2QHMuY8I", "6281110000665", "120363195609109582", _querySetDBCompare);
            await RestHelper.Post("hsfvXBi91oPj2QHMuY8I", "6281110000665", "6281297037370", _querySetDBCompare);
        }

        #region Process BackOffice Data

        protected void QueueFundInOutData(ColumnBase data)
        {
            lock (_fioDataQueue)
                _fioDataQueue.Enqueue(data);
        }

        public void ProcessFundInOutQueue()
        {
            int cnt = _fioDataQueue.Count;
            for (int i = 0, j = cnt; i < j; i++)
            {

                ColumnBase o = null;
                lock (_fioDataQueue)
                {
                    o = _fioDataQueue.Dequeue();
                }
                if (o == null) continue;

                try
                {
                    this._fiolblQueue.show(System.Threading.Interlocked.Decrement(ref QueueCount));
                    switch (o.DataType)
                    {
                        case ColumnType.FundInOutModelType:
                            GetDBTimestamp();
                            System.Threading.Interlocked.Decrement(ref FioCount);
                            if (_fioDataQueue.Count == 0 || FioCount == 0)
                                AddLog(LogType.INFO, "Completed XT -> WA Sent");
                            break;
                        /*case ColumnType.OrderCheckType:
                            GetOrderStatusPreop();
                            System.Threading.Interlocked.Decrement(ref FioCount);
                            if (_fioDataQueue.Count == 0 || FioCount == 0)
                                AddLog(LogType.INFO, "Completed XT -> WA Sent");
                            break;
                       */


                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    _fioDataFailedQueue.Enqueue(o);
                    AddLog(LogType.ERROR, e.Message, e.StackTrace);
                }
            }
        }

        public void ProcessBackOfficeQueueFailed()
        {
            for (int i = 0, j = _fioDataFailedQueue.Count; i < j; i++)
            {
                ColumnBase o = null;
                lock (_fioDataQueue)
                {
                    o = _fioDataFailedQueue.Dequeue();
                }
                if (o == null) continue;

                try
                {
                    switch (o.DataType)
                    {
                        case ColumnType.FundInOutModelType:
                            GetDBTimestamp();
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    AddLog(LogType.ERROR, e.Message, e.StackTrace);
                }
            }
        }

        public void LoadTimestamp()
        {
            lock (_timestampLock)
            {
                try
                {
                    StreamReader sr = new StreamReader(File.OpenRead(@"timestamp.txt"));
                    string data = sr.ReadToEnd();
                    sr.Close();
                    string[] timestamps = data.Split(new char[] { '|' });
                    foreach (string ts in timestamps)
                    {
                        string[] keyvalue = ts.Split(new char[] { '=' });
                        if (keyvalue.Length == 2)
                        {
                            switch (keyvalue[0])
                            {
                                case "TimeStampWrite":
                                    _CCtimeStampBuffer.CCTimestamp = long.Parse(keyvalue[1]);
                                    break;

                            }
                            //AddLog(LogType.INFO, _CCtimeStampBuffer.CCTimestamp.ToString());
                        }
                    }

                }
                catch (Exception e)
                {
                    AddLog(LogType.ERROR, e.Message, e.StackTrace);
                }
            }
        }

        public void StoreTimestamp()
        {
            lock (_timestampLock)
            {
                try
                {
                    StreamWriter sw = new StreamWriter(File.Create(@"timestamp.txt"));
                    StringBuilder sb = new StringBuilder();
                    sb.Append("TimeStampWrite=").Append(_CCtimeStampBuffer.CCTimestamp);
                    sw.WriteLine(sb.ToString());
                    sw.Flush();
                    sw.Close();
                    //AddLog(LogType.INFO, sb.ToString());
                }
                catch (Exception e)
                {
                    AddLog(LogType.ERROR, e.Message, e.StackTrace);
                }
            }
        }
        protected void Initialize()
        {
            _CCtimeStampBuffer = new TimeStampBuffer();
            _logQueue = new Queue<LogBase>();
            _fioDataQueue = new Queue<ColumnBase>();
            _fioDataFailedQueue = new Queue<ColumnBase>();

        }
        protected bool GetBooleanValue(string value)
        {
            if (value.Trim().Equals("0") || value.ToLowerInvariant().Trim().Equals("false"))
                return false;
            return true;
        }

        public void AddLog(LogType Type, string Message)
        {
            LogInfo o = new LogInfo();
            o.Message = Message;
            QueueAddLog(o);
        }

        public void AddLog(LogType Type, string Message, string Stacktrace)
        {
            LogError o = new LogError();
            o.Message = Message;
            o.Stacktrace = Stacktrace;
            QueueAddLog(o);
        }
        public void QueueAddLog(LogBase logBase)
        {
            lock (_logQueue)
                _logQueue.Enqueue(logBase);
        }

        public void ProcessLogQueue()
        {
            int cnt = _logQueue.Count;
            for (int i = 0, j = cnt; i < j; i++)
            {

                LogBase o = null;
                lock (_logQueue)
                {
                    o = _logQueue.Dequeue();
                }
                if (o == null) continue;

                try
                {
                    switch (o.DataType)
                    {
                        case LogType.INFO:
                            Log(o);
                            break;
                        case LogType.ERROR:
                            LogException(o);
                            break;
                        default:
                            break;
                    }

                }

                catch
                {
                    //AddLog(LogType.ERROR, e.Message, e.StackTrace);
                }
            }
        }

        protected void Log(LogBase o)
        {
            LogInfo a = o as LogInfo;
            if (this._logger == null)
                return;

            DateTime logTime = DateTime.Now;


            foreach (ILogger il in this._logger)
                if (il != null)
                    il.Log(logTime, a.DataType, a.Message);


        }

        protected void LogException(LogBase o)
        {
            LogError a = o as LogError;
            if (this._logger == null)
                return;

            DateTime logTime = DateTime.Now;


            foreach (ILogger il in this._logger)
                if (il != null)
                    il.LogException(logTime, a.DataType, a.Message, a.Stacktrace);

        }




        //ListViewList(o);



        //ColumnBase o = null;
        //ListViewList(o);

        /*try
        {


            ColumnBase o = null;
            FundInOutModel a = o as FundInOutModel;
            string ClientIDres = a.ClientID.ToString();
            string Amountres = a.Amount.ToString();

            //MessageBox.Show(ClientIDres + Amountres);
            //txtResponse.Text = result.Count.ToString();



        //DateTime currentDateTime = DateTime.Now;
        //string formattedDateTime = currentDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");
        //querySet = formattedDateTime + System.Environment.NewLine + System.Environment.NewLine + "Total Count S21" + System.Environment.NewLine + System.Environment.NewLine + "ClientCash : " + queryres + System.Environment.NewLine + "ClientStock : " + queryres2 + System.Environment.NewLine + "User : " + queryres3 + System.Environment.NewLine + "Client : " + queryres4 + System.Environment.NewLine + System.Environment.NewLine + "------------------------------------------"
        //+ System.Environment.NewLine + System.Environment.NewLine + "Total Count Trus" + System.Environment.NewLine + System.Environment.NewLine + "ClientCash : " + queryres5 + System.Environment.NewLine + "ClientStock : " + queryres6 + System.Environment.NewLine + "User : " + queryres7 + System.Environment.NewLine + "Client : " + queryres8;
        querySet = ClientIDres + System.Environment.NewLine + Amountres;
            textBox1.Text = (querySet);
        //txtResponse.Text = formattedDateTime + System.Environment.NewLine + System.Environment.NewLine + writeTS;
        //}

        //var response = await RestHelper.Post("hsfvXBi91oPj2QHMuY8I", "6285174377897", "120363195609109582", querySet );
            //txtResponse.Text = RestHelper.BeautifyJson(response);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        };
        */

        //txtResponse.Text = "\tSend Total Query DB -> WA DONE" + System.Environment.NewLine + System.Environment.NewLine + querySet;




    }
}


#endregion
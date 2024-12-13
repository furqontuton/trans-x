using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WA_Send_API.DataModel;

namespace WA_Send_API.Function
{
    public class FunctionScheduler
    {
        private bool _isNewStart = true;
        private bool _isGetdataFIOdone, _isGetStatusPreopdone, _isGetStatusOpendone, _isGetShortdone, _isGetOrderdone, _isGetDBCompare;
        private TimeSpan _timeSpanPreop, _timeSpanOpen, _timeSpanShort, _timeSpanOrder, _timeSpanDBcompare;

        private DateTime _prevDate;
        private System.Timers.Timer _timer;
        private FunctionContext _context;     

        public FunctionContext FunctionContext
        { 
            get { return this._context; }
            set { this._context = value; }
        }
            
        public FunctionScheduler(FunctionContext context)
        {
            this._context = context;
            this._timer = new System.Timers.Timer(20000);
            this._timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
            this._prevDate = DateTime.Now;
        }
        public void Start()
        {
            this._timer.Start();
            _timeSpanPreop = TimeSpan.Parse("08:45:02");
            _timeSpanOpen = TimeSpan.Parse("09:00:02");
            _timeSpanShort = TimeSpan.Parse("16:10:00");
            _timeSpanOrder = TimeSpan.Parse("04:16:00");
            _timeSpanDBcompare = TimeSpan.Parse("04:12:00");
            this._timer_Elapsed(null, null);

            //this._timer.Start(); //for test only
            //_timeSpanPreop = TimeSpan.Parse("15:12:00");
            //_timeSpanOpen = TimeSpan.Parse("15:12:10");
            //_timeSpanShort = TimeSpan.Parse("15:12:20");
            //_timeSpanOrder = TimeSpan.Parse("15:12:30");
            //_timeSpanDBcompare = TimeSpan.Parse("15:12:40");
            //this._timer_Elapsed(null, null);


        }

        public void Stop()
        {
            this._timer.Stop();
        }
                
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this._context == null)
                return;
            if (this._prevDate < DateTime.Now.Date)
            {
                this._prevDate = DateTime.Now;    
                this._isGetdataFIOdone = this._isGetStatusPreopdone = this._isGetStatusOpendone =this._isGetShortdone=this._isGetDBCompare = this._isNewStart = false; 
            }
            TimeSpan now = DateTime.Now.TimeOfDay;
            if (_timeSpanPreop <= now && !this._isGetStatusPreopdone)
            {
                if (this._isNewStart && this._prevDate.TimeOfDay > _timeSpanPreop)
                {

                }
                else
                {
                    this._isGetStatusPreopdone = true;
                    this._context.GetOrderStatusPreop();
                    this._context.GetOrderStatusPreOpOUCH();
                    this._context.GetApiPreop();
                }
                this._isGetStatusPreopdone = true;    
            }

            if (_timeSpanOpen <= now && !this._isGetStatusOpendone)
            {
                if (this._isNewStart && this._prevDate.TimeOfDay > _timeSpanOpen)
                {

                }
                else
                {
                    this._isGetStatusOpendone = true;
                    this._context.GetOrderStatusOpen();
                    this._context.GetOrderStatusOpenOUCH();
                    this._context.GetApiOpen();
                }
                this._isGetStatusOpendone = true;
            }
            if (_timeSpanShort <= now && !this._isGetShortdone)
            {
                if (this._isNewStart && this._prevDate.TimeOfDay > _timeSpanShort)
                {

                }
                else
                {
                    this._isGetShortdone = true;
                    this._context.GetClientShort();
                    this._context.GetClientShortOUCH();

                }
                this._isGetShortdone = true;
            }

            if (_timeSpanOrder <= now && !this._isGetOrderdone)
            {
                if (this._isNewStart && this._prevDate.TimeOfDay > _timeSpanOrder)
                {

                }
                else
                {
                    this._isGetOrderdone = true;
                    this._context.GetOrderData();
                    this._context.GetOrderDataOuch();
                    this._context.GetApiOrderCheck();
                }
                this._isGetOrderdone = true;
            }

            if (_timeSpanDBcompare <= now && !this._isGetDBCompare)
            {
                if (this._isNewStart && this._prevDate.TimeOfDay > _timeSpanDBcompare)
                {

                }
                else
                { 
                    this._isGetDBCompare = true;
                    this._context.GetDBCompare();
                }
                this._isGetDBCompare = true;
            }
            else
            {
                this._isGetdataFIOdone = true;
                //this._context.GetDBTimestamp();
            }
        }
    }
}
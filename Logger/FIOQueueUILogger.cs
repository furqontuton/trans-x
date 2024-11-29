using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WA_Send_API.Logger
{
    public class FIOQueueUILogger
    {
        private ToolStripStatusLabel _lblQueue;
        private Form main;
        private ToolStrip _toolStrip;
        public FIOQueueUILogger(ToolStripStatusLabel lblQueue, ToolStrip toolStrip, Form frmMain)
        {
            this._lblQueue = lblQueue;
            main = frmMain;
            this._toolStrip = toolStrip;
        }

        public void show(double i)
        {
            try
            {
                if (_toolStrip.InvokeRequired)
                {
                    this._toolStrip.Invoke(new CrossThreadLabelQueueHandler(show), i);
                }
                else
                {
                    lock (this._lblQueue)
                    {
                        _lblQueue.Text = "Queue : " + i.ToString();
                        //--queue;
                    }
                }
            }
            catch { }
        }

        public void addQueue(double i)
        {
            try
            {
                if (_toolStrip.InvokeRequired)
                {
                    if (_toolStrip.InvokeRequired)
                    {
                        this._toolStrip.Invoke(new CrossThreadLabelQueueHandler(addQueue), i);
                    }
                }
                else
                {
                    lock (this._lblQueue)
                    {
                        //queue += i;
                        _lblQueue.Text = "Queue : " + i.ToString();
                    }
                }
            }
            catch { }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;
using System.Text.Json;
using System.Threading;
using WA_Send_API.Function;
using WA_Send_API.DataSql;
using System.Runtime.InteropServices;
using WA_Send_API.DataModel;
using System.Data.SqlClient;
using System.Data;
using WA_Send_API.Logger;


namespace WA_Send_API
{
    public class Program
    {
        private static DateTime prevDate;
        private static ILogger[] logger;
        private static Main ui;
        private static bool running = true;
        private static Thread FundinoutThread, tmrThread, logThread;
        private static FunctionContext fc;
        private static FunctionScheduler sch;
        private static FIOQueueUILogger FIOlblQueue;
        [STAThread]
        private static void Main(string[] args)
        {
                ui = new Main();
                //fc = new FunctionContext();
                logger = new ILogger[2];
                prevDate = DateTime.Now;
                logger[0] = new FundInOutUILogger(ui.ListViewStatus);

                FIOlblQueue = new FIOQueueUILogger(ui.labelQueue, ui.toolStrip, ui);


                fc = new FunctionContext(FIOlblQueue, logger);
                fc.InitFIOSqlWrapper();
                fc.LoadTimestamp();


            
            sch = new FunctionScheduler(fc);
            sch.Start();
            Log(LogType.INFO, "Application Started...");

            running = true;

            //System.Timers.Timer timer = new System.Timers.Timer(2000); // Interval in milliseconds
            //timer.Elapsed += async (sender, e) =>
                tmrThread = new Thread(() =>
                {
                    while (running)
                    {
                        //Log(LogType.INFO, "Starting Get Data...");
                        fc.StoreTimestamp();
                        
                        Thread.Sleep(1000);
                    }
                });
                FundinoutThread = new Thread(() =>
                {
                   
                    while (running)
                    {
                        
                        fc.ProcessFundInOutQueue();
                        fc.ProcessBackOfficeQueueFailed();
                        Thread.Sleep(1000);
                    }
                });
                logThread = new Thread(() =>
                {
                    while (running)
                    {
                        fc.ProcessLogQueue();
                        Thread.Sleep(1000);
                    }
                });
            ui.FunctionContext = fc;

            FundinoutThread.Start();
            tmrThread.Start();
            logThread.Start();
            
            
            Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(ui);

        }

        
        public static void Shutdown()
        {
            running = false;

            if (tmrThread != null)
            {
                tmrThread.Join();
                tmrThread = null;
            }

            if (FundinoutThread != null)
            {
                FundinoutThread.Join();
                FundinoutThread = null;
            }

            if (logThread != null)
            {
                logThread.Join();
                logThread = null;
            }

            if (sch != null)
            {
                sch.Stop();
                sch = null;
            }

            fc = null;
        }

        public static void Kill(bool status)
        {
            running = false;

            if (tmrThread != null)
            {
                tmrThread.Abort();
                tmrThread = null;
            }

            if (FundinoutThread != null)
            {
                FundinoutThread.Abort();
                FundinoutThread = null;
            }

            if (logThread != null)
            {
                logThread.Abort();
                logThread = null;
            }

            if (sch != null)
            {
                sch.Stop();
                sch = null;
            }

            
            fc = null;
            if (status)
                Environment.Exit(0);
        }
        private static void Log(LogType type, string message)
        {
            if (logger == null)
                return;

            DateTime logTime = DateTime.Now;
            foreach (ILogger il in logger)
                if (il != null)
                    il.Log(logTime, type, message);
        }
    }
}

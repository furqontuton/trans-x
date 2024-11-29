

namespace WA_Send_API
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.sendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendWhatsappToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aLLCountDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colStatusStacktrace = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FioLblQueue = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripSttsLblQueue = new System.Windows.Forms.ToolStripStatusLabel();
            this.LvwList = new WA_Send_API.UI.WASAPIListView();
            this.colStatusDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colStatusLogType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colStatusInformation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.testSendDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Orange;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 12);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(776, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // sendToolStripMenuItem
            // 
            this.sendToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendWhatsappToolStripMenuItem,
            this.testConnectionToolStripMenuItem});
            this.sendToolStripMenuItem.Name = "sendToolStripMenuItem";
            this.sendToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.sendToolStripMenuItem.Text = "Send";
            // 
            // sendWhatsappToolStripMenuItem
            // 
            this.sendWhatsappToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aLLCountDataToolStripMenuItem,
            this.testSendDataToolStripMenuItem});
            this.sendWhatsappToolStripMenuItem.Name = "sendWhatsappToolStripMenuItem";
            this.sendWhatsappToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.sendWhatsappToolStripMenuItem.Text = "Send Whatsapp";
            // 
            // aLLCountDataToolStripMenuItem
            // 
            this.aLLCountDataToolStripMenuItem.Name = "aLLCountDataToolStripMenuItem";
            this.aLLCountDataToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aLLCountDataToolStripMenuItem.Text = "ALL Count Data";
            this.aLLCountDataToolStripMenuItem.Click += new System.EventHandler(this.aLLCountDataToolStripMenuItem_Click);
            // 
            // testConnectionToolStripMenuItem
            // 
            this.testConnectionToolStripMenuItem.Name = "testConnectionToolStripMenuItem";
            this.testConnectionToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.testConnectionToolStripMenuItem.Text = "Test Connection";
            // 
            // colStatusStacktrace
            // 
            this.colStatusStacktrace.Text = "Stacktrace";
            this.colStatusStacktrace.Width = 175;
            // 
            // FioLblQueue
            // 
            this.FioLblQueue.AutoSize = true;
            this.FioLblQueue.BackColor = System.Drawing.Color.LemonChiffon;
            this.FioLblQueue.Location = new System.Drawing.Point(710, 20);
            this.FioLblQueue.Name = "FioLblQueue";
            this.FioLblQueue.Size = new System.Drawing.Size(0, 13);
            this.FioLblQueue.TabIndex = 4;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSttsLblQueue});
            this.statusStrip1.Location = new System.Drawing.Point(0, 418);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(776, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripSttsLblQueue
            // 
            this.toolStripSttsLblQueue.BackColor = System.Drawing.Color.AntiqueWhite;
            this.toolStripSttsLblQueue.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSttsLblQueue.Name = "toolStripSttsLblQueue";
            this.toolStripSttsLblQueue.Size = new System.Drawing.Size(51, 17);
            this.toolStripSttsLblQueue.Text = "Queue : ";
            // 
            // LvwList
            // 
            this.LvwList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colStatusDate,
            this.colStatusLogType,
            this.colStatusInformation});
            this.LvwList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LvwList.FullRowSelect = true;
            this.LvwList.GridLines = true;
            this.LvwList.HideSelection = false;
            this.LvwList.Location = new System.Drawing.Point(0, 36);
            this.LvwList.Name = "LvwList";
            this.LvwList.Size = new System.Drawing.Size(776, 404);
            this.LvwList.TabIndex = 3;
            this.LvwList.UseCompatibleStateImageBehavior = false;
            this.LvwList.View = System.Windows.Forms.View.Details;
            this.LvwList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.LvwList_ColumnClick);
            // 
            // colStatusDate
            // 
            this.colStatusDate.Text = "Date";
            this.colStatusDate.Width = 115;
            // 
            // colStatusLogType
            // 
            this.colStatusLogType.Text = "Type";
            // 
            // colStatusInformation
            // 
            this.colStatusInformation.Text = "Information";
            this.colStatusInformation.Width = 432;
            // 
            // testSendDataToolStripMenuItem
            // 
            this.testSendDataToolStripMenuItem.Name = "testSendDataToolStripMenuItem";
            this.testSendDataToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.testSendDataToolStripMenuItem.Text = "Test Send Data";
            //this.testSendDataToolStripMenuItem.Click += new System.EventHandler(this.testSendDataToolStripMenuItem_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(776, 440);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.FioLblQueue);
            this.Controls.Add(this.LvwList);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.Padding = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Trans Xtions";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.Load += new System.EventHandler(this.Main_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        /*private void InitializeListView()
        {
            LvwList.View = View.Details;
            LvwList.LabelEdit = true;
            LvwList.AllowColumnReorder = true;
            LvwList.FullRowSelect = true;
            LvwList.GridLines = true;
            //LvwList.Sorting = SortOrder.Ascending;
            LvwList.Columns.Add("FundinoutNID", 150, HorizontalAlignment.Left);
            LvwList.Columns.Add("Clientid", 250, HorizontalAlignment.Left);
            LvwList.Columns.Add("Cash", 100, HorizontalAlignment.Left);
            LvwList.Columns.Add("In/Out", 100, HorizontalAlignment.Left);

            
            
        }
        */

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem sendToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendWhatsappToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aLLCountDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testConnectionToolStripMenuItem;
        private WA_Send_API.UI.WASAPIListView LvwList;
        private System.Windows.Forms.ColumnHeader colStatusDate;
        private System.Windows.Forms.ColumnHeader colStatusLogType;
        private System.Windows.Forms.ColumnHeader colStatusInformation;
        private System.Windows.Forms.ColumnHeader colStatusStacktrace;
        private System.Windows.Forms.Label FioLblQueue;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripSttsLblQueue;
        private System.Windows.Forms.ToolStripMenuItem testSendDataToolStripMenuItem;
    }
}


namespace nps
{
    partial class MainForm
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
            btnStart = new Button();
            listPacket = new ListView();
            label1 = new Label();
            selectDevices = new ComboBox();
            btnStop = new Button();
            label2 = new Label();
            comboBox1 = new ComboBox();
            comboBox2 = new ComboBox();
            comboBox3 = new ComboBox();
            btnDeviceDetails = new Button();
            btnFilterStart = new Button();
            btnFilterStop = new Button();
            listFilter = new ListView();
            treeView1 = new TreeView();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(296, 56);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(94, 29);
            btnStart.TabIndex = 1;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // listPacket
            // 
            listPacket.FullRowSelect = true;
            listPacket.Location = new Point(12, 91);
            listPacket.Name = "listPacket";
            listPacket.Size = new Size(988, 312);
            listPacket.TabIndex = 2;
            listPacket.UseCompatibleStateImageBehavior = false;
            listPacket.SelectedIndexChanged += listPacket_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(84, 20);
            label1.TabIndex = 3;
            label1.Text = "网卡选择：";
            // 
            // selectDevices
            // 
            selectDevices.FormattingEnabled = true;
            selectDevices.Location = new Point(102, 6);
            selectDevices.Name = "selectDevices";
            selectDevices.Size = new Size(451, 28);
            selectDevices.TabIndex = 4;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(459, 56);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(94, 29);
            btnStop.TabIndex = 5;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(576, 9);
            label2.Name = "label2";
            label2.Size = new Size(69, 20);
            label2.TabIndex = 6;
            label2.Text = "过滤器：";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(642, 6);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(80, 28);
            comboBox1.TabIndex = 7;
            comboBox1.Text = "网络层";
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(739, 6);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(80, 28);
            comboBox2.TabIndex = 8;
            comboBox2.Text = "会话层";
            // 
            // comboBox3
            // 
            comboBox3.FormattingEnabled = true;
            comboBox3.Location = new Point(840, 6);
            comboBox3.Name = "comboBox3";
            comboBox3.Size = new Size(80, 28);
            comboBox3.TabIndex = 9;
            comboBox3.Text = "应用层";
            // 
            // btnDeviceDetails
            // 
            btnDeviceDetails.Location = new Point(12, 56);
            btnDeviceDetails.Name = "btnDeviceDetails";
            btnDeviceDetails.Size = new Size(94, 29);
            btnDeviceDetails.TabIndex = 10;
            btnDeviceDetails.Text = "网卡信息";
            btnDeviceDetails.UseVisualStyleBackColor = true;
            btnDeviceDetails.Click += btnDeviceDetails_Click;
            // 
            // btnFilterStart
            // 
            btnFilterStart.Location = new Point(725, 56);
            btnFilterStart.Name = "btnFilterStart";
            btnFilterStart.Size = new Size(94, 29);
            btnFilterStart.TabIndex = 11;
            btnFilterStart.Text = "开始过滤";
            btnFilterStart.UseVisualStyleBackColor = true;
            btnFilterStart.Click += btnFilterStart_Click;
            // 
            // btnFilterStop
            // 
            btnFilterStop.Location = new Point(826, 56);
            btnFilterStop.Name = "btnFilterStop";
            btnFilterStop.Size = new Size(94, 29);
            btnFilterStop.TabIndex = 12;
            btnFilterStop.Text = "取消过滤";
            btnFilterStop.UseVisualStyleBackColor = true;
            btnFilterStop.Click += btnFilterStop_Click;
            // 
            // listFilter
            // 
            listFilter.FullRowSelect = true;
            listFilter.Location = new Point(12, 91);
            listFilter.Name = "listFilter";
            listFilter.Size = new Size(988, 312);
            listFilter.TabIndex = 13;
            listFilter.UseCompatibleStateImageBehavior = false;
            listFilter.Visible = false;
            listFilter.SelectedIndexChanged += listFilter_SelectedIndexChanged;
            // 
            // treeView1
            // 
            treeView1.Location = new Point(12, 409);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(988, 216);
            treeView1.TabIndex = 14;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1012, 637);
            Controls.Add(treeView1);
            Controls.Add(listFilter);
            Controls.Add(btnFilterStop);
            Controls.Add(btnFilterStart);
            Controls.Add(btnDeviceDetails);
            Controls.Add(comboBox3);
            Controls.Add(comboBox2);
            Controls.Add(comboBox1);
            Controls.Add(label2);
            Controls.Add(btnStop);
            Controls.Add(selectDevices);
            Controls.Add(label1);
            Controls.Add(listPacket);
            Controls.Add(btnStart);
            Name = "MainForm";
            Text = "网络嗅探工具";
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnStart;
        private ListView listPacket;
        private Label label1;
        private ComboBox selectDevices;
        private Button btnStop;
        private Label label2;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private ComboBox comboBox3;
        private Button btnDeviceDetails;
        private Button btnFilterStart;
        private Button btnFilterStop;
        private ListView listFilter;
        private TreeView treeView1;
    }
}
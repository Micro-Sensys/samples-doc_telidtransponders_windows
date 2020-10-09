namespace SampleApp_CSharp_GetSensor
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panel_Sensor = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel_ResultSensor = new System.Windows.Forms.Panel();
            this.label_LastTime = new System.Windows.Forms.Label();
            this.richTextBox_SensorValues = new System.Windows.Forms.RichTextBox();
            this.label_SerialNumber = new System.Windows.Forms.Label();
            this.panel_Connection = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.button_Initialize = new System.Windows.Forms.Button();
            this.comboBox_Protocol = new System.Windows.Forms.ComboBox();
            this.comboBox_PortType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown_PortNumber = new System.Windows.Forms.NumericUpDown();
            this.panel_StartStop = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_ParamArray = new System.Windows.Forms.TextBox();
            this.textBox_PhysicalSize = new System.Windows.Forms.TextBox();
            this.textBox_SensorType = new System.Windows.Forms.TextBox();
            this.radioButton_ManSensor = new System.Windows.Forms.RadioButton();
            this.radioButton_AutoSensor = new System.Windows.Forms.RadioButton();
            this.button_StartStop = new System.Windows.Forms.Button();
            this.timer_Scan = new System.Windows.Forms.Timer(this.components);
            this.timer_CheckReader = new System.Windows.Forms.Timer(this.components);
            this.panel_Info = new System.Windows.Forms.Panel();
            this.panel_ReaderStatus = new System.Windows.Forms.Panel();
            this.textBox_ReaderID = new System.Windows.Forms.TextBox();
            this.textBox_DriverVersion = new System.Windows.Forms.TextBox();
            this.panel_Sensor.SuspendLayout();
            this.panel_Connection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_PortNumber)).BeginInit();
            this.panel_StartStop.SuspendLayout();
            this.panel_Info.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_Sensor
            // 
            this.panel_Sensor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_Sensor.Controls.Add(this.label6);
            this.panel_Sensor.Controls.Add(this.label5);
            this.panel_Sensor.Controls.Add(this.label4);
            this.panel_Sensor.Controls.Add(this.label3);
            this.panel_Sensor.Controls.Add(this.panel_ResultSensor);
            this.panel_Sensor.Controls.Add(this.label_LastTime);
            this.panel_Sensor.Controls.Add(this.richTextBox_SensorValues);
            this.panel_Sensor.Controls.Add(this.label_SerialNumber);
            this.panel_Sensor.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_Sensor.Location = new System.Drawing.Point(0, 313);
            this.panel_Sensor.Name = "panel_Sensor";
            this.panel_Sensor.Size = new System.Drawing.Size(430, 224);
            this.panel_Sensor.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(52, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(99, 16);
            this.label6.TabIndex = 11;
            this.label6.Text = "Sensor Values:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(303, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 16);
            this.label5.TabIndex = 10;
            this.label5.Text = "Last Time Read:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(52, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 16);
            this.label4.TabIndex = 9;
            this.label4.Text = "Serial Number:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Last Result:";
            // 
            // panel_ResultSensor
            // 
            this.panel_ResultSensor.Location = new System.Drawing.Point(81, 13);
            this.panel_ResultSensor.Name = "panel_ResultSensor";
            this.panel_ResultSensor.Size = new System.Drawing.Size(20, 20);
            this.panel_ResultSensor.TabIndex = 7;
            // 
            // label_LastTime
            // 
            this.label_LastTime.AutoSize = true;
            this.label_LastTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_LastTime.Location = new System.Drawing.Point(325, 91);
            this.label_LastTime.Name = "label_LastTime";
            this.label_LastTime.Size = new System.Drawing.Size(71, 20);
            this.label_LastTime.TabIndex = 2;
            this.label_LastTime.Text = "10:00:00";
            // 
            // richTextBox_SensorValues
            // 
            this.richTextBox_SensorValues.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_SensorValues.Location = new System.Drawing.Point(123, 107);
            this.richTextBox_SensorValues.Name = "richTextBox_SensorValues";
            this.richTextBox_SensorValues.Size = new System.Drawing.Size(149, 105);
            this.richTextBox_SensorValues.TabIndex = 1;
            this.richTextBox_SensorValues.Text = "";
            // 
            // label_SerialNumber
            // 
            this.label_SerialNumber.AutoSize = true;
            this.label_SerialNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_SerialNumber.Location = new System.Drawing.Point(155, 58);
            this.label_SerialNumber.Name = "label_SerialNumber";
            this.label_SerialNumber.Size = new System.Drawing.Size(18, 20);
            this.label_SerialNumber.TabIndex = 0;
            this.label_SerialNumber.Text = "0";
            // 
            // panel_Connection
            // 
            this.panel_Connection.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_Connection.Controls.Add(this.label2);
            this.panel_Connection.Controls.Add(this.button_Initialize);
            this.panel_Connection.Controls.Add(this.comboBox_Protocol);
            this.panel_Connection.Controls.Add(this.comboBox_PortType);
            this.panel_Connection.Controls.Add(this.label1);
            this.panel_Connection.Controls.Add(this.numericUpDown_PortNumber);
            this.panel_Connection.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_Connection.Location = new System.Drawing.Point(0, 0);
            this.panel_Connection.Name = "panel_Connection";
            this.panel_Connection.Size = new System.Drawing.Size(430, 101);
            this.panel_Connection.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(147, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Port Type:";
            // 
            // button_Initialize
            // 
            this.button_Initialize.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Initialize.Location = new System.Drawing.Point(306, 12);
            this.button_Initialize.Name = "button_Initialize";
            this.button_Initialize.Size = new System.Drawing.Size(112, 74);
            this.button_Initialize.TabIndex = 4;
            this.button_Initialize.Text = "INITIALIZE";
            this.button_Initialize.UseVisualStyleBackColor = true;
            this.button_Initialize.Click += new System.EventHandler(this.button_Initialize_Click);
            // 
            // comboBox_Protocol
            // 
            this.comboBox_Protocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Protocol.Enabled = false;
            this.comboBox_Protocol.FormattingEnabled = true;
            this.comboBox_Protocol.Items.AddRange(new object[] {
            "3000pro",
            "v4 protocol"});
            this.comboBox_Protocol.Location = new System.Drawing.Point(12, 65);
            this.comboBox_Protocol.Name = "comboBox_Protocol";
            this.comboBox_Protocol.Size = new System.Drawing.Size(92, 21);
            this.comboBox_Protocol.TabIndex = 3;
            // 
            // comboBox_PortType
            // 
            this.comboBox_PortType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_PortType.FormattingEnabled = true;
            this.comboBox_PortType.Items.AddRange(new object[] {
            "Serial",
            "Bluetooth",
            "USB"});
            this.comboBox_PortType.Location = new System.Drawing.Point(168, 45);
            this.comboBox_PortType.Name = "comboBox_PortType";
            this.comboBox_PortType.Size = new System.Drawing.Size(94, 21);
            this.comboBox_PortType.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "COM";
            // 
            // numericUpDown_PortNumber
            // 
            this.numericUpDown_PortNumber.Location = new System.Drawing.Point(49, 27);
            this.numericUpDown_PortNumber.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numericUpDown_PortNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_PortNumber.Name = "numericUpDown_PortNumber";
            this.numericUpDown_PortNumber.Size = new System.Drawing.Size(43, 20);
            this.numericUpDown_PortNumber.TabIndex = 0;
            this.numericUpDown_PortNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // panel_StartStop
            // 
            this.panel_StartStop.Controls.Add(this.label9);
            this.panel_StartStop.Controls.Add(this.label8);
            this.panel_StartStop.Controls.Add(this.label7);
            this.panel_StartStop.Controls.Add(this.textBox_ParamArray);
            this.panel_StartStop.Controls.Add(this.textBox_PhysicalSize);
            this.panel_StartStop.Controls.Add(this.textBox_SensorType);
            this.panel_StartStop.Controls.Add(this.radioButton_ManSensor);
            this.panel_StartStop.Controls.Add(this.radioButton_AutoSensor);
            this.panel_StartStop.Controls.Add(this.button_StartStop);
            this.panel_StartStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_StartStop.Location = new System.Drawing.Point(0, 101);
            this.panel_StartStop.Name = "panel_StartStop";
            this.panel_StartStop.Size = new System.Drawing.Size(430, 212);
            this.panel_StartStop.TabIndex = 2;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 139);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(130, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Please enter params here:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 113);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(179, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Please enter sensor type value here:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 87);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(184, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Please enter physical size value here:";
            // 
            // textBox_ParamArray
            // 
            this.textBox_ParamArray.Location = new System.Drawing.Point(219, 132);
            this.textBox_ParamArray.Name = "textBox_ParamArray";
            this.textBox_ParamArray.Size = new System.Drawing.Size(44, 20);
            this.textBox_ParamArray.TabIndex = 5;
            this.textBox_ParamArray.Text = "01";
            // 
            // textBox_PhysicalSize
            // 
            this.textBox_PhysicalSize.Location = new System.Drawing.Point(219, 80);
            this.textBox_PhysicalSize.Name = "textBox_PhysicalSize";
            this.textBox_PhysicalSize.Size = new System.Drawing.Size(44, 20);
            this.textBox_PhysicalSize.TabIndex = 4;
            this.textBox_PhysicalSize.Text = "05";
            // 
            // textBox_SensorType
            // 
            this.textBox_SensorType.Location = new System.Drawing.Point(219, 106);
            this.textBox_SensorType.Name = "textBox_SensorType";
            this.textBox_SensorType.Size = new System.Drawing.Size(44, 20);
            this.textBox_SensorType.TabIndex = 3;
            this.textBox_SensorType.Text = "00";
            // 
            // radioButton_ManSensor
            // 
            this.radioButton_ManSensor.AutoSize = true;
            this.radioButton_ManSensor.Location = new System.Drawing.Point(290, 28);
            this.radioButton_ManSensor.Name = "radioButton_ManSensor";
            this.radioButton_ManSensor.Size = new System.Drawing.Size(128, 17);
            this.radioButton_ManSensor.TabIndex = 2;
            this.radioButton_ManSensor.Text = "Manual  sensor select";
            this.radioButton_ManSensor.UseVisualStyleBackColor = true;
            // 
            // radioButton_AutoSensor
            // 
            this.radioButton_AutoSensor.AutoSize = true;
            this.radioButton_AutoSensor.Checked = true;
            this.radioButton_AutoSensor.Location = new System.Drawing.Point(17, 28);
            this.radioButton_AutoSensor.Name = "radioButton_AutoSensor";
            this.radioButton_AutoSensor.Size = new System.Drawing.Size(137, 17);
            this.radioButton_AutoSensor.TabIndex = 1;
            this.radioButton_AutoSensor.TabStop = true;
            this.radioButton_AutoSensor.Text = "Automatic sensor select";
            this.radioButton_AutoSensor.UseVisualStyleBackColor = true;
            this.radioButton_AutoSensor.CheckedChanged += new System.EventHandler(this.OnAutomaticChanged);
            // 
            // button_StartStop
            // 
            this.button_StartStop.Enabled = false;
            this.button_StartStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_StartStop.Location = new System.Drawing.Point(300, 63);
            this.button_StartStop.Name = "button_StartStop";
            this.button_StartStop.Size = new System.Drawing.Size(111, 89);
            this.button_StartStop.TabIndex = 0;
            this.button_StartStop.Text = "START";
            this.button_StartStop.UseVisualStyleBackColor = true;
            this.button_StartStop.Click += new System.EventHandler(this.button_StartStop_Click);
            // 
            // timer_Scan
            // 
            this.timer_Scan.Interval = 2000;
            this.timer_Scan.Tick += new System.EventHandler(this.timer_Scan_Tick);
            // 
            // timer_CheckReader
            // 
            this.timer_CheckReader.Interval = 500;
            this.timer_CheckReader.Tick += new System.EventHandler(this.timer_CheckReader_Tick);
            // 
            // panel_Info
            // 
            this.panel_Info.Controls.Add(this.panel_ReaderStatus);
            this.panel_Info.Controls.Add(this.textBox_ReaderID);
            this.panel_Info.Controls.Add(this.textBox_DriverVersion);
            this.panel_Info.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_Info.Location = new System.Drawing.Point(0, 267);
            this.panel_Info.Name = "panel_Info";
            this.panel_Info.Size = new System.Drawing.Size(430, 46);
            this.panel_Info.TabIndex = 4;
            // 
            // panel_ReaderStatus
            // 
            this.panel_ReaderStatus.Location = new System.Drawing.Point(223, 13);
            this.panel_ReaderStatus.Name = "panel_ReaderStatus";
            this.panel_ReaderStatus.Size = new System.Drawing.Size(20, 20);
            this.panel_ReaderStatus.TabIndex = 8;
            // 
            // textBox_ReaderID
            // 
            this.textBox_ReaderID.Location = new System.Drawing.Point(254, 12);
            this.textBox_ReaderID.Name = "textBox_ReaderID";
            this.textBox_ReaderID.ReadOnly = true;
            this.textBox_ReaderID.Size = new System.Drawing.Size(143, 20);
            this.textBox_ReaderID.TabIndex = 7;
            this.textBox_ReaderID.Text = "No Reader";
            // 
            // textBox_DriverVersion
            // 
            this.textBox_DriverVersion.Location = new System.Drawing.Point(13, 12);
            this.textBox_DriverVersion.Name = "textBox_DriverVersion";
            this.textBox_DriverVersion.ReadOnly = true;
            this.textBox_DriverVersion.Size = new System.Drawing.Size(174, 20);
            this.textBox_DriverVersion.TabIndex = 2;
            this.textBox_DriverVersion.Text = "No driver info yet...";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 537);
            this.Controls.Add(this.panel_Info);
            this.Controls.Add(this.panel_StartStop);
            this.Controls.Add(this.panel_Connection);
            this.Controls.Add(this.panel_Sensor);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "GetSensor Sample";
            this.panel_Sensor.ResumeLayout(false);
            this.panel_Sensor.PerformLayout();
            this.panel_Connection.ResumeLayout(false);
            this.panel_Connection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_PortNumber)).EndInit();
            this.panel_StartStop.ResumeLayout(false);
            this.panel_StartStop.PerformLayout();
            this.panel_Info.ResumeLayout(false);
            this.panel_Info.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_Sensor;
        private System.Windows.Forms.Label label_LastTime;
        private System.Windows.Forms.RichTextBox richTextBox_SensorValues;
        private System.Windows.Forms.Label label_SerialNumber;
        private System.Windows.Forms.Panel panel_Connection;
        private System.Windows.Forms.ComboBox comboBox_Protocol;
        private System.Windows.Forms.ComboBox comboBox_PortType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDown_PortNumber;
        private System.Windows.Forms.Panel panel_StartStop;
        private System.Windows.Forms.Button button_StartStop;
        private System.Windows.Forms.Timer timer_Scan;
        private System.Windows.Forms.Timer timer_CheckReader;
        private System.Windows.Forms.Button button_Initialize;
        private System.Windows.Forms.Panel panel_ResultSensor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_ParamArray;
        private System.Windows.Forms.TextBox textBox_PhysicalSize;
        private System.Windows.Forms.TextBox textBox_SensorType;
        private System.Windows.Forms.RadioButton radioButton_ManSensor;
        private System.Windows.Forms.RadioButton radioButton_AutoSensor;
        private System.Windows.Forms.Panel panel_Info;
        private System.Windows.Forms.Panel panel_ReaderStatus;
        private System.Windows.Forms.TextBox textBox_ReaderID;
        private System.Windows.Forms.TextBox textBox_DriverVersion;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
    }
}


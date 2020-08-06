using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SampleApp_CSharp_GetSensor
{
    public partial class Form1 : Form
    {
        RFIDClass Reader = null;

        public Form1()
        {
            InitializeComponent();

            comboBox_PortType.SelectedIndex = 2;
            comboBox_Protocol.SelectedIndex = 0;
            numericUpDown_PortNumber.Value = 1;

            OnAutomaticChanged(this, new EventArgs());
        }

        private void timer_CheckReader_Tick(object sender, EventArgs e)
        {
            if (Reader == null) 
                return;

            //Update Port state
            Reader.Reader_GetState();
            if ((Reader.Port_Initialized == 1) && (Reader.Reader_Connected == 1))
            {
                //Okay, Reader connected
                textBox_ReaderID.Text = "Reader ID: " + Reader.Reader_Id;
                panel_ReaderStatus.BackColor = Color.Green;
                panel_ResultSensor.BackColor = Color.Transparent;
                if ((Reader.Version_Main_Driver_Engine < 0x10) && (Reader.Version_Sub_Driver_Engine < 0x48)) 
                    return; //Not supported!
                if (!button_StartStop.Enabled) 
                    button_StartStop.Enabled = true;

                return;
            }
            else
            {
                panel_ReaderStatus.BackColor = Color.Yellow;
            }

            if (Reader.Port_Initialized == 0)
            {
                //Port not valid, try to re-initialize
                Reader.Reader_CloseInterface();
                Reader.Reader_OpenInterface();
                if (Reader.Port_Initialized == 1)
                {
                    //StatusBar_Main.Text = "Port re-initialized!";
                }
            }
            else
            {
                //Reader not detected, try to re-initialize
                //StatusBar_Main.Text = String.Format("No Reader connected, driver ver {0}.{1} ready.", Reader.Version_Main_Driver_Engine, Reader.Version_Sub_Driver_Engine);
            }
        }

        private void button_Initialize_Click(object sender, EventArgs e)
        {
            if (Reader == null)
            {
                Reader = new RFIDClass();
            }

            //Initialize variables
            Reader.PortName = "COM" + numericUpDown_PortNumber.Value + ":";
            Reader.ProtocolType = 3;
            Reader.InterfaceType = 1356;
            switch (comboBox_PortType.SelectedIndex)
            {
                case 0://Serial
                    Reader.PortType = 0;
                    break;
                case 1://BT
                    Reader.PortType = 2;
                    break;
                case 2://USB
                    Reader.PortType = 4;
                    break;
            }

            //Open communication interface
            Reader.Reader_OpenInterface();
            textBox_DriverVersion.Text = String.Format("Version: {0:X2}.{1:X2}", Reader.Version_Main_Driver_Engine, Reader.Version_Sub_Driver_Engine);
            
            if (Reader.Port_Initialized == 1)
            {
                numericUpDown_PortNumber.Enabled = false;
                comboBox_PortType.Enabled = false;
                timer_CheckReader.Enabled = true;
                (sender as Button).Enabled = false;
            }
        }

        private void button_StartStop_Click(object sender, EventArgs e)
        {
            //Get parameters from UI
            try
            {
                byte[] check = microsensys.DevTools.Converter.StringConverter.HexStringToByteArray(this.textBox_ParamArray.Text);
                byte bcheck = microsensys.DevTools.Converter.StringConverter.HexStringToByte(this.textBox_PhysicalSize.Text);
                bcheck = microsensys.DevTools.Converter.StringConverter.HexStringToByte(this.textBox_SensorType.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Error converting manual sensor values. Please enter hexadecimal bytes/Byte array!");
                return;
            }
            
            if (timer_CheckReader.Enabled)
            {
                //If stopped --> START
                timer_CheckReader.Enabled = false;
                timer_Scan.Enabled = true;
                label_LastTime.Text = "";
                label_SerialNumber.Text = "";
                richTextBox_SensorValues.Text = "";
                button_StartStop.Text = "STOP";
            }
            else
            {
                //If started --> STOP
                timer_CheckReader.Enabled = true;
                timer_Scan.Enabled = false;
                button_StartStop.Text = "START";
            }
        }

        private void timer_Scan_Tick(object sender, EventArgs e)
        {
            //Initialize variables
            uint serNum = 0;
            byte pc = 0;
            byte vc = 0;
            byte numSizes = 0;
            byte[] sizes = new byte[10];
            byte[] xtraData = new byte[32];
            int result = 0xFF;

            //Get iID-L Sensor Information
            //  Reader.Port_Handle -> Communication handle
            //  0 -> parameter (Reserved)
            //  0 -> driver parameter (Reserved)
            //  serNum -> pointer where the TELID Serial number will be load
            //  pc -> pointer where the TELID Product code will be load
            //  vc -> pointer where the TELID Version code will be load
            //  numSizes -> number of physical sizes the TELID can read
            //  sizes -> type of physical sizes the TELID can read
            //  xtraData -> pointer where additional sensor data will be load (needed for "get_sensor_data")
            result = RFIDClass.iidl_c_get_sensor_information(Reader.Port_Handle, 0, 0, ref serNum, ref pc, ref vc, ref numSizes, sizes, xtraData);
            if (result == 0)
            {
                //Result OK
                label_SerialNumber.Text = String.Format("{0}", serNum);
                byte numSensor = 0;
                float[] sensorValues = new float[10];

                if (this.radioButton_AutoSensor.Checked)
                {
                    //Get iID-L Sensor Data
                    //  Reader.Port_Handle -> Communication handle
                    //  0 -> parameter (Reserved)
                    //  0 -> driver parameter (Reserved)
                    //  serNum -> pointer where the TELID Serial number will be load
                    //  pc -> pointer where the TELID Product code will be load
                    //  vc -> pointer where the TELID Version code will be load
                    //  numSizes -> number of physical sizes the TELID can read
                    //  sizes -> type of physical sizes the TELID can read
                    //  xtraData -> pointer where additional sensor data will be load (needed for "get_sensor_data")
                    //  numSensor -> number of sensor values calculated by the function (size of "sensorValues")
                    //  sensorValues -> sensor values as float
                    result = RFIDClass.iidl_c_get_sensor_data(Reader.Port_Handle, 0, 0, serNum, pc, vc, ref numSizes, sizes, xtraData, ref numSensor, sensorValues);
                }
                else
                {
                    //Get parameters from UI
                    byte bSensorType = microsensys.DevTools.Converter.StringConverter.HexStringToByte(this.textBox_SensorType.Text);
                    byte bPhysicalSize = microsensys.DevTools.Converter.StringConverter.HexStringToByte(this.textBox_PhysicalSize.Text);
                    byte[] bParams = microsensys.DevTools.Converter.StringConverter.HexStringToByteArray(this.textBox_ParamArray.Text);
                    byte bParamLength = Convert.ToByte(bParams.GetLength(0));

                    //Get iID-L Sensor Data
                    //  Reader.Port_Handle -> Communication handle
                    //  0 -> driver parameter (Reserved)
                    //  bPhysicalSize -> physical size to read
                    //  bParamLength -> length of parameters
                    //  bParams -> parameters
                    //  serNum -> pointer where the TELID Serial number will be load
                    //  pc -> pointer where the TELID Product code will be load
                    //  vc -> pointer where the TELID Version code will be load
                    //  numSizes -> number of physical sizes the TELID can read
                    //  sizes -> type of physical sizes the TELID can read
                    //  xtraData -> pointer where additional sensor data will be load (needed for "get_sensor_data")
                    //  numSensor -> number of sensor values calculated by the function (size of "sensorValues")
                    //  sensorValues -> sensor values as float
                    result = RFIDClass.iidl_c_get_sensor_data_extended(Reader.Port_Handle, 0, bPhysicalSize, bSensorType, bParamLength, bParams, serNum, pc, vc, ref numSizes, sizes, xtraData, ref numSensor, sensorValues);
                }

                Console.WriteLine("Sensor Data result" + result);
                if (result == 0)
                {
                    //Result OK
                    label_LastTime.Text = String.Format("{0:T}", DateTime.Now);
                    richTextBox_SensorValues.Text = "";
                    if (numSensor != numSizes) return;

                    //Write each sensor value into UI
                    for (int i = 0; i < numSensor; i++)
                    {
                        richTextBox_SensorValues.Text += String.Format("{0} {1}\n", sensorValues[i], GetUnit(sizes[i]));
                    }
                    panel_ResultSensor.BackColor = Color.Green;
                    return;
                }
            }
            panel_ResultSensor.BackColor = Color.Red;
        }

        private string GetUnit(byte _size)
        {
            switch (_size)
            {
                case 1:
                    return "°C";
                case 3:
                    return "%";
                case 4:
                    return "mbar";
                case 5:
                    return "V";
                case 6:
                    return "Ohm";
                case 8:
                    return "g";
                default:
                    return "";
            }
        }

        private void OnAutomaticChanged(object sender, EventArgs e)
        {
            if (this.radioButton_AutoSensor.Checked)
            {
                this.textBox_ParamArray.Enabled = false;
                this.textBox_PhysicalSize.Enabled = false;
                this.textBox_SensorType.Enabled = false;
            }
            else
            {
                this.textBox_ParamArray.Enabled = true;
                this.textBox_PhysicalSize.Enabled = true;
                this.textBox_SensorType.Enabled = true;
            }
        }
    }
}

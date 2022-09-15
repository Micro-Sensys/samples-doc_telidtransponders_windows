using iIDReaderLibrary;
using iIDReaderLibrary.Utils;
using System;
using System.ComponentModel;
using System.Windows;

namespace SampleAsync_CSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BackgroundWorker m_Worker = new BackgroundWorker();
        private bool m_ReaderFound = false;

        DocInterfaceControl m_DocInterface = null;

        public MainWindow()
        {
            InitializeComponent();

            SetUiEnabled(false, 0);
            m_Worker.WorkerReportsProgress = true;
            m_Worker.WorkerSupportsCancellation = true;
            m_Worker.DoWork += Worker_DoWorkAsync;

            textBlockInitialize_DriverVersion.Text = DocInterfaceControl.LibraryVersion;
            textBlock_ReaderInfo.Text = "Library version: " + textBlockInitialize_DriverVersion.Text + "  - Waiting for Initialize";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Stop background worker
            m_Worker.CancelAsync();
            if (m_DocInterface != null)
            {
                m_DocInterface.Terminate();
            }
        }

        private void SetUiEnabled(bool _enabled, int _readerID)
        {
            Dispatcher.Invoke(() =>
            {
                tabItem_T200.IsEnabled = _enabled;

                if (_enabled)
                {
                    if (_readerID > 0)
                        textBlock_ReaderInfo.Text = "ReaderID: " + _readerID;
                }
                else
                {
                    tabControl.SelectedIndex = 0;
                    textBlock_ReaderInfo.Text = "Communication lost/not possible  - Waiting for Initialize";
                    textBlock_Status.Text = "";
                }
            });
        }

        #region Initialize
        private async void ButtonInitialize_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (m_DocInterface != null)
            {
                //First dispose previous instance
                m_DocInterface.Terminate();
                m_DocInterface = null;
            }
            //Get Interface parameters and initialize class
            try
            {
                //Port type --> Get from UI
                //  0 = Serial
                //  2 = Bluetooth
                //  4 = USB
                byte portType = 4; //Default USB
                if (radioButtonInitialize_PortSerial.IsChecked.Value)
                    portType = 0;
                if (radioButtonInitialize_PortBt.IsChecked.Value)
                    portType = 2;

                var readerPortSettings = InterfaceCommunicationSettings.GetForSerialDevice(portType, textBoxInitialize_PortName.Text);
                //Interface Type --> 1356 = 13.56MHz (HF)

                //Initialize class. Then call "initialize"
                m_DocInterface = new DocInterfaceControl(readerPortSettings, 1356);

                //Initialize
                textBlockInitialize_ParamInterfaceType.Text = "InterfaceType: 1356";
                textBlockInitialize_ParamPortType.Text = "PortType: " + portType;
                textBlockInitialize_ParamPortName.Text = "PortName: " + textBoxInitialize_PortName.Text;

                textBlock_ReaderInfo.Text = "Calling Initialize";
                if (await m_DocInterface.InitializeAsync())
                {
                    //Initialize worked --> Enable UI & enable BackgroundWorker to check Reader-ID
                    textBlock_ReaderInfo.Text = "Initialize Result: True";
                    if (m_Worker.IsBusy != true)
                    {
                        // Start the asynchronous operation.
                        m_Worker.RunWorkerAsync();
                    }
                }
                else
                {
                    //Initialize didn't work --> disable UI
                    SetUiEnabled(false, 0);
                    textBlock_ReaderInfo.Text = "Initialize Result: False";
                }
            }
            catch
            {
                //TODO catch exception & notify
            }
        }

        private void ButtonTerminate_Click(object sender, RoutedEventArgs e)
        {
            //Stop background worker
            m_Worker.CancelAsync();

            if (m_DocInterface != null)
            {
                m_DocInterface.Terminate();
                //m_DocInterface = null;
            }
            m_ReaderFound = false;
            SetUiEnabled(false, 0);
        }
        private async void Worker_DoWorkAsync(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            int readerCheckFailCount = 0;
            //Check readerID in 5 seconds interval
            TimeSpan readerCheckSpan = TimeSpan.FromSeconds(5);
            //Initialize lastChecked in the past --> when Worker started it should check for the ReaderID first of all
            DateTime lastCheckedOk = DateTime.UtcNow.AddMinutes(-1);

            //While port initialized:
            //  Check reader communication is still possible in 5 seconds interval
            //      Hint: This interval is not fixed! It is recomended to check the communication with the reader when no other operation is executed.
            //          => Most important for battery powered devices!
            while (m_DocInterface.IsInitialized)
            {
                if (worker.CancellationPending == true)
                {
                    //Exit loop if Background worker is cancelled
                    e.Cancel = true;
                    break;
                }
                else
                {
                    if ((DateTime.UtcNow - lastCheckedOk) < readerCheckSpan)
                    {
                        //Next check time still not reached --> just do nothing
                        System.Threading.Thread.Sleep(200);
                        continue;
                    }
                    else
                    {
                        //Next check time reached --> check ReaderID

                        var readerInfo = await m_DocInterface.ReadReaderIDAsync();
                        if (readerInfo != null)
                        {
                            //ReaderID check OK
                            readerCheckFailCount = 0;
                            lastCheckedOk = DateTime.UtcNow;
                            if (!m_ReaderFound)
                            {
                                //Not previously found --> Enable functions
                                m_ReaderFound = true;
                                SetUiEnabled(true, readerInfo.ReaderID); //TODO show more info? Also HW info?
                            }
                        }
                        else
                        {
                            //ReaderID check failed
                            readerCheckFailCount++;
                            if (readerCheckFailCount > 5)
                            {
                                //Reader Check failed multiple times
                                if (m_ReaderFound)
                                {
                                    //Previously found --> Asume Reader is lost!
                                    m_ReaderFound = false;
                                    SetUiEnabled(false, 0);
                                    return;
                                }
                            }
                            System.Threading.Thread.Sleep(200);
                        }
                    }
                }
            }
        }
        #endregion

        private async void Button_GetSensor_ClickAsync(object sender, RoutedEventArgs e)
        {
            //GetSensor --> Search for TELID® transponders 
            if (m_DocInterface != null)
            {
                if (m_DocInterface.IsInitialized)
                {
                    DateTime startTime = DateTime.UtcNow;
                    //This function blocks & searches for a default time of 1 second (optional parameter)
                    try
                    {
                        textBox_ThreadLog.Text += "\n = GetSensor =\n";
                        var getSensorResult = await m_DocInterface.GetSensorDataAsync(0xFC); //0xFC = scan all TELID® types
                        TimeSpan processSpan = DateTime.UtcNow - startTime;
                        if (getSensorResult != null)
                        {
                            textBox_Result.Text = "- TELID® found -\n";
                            textBox_Result.Text += string.Format("SerNo: {0}\n", getSensorResult.SerialNumber);
                            textBox_Result.Text += string.Format("Description: {0}\n", getSensorResult.Description);
                            textBox_Result.Text += "Measurements:\n";
                            foreach (var meas in getSensorResult.Measurements)
                            {
                                textBox_Result.Text += string.Format("\tTimestamp: {0}\n", meas.Timestamp);
                                textBox_Result.Text += "\tValues:\n";
                                foreach (var value in meas.Values)
                                {
                                    textBox_Result.Text += string.Format("\t  {0}{1}{2}\n", value.Symbol, value.Magnitude, value.Unit);
                                }
                            }
                            string toLog = string.Format("Result: OK. Duration: {0}\n", processSpan);
                            toLog += "- Sensor Result -\n";
                            toLog += string.Format("\tSerNo: {0}\n", getSensorResult.SerialNumber);
                            toLog += string.Format("\tNumMeas: {0}\n", getSensorResult.Measurements.Length);
                            textBox_ThreadLog.Text += toLog;
                            textBox_ThreadLog.ScrollToEnd();
                        }
                        else
                        {
                            //Update result in UI
                            textBox_ThreadLog.Text += string.Format("Result: FAIL. Duration: {0}\n", processSpan);
                            textBox_ThreadLog.ScrollToEnd();
                        }
                    }
                    catch (Exception ex)
                    {
                        TimeSpan processSpan = DateTime.UtcNow - startTime;
                        textBox_ThreadLog.Text += string.Format("Result: Exception. Duration: {0}\n", processSpan);
                        textBox_ThreadLog.ScrollToEnd();
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }
            }
        }
    }
}

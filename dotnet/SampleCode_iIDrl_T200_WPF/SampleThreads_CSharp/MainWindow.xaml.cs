using iIDReaderLibrary;
using iIDReaderLibrary.Utils;
using System;
using System.ComponentModel;
using System.Windows;

namespace SampleThreads_CSharp
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
            m_Worker.DoWork += Worker_DoWork;

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
        private void ButtonInitialize_Click(object sender, RoutedEventArgs e)
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

                m_DocInterface.InitializeCompleted += DocInterface_InitializeCompleted;
                m_DocInterface.DocResultChanged += DocInterface_DocResultChanged;
                textBlock_ReaderInfo.Text = "Calling Initialize";
                m_DocInterface.StartInitialize();
            }
            catch
            {
                //TODO catch exception & notify
            }
        }

        private void DocInterface_InitializeCompleted(object _sender, bool _portOpen)
        {
            Dispatcher.Invoke(() =>
            {
                if (_portOpen)
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
            });
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
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
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

                        var readerInfo = m_DocInterface.ReadReaderID();
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

        DateTime mLastDocResultTimestamp;
        private void DocInterface_DocResultChanged(object sender, DocResultEventArgs _docResult)
        {
            if (_docResult != null)
            {
                TimeSpan span = DateTime.Now - mLastDocResultTimestamp;
                if (_docResult.ResultInfo != null)
                {
                    if (_docResult.ResultInfo is TELIDSensorResultInfo getSensorResult)
                    {
                        //Result to GetSensorData
                        Dispatcher.Invoke(() =>
                        {
                            //Update result in UI - For demo purposes done using "Dispatcher.Invoke"
                            string toLog = string.Format("{0} (Duration: {1})\n", _docResult.Timestamp, span);
                            toLog += "- TELID SensorResult -\n";
                            if (getSensorResult != null)
                            {
                                toLog += string.Format("SerNo: {0}\n", getSensorResult.SerialNumber);
                                toLog += string.Format("Description: {0}\n", getSensorResult.Description);
                                toLog += "Measurements:\n";
                                foreach (var meas in getSensorResult.Measurements)
                                {
                                    toLog += string.Format("\tTimestamp: {0}\n", meas.Timestamp);
                                    toLog += "\tValues:\n";
                                    foreach (var value in meas.Values)
                                    {
                                        toLog += string.Format("\t  {0}{1}{2}\n", value.Symbol, value.Magnitude, value.Unit);
                                    }
                                }
                                toLog += "\n";
                                textBox_Result.Text += toLog;
                                textBox_Result.ScrollToEnd();

                                toLog = string.Format("Result: OK. Duration: {0}\n", span);
                                toLog += "- Sensor Result -\n";
                                toLog += string.Format("\tSerNo: {0}\n", getSensorResult.SerialNumber);
                                toLog += string.Format("\tNumMeas: {0}\n", getSensorResult.Measurements.Length);
                                textBox_ThreadLog.Text += toLog;
                                textBox_ThreadLog.ScrollToEnd();
                            }
                        });
                    }
                }
                else
                {
                    if (_docResult.ProcessFinished)
                    {
                        ThreadProcessFinished();
                        return;
                    }
                    Dispatcher.Invoke(() =>
                    {
                        textBox_ThreadLog.Text += string.Format("{0} (Duration: {1})\n ResultInfo = NULL\n", _docResult.Timestamp, span);
                        textBox_ThreadLog.ScrollToEnd();
                    });
                }
                if (_docResult.ResultException != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        textBox_ThreadLog.Text += string.Format("{0} (Duration: {1})\n Result: Exception\n", _docResult.Timestamp, span);
                        textBox_ThreadLog.ScrollToEnd();
                    });
                    System.Diagnostics.Debug.WriteLine(_docResult.ResultException.ToString());
                }

                mLastDocResultTimestamp = DateTime.Now;
            }
        }

        private void ThreadProcessFinished()
        {
            Dispatcher.Invoke(() =>
            {
                progressBar.IsIndeterminate = false;
            });
        }

        private void Button_GetSensor_Click(object sender, RoutedEventArgs e)
        {
            //GetSensor --> Search for TELID® transponders 
            if (m_DocInterface != null)
            {
                if (m_DocInterface.IsInitialized)
                {
                    //This function starts the "GetSensorData" process in a new thread, and reports the result using "DocResultChanged" Event
                    /*
                     * Parameters:
                     *  _repeatCount --> number of times "Identify" will be executed internally
                     *  _delayBetweenSearchMs --> number of milliseconds to wait between internal calls to "Identify"
                     *  _notifySuccessOnly --> if true, "DocResultChanged" Event will only be raised by success on internal "Identify" calls
                     */
                    m_DocInterface.StartGetSensorData(0xFC, 5, 0, false);
                    textBox_ThreadLog.Text += "\n = StartGetSensorData =\n";
                    textBox_ThreadLog.ScrollToEnd();
                    textBox_Result.Text = "";
                    progressBar.IsIndeterminate = true;
                    mLastDocResultTimestamp = DateTime.Now;
                }
            }
        }
    }
}

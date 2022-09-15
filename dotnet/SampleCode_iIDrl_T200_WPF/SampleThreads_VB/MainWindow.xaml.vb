Imports System.ComponentModel
Imports iIDReaderLibrary
Imports iIDReaderLibrary.Utils

Class MainWindow

    Private m_Worker As BackgroundWorker = New BackgroundWorker()
    Private m_ReaderFound As Boolean

    Private m_DocInterface As DocInterfaceControl = Nothing

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        SetUiEnabled(False, 0)
        m_Worker.WorkerReportsProgress = True
        m_Worker.WorkerSupportsCancellation = True
        AddHandler m_Worker.DoWork, AddressOf Worker_DoWork

        textBlockInitialize_DriverVersion.Text = DocInterfaceControl.LibraryVersion
        textBlock_ReaderInfo.Text = "Library version: " + textBlockInitialize_DriverVersion.Text + "  - Waiting for Initialize"
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        'Stop background worker
        m_Worker.CancelAsync()
        If m_DocInterface IsNot Nothing Then
            m_DocInterface.Terminate()
        End If
    End Sub

    Private Sub SetUiEnabled(_enabled As Boolean, _readerID As Integer)
        Dispatcher.Invoke(Sub()
                              tabItem_T200.IsEnabled = _enabled

                              If _enabled Then
                                  If _readerID > 0 Then
                                      textBlock_ReaderInfo.Text = String.Format("ReaderID: {0}", _readerID)
                                  End If
                              Else
                                  tabControl.SelectedIndex = 0
                                  textBlock_ReaderInfo.Text = "Communication lost/not possible  - Waiting for Initialize"
                                  textBlock_Status.Text = ""
                              End If
                          End Sub)
    End Sub

#Region "Initialize"
    Private Sub ButtonInitialize_Click(sender As Object, e As RoutedEventArgs)
        If m_DocInterface IsNot Nothing Then
            'First dispose previous instance
            m_DocInterface.Terminate()
            m_DocInterface = Nothing
        End If
        'Get Interface parameters and initialize class
        Try
            'Port type --> Get from UI
            '   0 = Serial
            '   2 = Bluetooth
            '   4 = USB
            Dim portType As Byte = 4 'Default USB
            If radioButtonInitialize_PortSerial.IsChecked.Value Then
                portType = 0
            End If
            If radioButtonInitialize_PortBt.IsChecked.Value Then
                portType = 2
            End If

            Dim readerPortSettings = InterfaceCommunicationSettings.GetForSerialDevice(portType, textBoxInitialize_PortName.Text)

            'Initialize class. Then call "Initialize"
            m_DocInterface = New DocInterfaceControl(readerPortSettings, 1356)

            'Initialize
            textBlockInitialize_ParamInterfaceType.Text = "InterfaceType: 1356"
            textBlockInitialize_ParamPortType.Text = "PortType: " + portType.ToString()
            textBlockInitialize_ParamPortName.Text = "PortName: " + textBoxInitialize_PortName.Text

            AddHandler m_DocInterface.InitializeCompleted, AddressOf DocInterface_InitializeCompleted
            AddHandler m_DocInterface.DocResultChanged, AddressOf DocInterface_DocResultChanged
            textBlock_ReaderInfo.Text = "Calling Initialize"
            m_DocInterface.StartInitialize()
        Catch ex As Exception
            'TODO catch exception & notify
        End Try
    End Sub

    Private Sub DocInterface_InitializeCompleted(_sender As Object, _portOpen As Boolean)
        Dispatcher.Invoke(Sub()
                              If _portOpen Then
                                  'Initialize worked --> Enable UI & enable BackgroundWorker to check Reader-ID
                                  textBlock_ReaderInfo.Text = "Initialize Result: True"
                                  If Not m_Worker.IsBusy Then
                                      'Start the asynchronous operation
                                      m_Worker.RunWorkerAsync()
                                  End If
                              Else
                                  'Initialize didn't work --> disable UI
                                  SetUiEnabled(False, 0)
                                  textBlock_ReaderInfo.Text = "Initialize Result: False"
                              End If
                          End Sub)
    End Sub

    Private Sub ButtonTerminate_Click(sender As Object, e As RoutedEventArgs)
        'Stop background worker
        m_Worker.CancelAsync()

        If m_DocInterface IsNot Nothing Then
            m_DocInterface.Terminate()
        End If
        m_ReaderFound = False
        SetUiEnabled(False, 0)
    End Sub

    Private Sub Worker_DoWork(sender As Object, e As DoWorkEventArgs)
        Dim worker As BackgroundWorker = sender
        Dim readerCheckFailCount As Integer = 0
        'Check readerID in 2 seconds interval
        Dim readerCheckSpan As TimeSpan = TimeSpan.FromSeconds(5)
        'Initialize lastChecked in the past --> when Worker started it should check for the ReaderID first of all
        Dim lastCheckedOk As DateTime = DateTime.UtcNow.AddMinutes(-1)

        'While port initialized
        '  Check reader communication Is still possible in 5 seconds interval
        '      Hint: This interval Is Not fixed! It Is recomended to check the communication with the reader when no other operation Is executed.
        '=> Most important for battery powered devices!
        While (m_DocInterface.IsInitialized)

            If (worker.CancellationPending) Then
                'Exit loop if Background worker Is cancelled
                e.Cancel = True
                Exit While
            Else

                If ((DateTime.UtcNow - lastCheckedOk) < readerCheckSpan) Then

                    'Next check time still Not reached --> just do nothing
                    Threading.Thread.Sleep(200)
                    Continue While

                Else

                    'Next check time reached --> check ReaderID

                    Dim readerInfo = m_DocInterface.ReadReaderID()
                    If readerInfo IsNot Nothing Then
                        'ReaderID check OK
                        readerCheckFailCount = 0
                        lastCheckedOk = DateTime.UtcNow
                        If Not m_ReaderFound Then
                            'Not previously found --> Enable functions
                            m_ReaderFound = True
                            SetUiEnabled(True, readerInfo.ReaderID)
                        End If
                    Else
                        'ReaderID check failed
                        readerCheckFailCount += 1
                        If readerCheckFailCount > 5 Then
                            'Reader Check failed multiple times
                            If m_ReaderFound Then
                                'Previously found --> assume reader is lost!
                                m_ReaderFound = False
                                SetUiEnabled(False, 0)
                                Return
                            End If
                        End If
                        Threading.Thread.Sleep(200)
                    End If
                End If
            End If
        End While
    End Sub
#End Region

    Dim mLastDocResultTimestamp As DateTime
    Private Sub DocInterface_DocResultChanged(sender As Object, _docResult As DocResultEventArgs)
        If _docResult IsNot Nothing Then
            Dim span = DateTime.Now - mLastDocResultTimestamp
            If _docResult.ResultInfo IsNot Nothing Then
                If TypeOf _docResult.ResultInfo Is TELIDSensorResultInfo Then
                    'Result to GetSensorData
                    Dim telidResult As TELIDSensorResultInfo = _docResult.ResultInfo
                    Dispatcher.Invoke(Sub()
                                          ' Update result in UI - For demo purposes done using "Dispatcher.Invoke"
                                          Dim toLog As String = String.Format("{0} (Duration: {1})", _docResult.Timestamp, span) + Environment.NewLine
                                          toLog += "- TELID SensorResult -" + Environment.NewLine
                                          If telidResult IsNot Nothing Then
                                              toLog += String.Format("SerNo: {0}", telidResult.SerialNumber) + Environment.NewLine
                                              toLog += String.Format("Description: {0}", telidResult.Description) + Environment.NewLine
                                              toLog += "Measurements:\n"
                                              For Each meas As SensorMeasurement In telidResult.Measurements
                                                  toLog += vbTab + String.Format("Timestamp: {0}", meas.Timestamp) + Environment.NewLine
                                                  toLog += vbTab + "Values:" + Environment.NewLine
                                                  For Each value As SensorValue In meas.Values
                                                      toLog += vbTab + String.Format("  {0}{1}{2}", value.Symbol, value.Magnitude, value.Unit) + Environment.NewLine
                                                  Next
                                              Next
                                              toLog += Environment.NewLine
                                              textBox_Result.Text += toLog
                                              textBox_Result.ScrollToEnd()

                                              toLog = String.Format("Result: OK. Duration: {0}", span) + Environment.NewLine
                                              toLog += "- Sensor Result -" + Environment.NewLine
                                              toLog += vbTab + String.Format("SerNo: {0}", telidResult.SerialNumber) + Environment.NewLine
                                              toLog += vbTab + String.Format("NumMeas: {0}", telidResult.Measurements.Length) + Environment.NewLine
                                              textBox_ThreadLog.Text += toLog
                                              textBox_ThreadLog.ScrollToEnd()
                                          End If
                                      End Sub)
                End If
            Else
                If _docResult.ProcessFinished Then
                    ThreadProcessFinished()
                    Exit Sub
                End If
                Dispatcher.Invoke(Sub()
                                      textBox_ThreadLog.Text += String.Format("{0} (Duration: {1}{2} ResultInfo = NULL{2}", _docResult.Timestamp, span, Environment.NewLine)
                                      textBox_ThreadLog.ScrollToEnd()
                                  End Sub)
            End If
            If _docResult.ResultException IsNot Nothing Then
                Dispatcher.Invoke(Sub()
                                      textBox_ThreadLog.Text += String.Format("{0} (Duration: {1}{2} Result: Exception{2}", _docResult.Timestamp, span, Environment.NewLine)
                                      textBox_ThreadLog.ScrollToEnd()
                                  End Sub)
                System.Diagnostics.Debug.WriteLine(_docResult.ResultException.ToString())
            End If
        End If
    End Sub

    Private Sub ThreadProcessFinished()
        Dispatcher.Invoke(Sub()
                              progressBarThread.IsIndeterminate = False
                          End Sub)
    End Sub

    Private Sub Button_GetSensor_Click(sender As Object, e As RoutedEventArgs)
        'GetSensor --> Search for TELID® transponders 
        If m_DocInterface IsNot Nothing Then
            If m_DocInterface.IsInitialized Then
                'This function starts the "GetSensorData" process in a New thread, And reports the result using "DocResultChanged" Event
                '
                '   Parameters:
                '   _repeatCount --> number of times "Identify" will be executed internally
                '   _delayBetweenSearchMs --> Number of milliseconds to wait between internal calls to "Identify"
                '   _notifySuccessOnly --> If true, "DocResultChanged" Event will only be raised by success on internal "Identify" calls
                m_DocInterface.StartGetSensorData(&HFC, 5, 0, False)
                textBox_Result.Text = ""
                textBox_ThreadLog.Text += "\n = StartGetSensorData =\n"
                textBox_ThreadLog.ScrollToEnd()
                progressBarThread.IsIndeterminate = True
                mLastDocResultTimestamp = DateTime.Now
            End If
        End If
    End Sub
End Class

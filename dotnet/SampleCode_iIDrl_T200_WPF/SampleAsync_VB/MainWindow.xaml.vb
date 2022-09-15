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
        AddHandler m_Worker.DoWork, AddressOf Worker_DoWorkAsync

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
    Private Async Sub ButtonInitialize_ClickAsync(sender As Object, e As RoutedEventArgs)
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

            textBlock_ReaderInfo.Text = "Calling Initialize"
            If Await m_DocInterface.InitializeAsync() Then
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
        Catch ex As Exception
            'TODO catch exception & notify
        End Try
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

    Private Async Sub Worker_DoWorkAsync(sender As Object, e As DoWorkEventArgs)
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

                    Dim readerInfo = Await m_DocInterface.ReadReaderIDAsync()
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

    Private Async Sub Button_GetSensor_ClickAsync(sender As Object, e As RoutedEventArgs)
        'GetSensor --> Search for TELID® transponders 
        If m_DocInterface IsNot Nothing Then
            If m_DocInterface.IsInitialized Then
                Dim startTime = DateTime.UtcNow
                'This function blocks & searches for a default time of 1 second (optional parameter)
                Try
                    textBox_ThreadLog.Text += Environment.NewLine + " = GetSensor =" + Environment.NewLine
                    Dim getSensorResult = Await m_DocInterface.GetSensorDataAsync(&HFC) '0xFC = scan all TELID® types
                    Dim processSpan = DateTime.UtcNow - startTime
                    If getSensorResult IsNot Nothing Then
                        textBox_Result.Text = "- TELID® found -" + Environment.NewLine
                        textBox_Result.Text += String.Format("SerNo: {0}", getSensorResult.SerialNumber) + Environment.NewLine
                        textBox_Result.Text += String.Format("Description: {0}", getSensorResult.Description) + Environment.NewLine
                        textBox_Result.Text += "Measurements:" + Environment.NewLine
                        For Each meas As SensorMeasurement In getSensorResult.Measurements
                            textBox_Result.Text += vbTab + String.Format("Timestamp: {0}", meas.Timestamp) + Environment.NewLine
                            textBox_Result.Text += vbTab + "Values:" + Environment.NewLine
                            For Each value As SensorValue In meas.Values
                                textBox_Result.Text += vbTab + String.Format("  {0}{1}{2}", value.Symbol, value.Magnitude, value.Unit) + Environment.NewLine
                            Next
                        Next
                        Dim toLog As String = String.Format("Result: OK. Duration: {0}", processSpan) + Environment.NewLine
                        toLog += "- Sensor Result -" + Environment.NewLine
                        toLog += vbTab + String.Format("SerNo: {0}", getSensorResult.SerialNumber) + Environment.NewLine
                        toLog += vbTab + String.Format("NumMeas: {0}", getSensorResult.Measurements.Length) + Environment.NewLine
                        textBox_ThreadLog.Text += toLog
                        textBox_ThreadLog.ScrollToEnd()
                    Else
                        'Update result in UI
                        textBox_ThreadLog.Text += String.Format("Result: FAIL. Duration: {0}", processSpan) + Environment.NewLine
                        textBox_ThreadLog.ScrollToEnd()
                    End If
                Catch ex As Exception
                    Dim processSpan = DateTime.UtcNow - startTime
                    textBox_ThreadLog.Text += String.Format("Result: Exception. Duration: {0}", processSpan) + Environment.NewLine
                    textBox_ThreadLog.ScrollToEnd()
                    System.Diagnostics.Debug.WriteLine(ex.ToString())
                End Try
            End If
        End If
    End Sub
End Class

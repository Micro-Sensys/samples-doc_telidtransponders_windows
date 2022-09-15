Imports System
Imports System.Threading
Imports iIDReaderLibrary
Imports iIDReaderLibrary.Utils

Module Program
    '
    ' SampleThreads_VB
    '       SampleCode for iIDReaderLibrary.DocInterfaceControl
    '       Implemented in VB
    '       Using "Start..." functions
    '
    ' This sample demonstrates how to call the DocInterfaceControl functions that run the process in a separate new thread.
    ' This is only for demo purposes. For a Console application is not efficient to work in this way.

    Sub Main(args As String())
        Console.WriteLine(".NETCore Console")
        Console.WriteLine("SampleThreads_VB")
        Console.WriteLine("--------------------")
        Console.WriteLine("Library Version: " + DocInterfaceControl.LibraryVersion)

        'Get DocInterfaceControl instance
        Dim docIntControl As DocInterfaceControl = Console_InitializeDocInterfaceControl()
        If docIntControl IsNot Nothing Then
            'DocInterfaceControl is initialized
            Console.WriteLine("")
            Console.Write("Detecting reader..")
            While True
                'First of all, get the Reader Information
                Console.Write(".")
                Dim readerID = docIntControl.ReadReaderID()
                If readerID IsNot Nothing Then
                    Console.WriteLine("")
                    Console.WriteLine("Detected Reader:")
                    Console.WriteLine(readerID.ToString())
                    Exit While
                End If
            End While

            'Reader info obtained --> execute functions using menu
            Console.WriteLine("")
            While Console_ExecuteAndContinue(docIntControl)
                Thread.Sleep(500)
            End While

            docIntControl.Terminate()
            Console.WriteLine("")
            Console.Write("EXITING in 5")
            Thread.Sleep(1000)
            Console.Write(", 4")
            Thread.Sleep(1000)
            Console.Write(", 3")
            Thread.Sleep(1000)
            Console.Write(", 2")
            Thread.Sleep(1000)
            Console.Write(", 1")
            Thread.Sleep(1000)
        End If
    End Sub

    Dim initializeCompleted As Boolean = False
    Private Function Console_InitializeDocInterfaceControl() As DocInterfaceControl
        Console.WriteLine("== Select initialize parameters ==")
        'Get PortType
        Dim portType As Integer = Console_InitializePortType()
        Dim portName = ""
        Select Case portType
            Case 0, 2
                'For serial & Bluetooth, PortName needed.
                portName = Console_InitializePortName()
        End Select
        'Initialize InterfaceCommunicationSettings class
        Dim readerPortSettings = InterfaceCommunicationSettings.GetForSerialDevice(portType, portName)
        'InterfaceType = 13.56MHz for TELID®200
        Dim interfaceType As Integer = 1356

        'Parameters selected --> Initialize class instance
        Console.WriteLine("")
        Dim result = New DocInterfaceControl(readerPortSettings, interfaceType)
        Console.WriteLine(String.Format("Selected parameters: PortType: {0} | PortName: {1} | IntType: {2}", portType, portName, interfaceType))

        'Call initialize to open the communication port
        AddHandler result.InitializeCompleted, AddressOf DocInterfaceControl_InitializeCompleted
        result.StartInitialize()
        Console.Write("Initializing...")
        'For demo purposes, just wait blocking execution until "Initialize" process is completed (notified using "InitializeCompleted" event
        While Not initializeCompleted
            Thread.Sleep(100)
            Console.Write(".")
        End While
        Console.WriteLine("")
        If result.IsInitialized Then
            Console.WriteLine(vbTab + "Initialized")
            AddHandler result.DocResultChanged, AddressOf DocInterfaceControl_DocResultChanged
            Return result
        Else
            'Iniitalization failed: Terminate class instance & retry
            Console.WriteLine(vbTab + "Initialize failed")
            result.Terminate()
            Return Console_InitializeDocInterfaceControl()
        End If
    End Function

    Private Sub DocInterfaceControl_InitializeCompleted(_sender As Object, _portOpen As Boolean)
        ' using "_portOpen" the result of the operation can be checked
        initializeCompleted = True
    End Sub

    Private Function Console_InitializePortType() As Integer
        Console.WriteLine("Port Type (0 = Serial, 2 = Bluetooth, 4 = USB)")
        Console.Write("Selection (confirm with ENTER): ")
        Dim portTypeTet = Console.ReadLine()
        Select Case portTypeTet
            Case "0"
                Console.WriteLine(vbTab + "Serial selected")
                Return 0
            Case "2"
                Console.WriteLine(vbTab + "Bluetooth selected")
                Return 2
            Case "4"
                Console.WriteLine(vbTab + "USB selected")
                Return 4
            Case Else
                Console.SetCursorPosition(0, Console.CursorTop - 2)
                Return Console_InitializePortType()
        End Select
    End Function
    Private Function Console_InitializePortName() As String
        Dim cursorTop = Console.CursorTop
        Dim portNames As String() = InterfaceCommunicationSettings.GetAvailablePortNames()
        Console.WriteLine("Port Name:")
        For i = 0 To portNames.Length - 1
            Console.WriteLine(String.Format("{0} - {1}", i, portNames(i)))
        Next
        Console.Write("Selection (confirm with ENTER): ")
        Dim portNameIndexTxt = Console.ReadLine()
        Dim portNameIndex As Integer
        If Integer.TryParse(portNameIndexTxt, portNameIndex) Then
            If portNameIndex < portNames.Length Then
                Console.WriteLine(vbTab + String.Format("{0} selected", portNames(portNameIndex)))
                Return portNames(portNameIndex)
            End If
        End If

        'Selection failed
        Console.SetCursorPosition(0, cursorTop)
        Return Console_InitializePortName()
    End Function

    Dim docOperationCompleted As Boolean = True
    Private Sub DocInterfaceControl_DocResultChanged(sender As Object, _docResult As DocResultEventArgs)
        ' this will be called for each result
        If _docResult IsNot Nothing Then
            Console.WriteLine(_docResult.Timestamp.ToString("HH:mm:ss,fff"))
            If _docResult.ResultInfo IsNot Nothing Then
                If TypeOf _docResult.ResultInfo Is TELIDSensorResultInfo Then
                    'Result to GetSensorData
                    Dim getSensorResult As TELIDSensorResultInfo = _docResult.ResultInfo
                    Console.WriteLine("SensorResult:")
                    Console.WriteLine(vbTab + String.Format("SerNo: {0}", getSensorResult.SerialNumber))
                    Console.WriteLine(vbTab + String.Format("Description: {0}", getSensorResult.Description))
                    Console.WriteLine(vbTab + "Measurements:")
                    For Each meas As SensorMeasurement In getSensorResult.Measurements
                        Console.WriteLine(vbTab + vbTab + String.Format("Timestamp: {0}", meas.Timestamp))
                        Console.WriteLine(vbTab + vbTab + "Values:")
                        For Each value As SensorValue In meas.Values
                            Console.WriteLine(vbTab + vbTab + String.Format("  {0}{1}{2}", value.Symbol, value.Magnitude, value.Unit))
                        Next
                    Next
                End If
            Else
                If _docResult.ProcessFinished Then
                    docOperationCompleted = True
                    Console.WriteLine(vbTab + "function END")
                    Return
                Else
                    Console.WriteLine(vbTab + "ResultInfo = NULL")
                    Return
                End If
            End If
            If _docResult.ResultException IsNot Nothing Then
                Console.WriteLine(vbTab + "Exception!")
                System.Diagnostics.Debug.WriteLine(_docResult.ResultInfo.ToString())
            End If
        End If
    End Sub

    Private Function Console_ExecuteAndContinue(docIntControl As DocInterfaceControl) As Boolean
        'Main Console MENU
        Console.WriteLine("")
        Console.WriteLine("--------------------")
        Console.WriteLine(" Console MENU")
        Console.WriteLine("--------------------")
        Console.WriteLine("0 - ReadReaderID")
        Console.WriteLine("1 - GetSensor")
        Console.WriteLine("X - EXIT")
        Console.Write("Selection (confirm with ENTER): ")
        Dim operationNumTxt = Console.ReadLine()
        Select Case operationNumTxt
            Case "0"
                Console.WriteLine(vbTab + "ReadReaderID")
                Console_Execute_ReadReaderID(docIntControl)
            Case "1"
                Console.WriteLine(vbTab + "GetSensor")
                Console_Execute_GetSensor(docIntControl)
            Case "x", "X"
                Return False
        End Select
        Return True
    End Function

    Private Sub Console_Execute_ReadReaderID(docIntControl As DocInterfaceControl)
        'First makes ure DocInterfaceContorl is initialized
        If docIntControl IsNot Nothing Then
            If docIntControl.IsInitialized Then
                Try
                    Dim startTime As Date = Date.UtcNow
                    'Call ReadReaderID and show result
                    Dim readerID = docIntControl.ReadReaderID()
                    Dim processSpan As TimeSpan = Date.UtcNow - startTime
                    If readerID IsNot Nothing Then
                        Console.WriteLine("")
                        Console.WriteLine("ReaderID:")
                        Console.WriteLine(readerID)
                        Console.WriteLine(String.Format("(Duration: {0})", processSpan))
                    Else
                        'Update result in UI
                        Console.WriteLine(String.Format("Result: FAIL. Duration: {0}", processSpan))
                    End If
                Catch ex As Exception
                    Console.WriteLine("Exception")
                End Try
            Else
                Console.WriteLine("DocInterfaceControl not initialized!")
            End If
        End If
    End Sub

    Private Sub Console_Execute_GetSensor(docIntControl As DocInterfaceControl)
        'First makes ure DocInterfaceContorl is initialized
        If docIntControl IsNot Nothing Then
            If docIntControl.IsInitialized Then
                Console.WriteLine("")
                docOperationCompleted = False
                'Start "GetSensor" process
                '  SensorType --> 0xFC = scan all TELID® types
                '  RepeatCount --> 5 (search for transponder 5 times)
                '  DelayBetweenSearchs --> 0 (ms to wait between each search)
                '  NotifySuccessOnly --> false (raise "DocResultChanged" event even if no transponder found)
                docIntControl.StartGetSensorData(&HFC, 5, 0, False)
                'For demo purposes, just wait blocking execution until DOC process is completed (notified using "DocResultChanged" event, ProcessFinished = true)
                While Not docOperationCompleted
                    Thread.Sleep(100)
                End While
                Console.WriteLine("")
            Else
                Console.WriteLine("DocInterfaceControl not initialized!")
            End If
        End If
    End Sub
End Module

Imports System
Imports System.Threading
Imports iIDReaderLibrary
Imports iIDReaderLibrary.Utils

Module Program
    '
    ' SampleAsync_VB
    '       SampleCode for iIDReaderLibrary.DocInterfaceControl
    '       Implemented in VB
    '       Using async functions
    '
    ' This sample demonstrates how to call the DocInterfaceControl functions that can be awaited upon

    Dim m_Completed = False
    Sub Main(args As String())
        'For demo purposes, just call MainAsync without waiting
        MainAsync(args)
        While Not m_Completed
            Thread.Sleep(1000)
        End While
    End Sub

    Async Sub MainAsync(args As String())
        Console.WriteLine(".NETCore Console")
        Console.WriteLine("SampleAsync_C#")
        Console.WriteLine("--------------------")
        Console.WriteLine("Library Version: " + iIDReaderLibrary.Version.LibraryVersion)

        'Get DocInterfaceControl instance
        Dim docIntControl As DocInterfaceControl = Await Console_InitializeDocInterfaceControlAsync()
        If docIntControl IsNot Nothing Then
            'DocInterfaceControl is initialized
            Console.WriteLine("")
            Console.Write("Detecting reader..")
            While True
                'First of all, get the Reader Information
                Console.Write(".")
                Dim readerID = Await docIntControl.ReadReaderIDAsync()
                If readerID IsNot Nothing Then
                    Console.WriteLine("")
                    Console.WriteLine("Detected Reader:")
                    Console.WriteLine(readerID.ToString())
                    Exit While
                End If
            End While

            'Reader info obtained --> execute functions using menu
            Console.WriteLine("")
            While Await Console_ExecuteAndContinueAsync(docIntControl)
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
        Else
            Console.Write("Initialization error <press ENTER to exit>")
            Console.ReadLine()
        End If

        m_Completed = True
    End Sub

    Private Async Function Console_InitializeDocInterfaceControlAsync() As Task(Of DocInterfaceControl)
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
        Console.WriteLine("Initializing...")

        'Call initialize to open the communication port
        If Await result.InitializeAsync() Then
            Console.WriteLine(vbTab + "Initialized")
            Return result
        Else
            'Iniitalization failed: Terminate class instance & retry
            Console.WriteLine(vbTab + "Initialize failed")
            result.Terminate()
            Return Await Console_InitializeDocInterfaceControlAsync()
        End If
    End Function

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

    Private Async Function Console_ExecuteAndContinueAsync(docIntControl As DocInterfaceControl) As Task(Of Boolean)
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
                Await Console_Execute_ReadReaderIDAsync(docIntControl)
            Case "1"
                Console.WriteLine(vbTab + "GetSensor")
                Await Console_Execute_GetSensorAsync(docIntControl)
            Case "x", "X"
                Return False
        End Select
        Thread.Sleep(500)
        Return True
    End Function

    Private Async Function Console_Execute_ReadReaderIDAsync(docIntControl As DocInterfaceControl) As Task
        'First makes ure DocInterfaceContorl is initialized
        If docIntControl IsNot Nothing Then
            If docIntControl.IsInitialized Then
                Try
                    Dim startTime As Date = Date.UtcNow
                    'Call ReadReaderID and show result
                    Dim readerID = Await docIntControl.ReadReaderIDAsync()
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
    End Function

    Private Async Function Console_Execute_GetSensorAsync(docIntControl As DocInterfaceControl) As Task
        'First makes ure DocInterfaceContorl is initialized
        If docIntControl IsNot Nothing Then
            If docIntControl.IsInitialized Then
                Try
                    'This function blocks & searches for a default time of 1 second (optional parameter)
                    Dim startTime As Date = Date.UtcNow
                    'GetSensor --> Search for TELID® transponders 
                    Dim getSensorResult = Await docIntControl.GetSensorDataAsync(&HFC) ' FC = scan all TELID® types
                    Dim processSpan As TimeSpan = Date.UtcNow - startTime
                    If getSensorResult IsNot Nothing Then
                        Console.WriteLine("")
                        Console.WriteLine("SensorResult:")
                        Console.WriteLine(String.Format("SerNo: {0}", getSensorResult.SerialNumber))
                        Console.WriteLine(String.Format("Description: {0}", getSensorResult.Description))
                        Console.WriteLine("Measurements:")
                        For Each meas As SensorMeasurement In getSensorResult.Measurements
                            Console.WriteLine(vbTab + String.Format("Timestamp: {0}", meas.Timestamp))
                            Console.WriteLine(vbTab + "Values:")
                            For Each value As SensorValue In meas.Values
                                Console.WriteLine(vbTab + String.Format("  {0}{1}{2}", value.Symbol, value.Magnitude, value.Unit))
                            Next
                        Next
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
    End Function
End Module

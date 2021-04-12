using iIDReaderLibrary;
using iIDReaderLibrary.Utils;
using System;
using System.Threading;

namespace SampleThreads_CSharp
{
    class Program
    {
        /*
         * SampleThreads_CSharp
         *      SampleCode for iIDReaderLibrary.DocInterfaceControl
         *      Implemented in C#
         *      Using "Start..." functions
         *      
         * This sample demonstrates how to call the DocInterfaceControl functions that run the process in a separate new thread.
         * This is only for demo purposes. For a Console application is not efficient to work in this way.
         */

        static void Main(string[] args)
        {
            Console.WriteLine(".NETCore Console");
            Console.WriteLine("SampleThreads_C#");
            Console.WriteLine("--------------------");
            Console.WriteLine("Library Version: " + iIDReaderLibrary.Version.LibraryVersion);

            //Get DocInterfaceControl instance
            DocInterfaceControl docIntControl = Console_InitializeDocInterfaceControl();
            if (docIntControl != null)
            {
                //DocInterfaceControl is initialized
                Console.WriteLine("");
                Console.Write("Detecting reader..");
                while (true)
                {
                    //First of all, get the Reader Information
                    Console.Write(".");
                    var readerID = docIntControl.ReadReaderID();
                    if (readerID != null)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Detected Reader:");
                        Console.WriteLine(readerID.ToString());
                        break;
                    }
                }

                //Reader info obtained --> execute functions using menu
                Console.WriteLine("");
                while (Console_ExecuteAndContinue(docIntControl)) ;

                docIntControl.Terminate();
                Console.WriteLine("");
                Console.Write("EXITING in 5");
                Thread.Sleep(1000);
                Console.Write(", 4");
                Thread.Sleep(1000);
                Console.Write(", 3");
                Thread.Sleep(1000);
                Console.Write(", 2");
                Thread.Sleep(1000);
                Console.Write(", 1");
                Thread.Sleep(1000);
            }
            else
            {
                Console.Write("Initialization error <press ENTER to exit>");
                Console.ReadLine();
            }
        }

        static volatile bool initializeCompleted = false;
        private static DocInterfaceControl Console_InitializeDocInterfaceControl()
        {
            Console.WriteLine("== Select initialize parameters ==");
            //Get PortType
            int portType = Console_InitializePortType();
            string portName = "";
            switch (portType)
            {
                case 0:
                case 2:
                    //For Serial & bluetooth, PortName needed.
                    portName = Console_InitializePortName();
                    break;
            }
            //Initialize InterfaceCommunicationSettings class
            var readerPortSettings = InterfaceCommunicationSettings.GetForSerialDevice(portType, portName);
            //InterfaceType = 13.56MHz for TELID®200
            int interfaceType = 1356;

            //Parameters selected --> Initialize class instance
            Console.WriteLine("");
            DocInterfaceControl result = new DocInterfaceControl(readerPortSettings, interfaceType);
            Console.WriteLine(string.Format("Selected parameters: PortType: {0} | PortName: {1} | IntType: {2}", portType, portName, interfaceType));

            //Call initialize to open the communication port
            result.InitializeCompleted += DocInterfaceControl_InitializeCompleted;
            result.StartInitialize();
            Console.Write("Initializing...");
            //For demo purposes, just wait blocking execution until "Initialize" process is completed (notified using "InitializeCompleted" event)
            while (!initializeCompleted) //Alternative, call "IsInitializing"
            {
                Thread.Sleep(100);
                Console.Write(".");
            }
            Console.WriteLine("");
            if (result.IsInitialized)
            {
                Console.WriteLine("\tInitialized");
                result.DocResultChanged += DocInterfaceControl_DocResultChanged;
                return result;
            }
            else
            {
                //Initialization failed: Terminate class instance & try again
                Console.WriteLine("\tInitialize failed");
                result.Terminate();
                return Console_InitializeDocInterfaceControl();
            }
        }

        private static void DocInterfaceControl_InitializeCompleted(object _sender, bool _portOpen)
        {
            // using "_portOpen" the result of the operation can be checked
            initializeCompleted = true;
        }

        private static int Console_InitializePortType()
        {
            Console.WriteLine("Port Type (0 = Serial, 2 = Bluetooth, 4 = USB)");
            Console.Write("Selection (confirm with ENTER): ");
            string portTypeTxt = Console.ReadLine();
            switch (portTypeTxt)
            {
                case "0":
                    Console.WriteLine("\tSerial selected");
                    return 0;
                case "2":
                    Console.WriteLine("\tBluetooth selected");
                    return 2;
                case "4":
                    Console.WriteLine("\tUSB selected");
                    return 4;
                default:
                    Console.SetCursorPosition(0, Console.CursorTop - 2);
                    return Console_InitializePortType();
            }
        }
        private static string Console_InitializePortName()
        {
            int cursorTop = Console.CursorTop;
            string[] portNames = InterfaceCommunicationSettings.GetAvailablePortNames();
            Console.WriteLine("Port Name:");
            for (int i = 0; i < portNames.Length; i++)
            {
                Console.WriteLine(string.Format("{0} - {1}", i, portNames[i]));
            }
            Console.Write("Selection (confirm with ENTER): ");
            string portNameIndexTxt = Console.ReadLine();
            if (int.TryParse(portNameIndexTxt, out int portNameIndex))
            {
                if (portNameIndex < portNames.Length)
                {
                    Console.WriteLine(string.Format("\t{0} selected", portNames[portNameIndex]));
                    return portNames[portNameIndex];
                }
            }

            //Selection failed
            Console.SetCursorPosition(0, cursorTop);
            return Console_InitializePortName();
        }

        static volatile bool docOperationCompleted = false;
        private static void DocInterfaceControl_DocResultChanged(object sender, DocResultEventArgs _docResult)
        {
            if (_docResult != null)
            {
                Console.WriteLine(_docResult.Timestamp.ToString("HH:mm:ss,fff"));
                if (_docResult.ResultInfo != null)
                {
                    if (_docResult.ResultInfo is TELIDSensorResultInfo getSensorResult)
                    {
                        //Result to GetSensorData
                        Console.WriteLine("SensorResult:");
                        Console.WriteLine(string.Format("\tSerNo: {0}", getSensorResult.SerialNumber));
                        Console.WriteLine(string.Format("\tDescription: {0}", getSensorResult.Description));
                        Console.WriteLine("\tMeasurements:");
                        foreach (var meas in getSensorResult.Measurements)
                        {
                            Console.WriteLine(string.Format("\t\tTimestamp: {0}", meas.Timestamp));
                            Console.WriteLine("\t\tValues:");
                            foreach (var value in meas.Values)
                            {
                                Console.WriteLine(string.Format("\t\t  {0}{1}{2}", value.Symbol, value.Magnitude, value.Unit));
                            }
                        }
                    }
                }
                else
                {
                    if (_docResult.ProcessFinished)
                    {
                        docOperationCompleted = true;
                        Console.WriteLine("\tDOC function END");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("\tResultInfo = NULL");
                        return;
                    }
                }
                if (_docResult.ResultException != null)
                {
                    Console.WriteLine("\tException! ");
                    System.Diagnostics.Debug.WriteLine(_docResult.ResultException.ToString());
                }
            }
        }

        private static bool Console_ExecuteAndContinue(DocInterfaceControl _docIntControl)
        {
            //Main Console MENU
            Console.WriteLine("");
            Console.WriteLine("--------------------");
            Console.WriteLine(" Console MENU");
            Console.WriteLine("--------------------");
            Console.WriteLine("0 - ReadReaderID");
            Console.WriteLine("1 - GetSensor");
            Console.WriteLine("X - EXIT");
            Console.Write("Selection (confirm with ENTER): ");
            string operationNumTxt = Console.ReadLine();
            switch (operationNumTxt)
            {
                case "0":
                    Console.WriteLine("\tReadReaderID");
                    Console_Execute_ReadReaderID(_docIntControl);
                    break;
                case "1":
                    Console.WriteLine("\tGetSensor");
                    Console_Execute_GetSensor(_docIntControl);
                    break;
                case "X":
                case "x":
                    return false;
                default:
                    break;
            }
            Thread.Sleep(500);
            return true;
        }

        private static void Console_Execute_ReadReaderID(DocInterfaceControl _docIntControl)
        {
            //First make sure DocInterfaceControl is initialized
            if (_docIntControl != null)
            {
                if (_docIntControl.IsInitialized)
                {
                    try
                    {
                        DateTime startTime = DateTime.UtcNow;
                        //Call ReadReaderID and show result
                        var readerID = _docIntControl.ReadReaderID();
                        TimeSpan processSpan = DateTime.UtcNow - startTime;
                        if (readerID != null)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("ReaderID:");
                            Console.WriteLine(readerID.ToString());
                            Console.WriteLine(string.Format("(Duration: {0})", processSpan));
                        }
                        else
                        {
                            //Update result in UI
                            Console.WriteLine(string.Format("Result: FAIL. Duration: {0}", processSpan));
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Exception");
                    }
                }
                else
                {
                    Console.WriteLine("DocInterfaceControl not initialized!");
                }
            }
        }

        private static void Console_Execute_GetSensor(DocInterfaceControl _docIntControl)
        {
            //First make sure DocInterfaceControl is initialized
            if (_docIntControl != null)
            {
                if (_docIntControl.IsInitialized)
                {
                    Console.WriteLine("");
                    docOperationCompleted = false;
                    //Start "GetSensor" process
                    //  SensorType --> 0xFC = scan all TELID® types
                    //  RepeatCount --> 5 (search for transponder 5 times)
                    //  DelayBetweenSearchs --> 0 (ms to wait between each search)
                    //  NotifySuccessOnly --> false (raise "DocResultChanged" event even if no transponder found)
                    _docIntControl.StartGetSensorData(0xFC, 5, 0, false);
                    //For demo purposes, just wait blocking execution until DOC process is completed (notified using "DocResultChanged" event, ProcessFinished = true)
                    while (!docOperationCompleted)
                    {
                        Thread.Sleep(100);
                    }
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("DocInterfaceControl not initialized!");
                }
            }
        }
    }
}

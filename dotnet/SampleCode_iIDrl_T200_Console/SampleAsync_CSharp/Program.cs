using iIDReaderLibrary;
using iIDReaderLibrary.Utils;
using System;
using System.Threading;

namespace SampleAsync_CSharp
{
    class Program
    {
        /*
         * SampleAsync_CSharp
         *      SampleCode for iIDReaderLibrary.DocInterfaceControl
         *      Implemented in C#
         *      Using async functions
         *      
         * This sample demonstrates how to call the DocInterfaceControl functions that can be awaited upon.
         */

        private static volatile bool m_Completed = false;
        static void Main(string[] args)
        {
            //For demo purposes, just call "MainAsync" and wait in loop until function completes
            MainAsync(args);
            while (!m_Completed)
                Thread.Sleep(1000);
        }
        static async System.Threading.Tasks.Task MainAsync(string[] args)
        {
            Console.WriteLine(".NETCore Console");
            Console.WriteLine("SampleAsync_C#");
            Console.WriteLine("--------------------");
            Console.WriteLine("Library Version: " + DocInterfaceControl.LibraryVersion);

            //Get DocInterfaceControl instance
            DocInterfaceControl docIntControl = await Console_InitializeDocInterfaceControlAsync();
            if (docIntControl != null)
            {
                //DocInterfaceControl is initialized
                Console.WriteLine("");
                Console.Write("Detecting reader..");
                while (true)
                {
                    //First of all, get the Reader Information
                    Console.Write(".");
                    var readerID = await docIntControl.ReadReaderIDAsync();
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
                while (await Console_ExecuteAndContinueAsync(docIntControl)) ;

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
            m_Completed = true;
        }

        private static async System.Threading.Tasks.Task<DocInterfaceControl> Console_InitializeDocInterfaceControlAsync()
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
            Console.WriteLine("Initializing...");

            //Call initialize to open the communication port
            try
            {
                if (await result.InitializeAsync())
                {
                    Console.WriteLine("\tInitialized");
                    return result;
                }
                else
                {
                    //Initialization failed: Terminate class instance & try again
                    Console.WriteLine("\tInitialize failed");
                    result.Terminate();
                    return await Console_InitializeDocInterfaceControlAsync();
                }
            }
            catch { }
            return null;
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

        private static async System.Threading.Tasks.Task<bool> Console_ExecuteAndContinueAsync(DocInterfaceControl _docIntControl)
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
                    await Console_Execute_ReadReaderIDAsync(_docIntControl);
                    break;
                case "1":
                    Console.WriteLine("\tGetSensor");
                    await Console_Execute_GetSensorAsync(_docIntControl);
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

        private static async System.Threading.Tasks.Task Console_Execute_ReadReaderIDAsync(DocInterfaceControl _docIntControl)
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
                        var readerID = await _docIntControl.ReadReaderIDAsync();
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

        private static async System.Threading.Tasks.Task Console_Execute_GetSensorAsync(DocInterfaceControl _docIntControl)
        {
            //First make sure DocInterfaceControl is initialized
            if (_docIntControl != null)
            {
                if (_docIntControl.IsInitialized)
                {
                    try
                    {
                        //This function blocks & searches for a default time of 1 second (optional parameter)
                        DateTime startTime = DateTime.UtcNow;
                        //GetSensor --> Search for TELID® transponders 
                        var getSensorResult = await _docIntControl.GetSensorDataAsync(0xFC); //0xFC = scan all TELID® types
                        TimeSpan processSpan = DateTime.UtcNow - startTime;
                        if (getSensorResult != null)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("SensorResult:");
                            Console.WriteLine(string.Format("SerNo: {0}", getSensorResult.SerialNumber));
                            Console.WriteLine(string.Format("Description: {0}", getSensorResult.Description));
                            Console.WriteLine("Measurements:");
                            foreach (var meas in getSensorResult.Measurements)
                            {
                                Console.WriteLine(string.Format("\tTimestamp: {0}", meas.Timestamp));
                                Console.WriteLine("\tValues:");
                                foreach (var value in meas.Values)
                                {
                                    Console.WriteLine(string.Format("\t  {0}{1}{2}", value.Symbol, value.Magnitude, value.Unit));
                                }
                            }
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
    }
}

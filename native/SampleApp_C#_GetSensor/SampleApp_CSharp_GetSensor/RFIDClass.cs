using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace SampleApp_CSharp_GetSensor
{
    public class RFIDClass
    {
        public UIntPtr Port_Handle;

        public int Version_Main_Driver_Engine, Version_Sub_Driver_Engine;
        public int Port_Initialized;	//Port initialized?!
        public int Reader_Connected;	//Reader connected?!
        public int Reader_Id;			//ID of connected Reader
        public byte Result;				//Result of previous operation

        public byte PortType;
        //0 -> serial port/CF port
        //1 -> CF-port via elserial (only for PPC 2000 systems)
        //2 -> Bluetooth serial port
        //4 -> USB port (only WIN32 systems)
        public int InterfaceType;
        //125 -> LF = 125 kHz
        //1356 -> HF = 13.56 MHz
        //868 -> UHF = 868 MHz
        public int ProtocolType;
        //0x2 -> iID 2000 protocol
        //0x3 -> iID 3000 protocol
        //0x4 -> iID interface protocol V4
        //0x1002 -> LEGIC direct protocol

        public string PortName = "COM2:";
        private const string DLL_NAME = "IIDDRV30_PRO.DLL";

        #region "DLLIMPORTs iIDDrv30_Pro.Dll"
        [DllImport(DLL_NAME)]
        private static extern byte c_initialize();

        [DllImport(DLL_NAME)]
        private static extern byte c_terminate();

        [DllImport(DLL_NAME)]
        private static extern byte c_get_driver_version(ref	int ver_main, ref int ver_sub);

        [DllImport(DLL_NAME)]
        private static extern byte c_read_reader_id(ref	int reader_id, byte[] Data);

        [DllImport(DLL_NAME)]
        private static extern byte c_set_port_type(byte PortType, string PortName);

        [DllImport(DLL_NAME)]
        private static extern byte c_set_protocol_type(int protocol);

        [DllImport(DLL_NAME)]
        private static extern byte c_set_timeout(int newtimeout);

        [DllImport(DLL_NAME)]
        private static extern byte c_set_interface_type(int frequency);

        [DllImport(DLL_NAME)]
        private static extern UIntPtr c_get_handle();

        [DllImport(DLL_NAME)]
        private static extern UIntPtr c_get_port_state(UIntPtr ActualPort);

        [DllImport(DLL_NAME)]
        public static extern int iidl_c_get_sensor_information(UIntPtr _handle, byte _param, uint _driverParam, ref uint _SerNum, ref byte _PC, ref byte _VC, ref byte _numSizes, byte[] _sizes, byte[] _xtraData);

        [DllImport(DLL_NAME)]
        public static extern int iidl_c_get_sensor_data(UIntPtr _handle, byte _param, uint _driverParam, uint _SerNum, byte _PC, byte _VC, ref byte _numSizes, byte[] _sizes, byte[] _xtraData, ref byte _numSensor, float[] _sensorValues);

        [DllImport(DLL_NAME)]
        public static extern int iidl_c_get_sensor_data_extended(UIntPtr _handle, uint _driverParam, byte _bPhysicalSize, byte _bSensorType, byte _bParamLength, byte[] _bParams, uint _SerNum, byte _PC, byte _VC, ref byte _numSizes, byte[] _sizes, byte[] _xtraData, ref byte _numSensor, float[] _sensorValues);
        #endregion

        public RFIDClass()
        {
            Port_Initialized = 0;	//Port initialized?!
            Reader_Connected = 0;	//Reader connected?!
            Reader_Id = 0;	//ID of connected Reader
            Result = 0xFF;	//Result of previous operation

            Port_Handle = UIntPtr.Zero;	//Handle of serial Port
            PortType = 0;	//standard port = serial port
            InterfaceType = 1356;	//standard frequency = 13.56MHz
            ProtocolType = 0x3;      //standard protocol = 3000 protocol

            Version_Main_Driver_Engine = 0;
            Version_Sub_Driver_Engine = 0;
        }

        //----------- Get Connection information of reader --------------------
        public void Reader_GetState()
        {
            byte[] data = new byte[9];
            Port_Handle = c_get_port_state(Port_Handle);
            if (Port_Handle == UIntPtr.Zero)
            {
                //If invalid port handle --> Not initialized
                Port_Initialized = 0;
            }
            else
            {
                //Initialized. Check if communication with reader possible
                Port_Initialized = 1;
                Reader_Connected = 0;
                Result = c_read_reader_id(ref Reader_Id, data);
                if (Result == 0) Reader_Connected = 1;
            }
        }

        //----------- Open the Interface - Port--------------------------------
        public void Reader_OpenInterface()
        {
            byte[] data = new byte[9];
            Result = c_get_driver_version(ref Version_Main_Driver_Engine, ref Version_Sub_Driver_Engine);
            Result = c_set_interface_type(InterfaceType);
            Result = c_set_port_type(PortType, PortName);
            Result = c_set_protocol_type(ProtocolType);
            Result = c_initialize();
            if (Result != 0)
            {
                //Error opening Communication interface 
                Port_Initialized = 0;
            }
            else
            {
                //Communication interface open
                Port_Initialized = 1;
                Port_Handle = c_get_handle();

                //Check if communication with reader is working
                Reader_Connected = 0;
                Result = c_read_reader_id(ref Reader_Id, data);
                if (Result == 0)
                    Reader_Connected = 1;
            }
        }

        //----------- Close the Interface -Port --------------------------------
        public void Reader_CloseInterface()
        {
            Result = c_terminate();
            Port_Initialized = 0;
        }

        //----------- Read Reader-ID --------------------------------------------
        public int Reader_ReadId()
        {
            if (Port_Initialized == 0)
                return (0xFF); //Not initialized --> Return

            //Initialize buffer to receive Reader-ID bytes
            byte[] data = new byte[9];

            Result = c_read_reader_id(ref Reader_Id, data);
            return (Result);
        }
    }
}

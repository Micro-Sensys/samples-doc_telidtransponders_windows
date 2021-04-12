# .NET DOC sample codes for TELID®200 sensor transponders
This sample code is for handling **TELID®200** sensor transponders on Windows devices using a Micro-Sensys RFID reader

> For details on DOC communication check [Useful Links](#Useful-Links) 

## Requirements
* IDE (for example Visual Studio)
* Micro-Sensys RFID reader
* TELID®200 sensor transponders

## Implementation
This code shows how to use **iIDReaderLibrary.DocInterfaceControl** class to communicate with a TELID®200 sensor transponder. 
For demo purposes, both a .NET Core console and a WPF App as sample code is provided. Inside each solution there are different projects available that show all the different functions provided.
Code used for these projects is written in C# or VisualBasic.

> Class information is available under API documentation. See [Useful Links](#Useful-Links)

## Steps
Import this project into your IDE, choose a project and launch it. First in the console application you can select the communication port name for the RFID reader (for example *COM3*). Then for console applications a menu with functions will be shown, and for WPF application the other tabs will be enabled

![Screenshot](screenshot/SampleCode_T200_Console.png) ![Screenshot](screenshot/SampleCode_T200_WPF.png)

 1. Select the device parameters to use for *Initialize* (communication port type and name). Once the initialization process finishes, the result will be shown including the Reader ID number.
 2. Use the options (menu or UI) to perform the desired functions

## Useful Links
* [.NET Library NuGet page](https://www.nuget.org/packages/Microsensys.iIDReaderLibrary.DocInterfaceControl/)
* [API documentation](https://www.microsensys.de/downloads/DevSamples/Libraries/Windows/)
* Check what is possible using our iID®DEMOsoft (section SENSORdemo) for PC! Download it using [this link](https://www.microsensys.de/downloads/CDContent/Install/iID%c2%ae%20DEMOsoft.zip)
* GitHub *documentation* repository: [Micro-Sensys/documentation](https://github.com/Micro-Sensys/documentation)
	* [communication-modes/doc](https://github.com/Micro-Sensys/documentation/tree/master/communication-modes/doc)

## Contact

* For coding questions or questions about this sample code, you can use [support@microsensys.de](mailto:support@microsensys.de)
* For general questions about the company or our devices, you can contact us using [info@microsensys.de](mailto:info@microsensys.de)

## Authors

* **Victor Garcia** - *Initial work* - [MICS-VGarcia](https://github.com/MICS-VGarcia/)


//#define ROBUST
//#define EFFICIENT
#define CONSOLE
//#define APP


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;


// Don't forget to add this
using vJoyInterfaceWrap;

namespace FeederDemoCS
{
#if CONSOLE
    class Program
    {
        static byte buttons;
        // Declaring one joystick (Device id 1) and a position structure. 
        static public vJoy joystick;
        static public vJoy.JoystickState iReport;
        static public uint id = 1;
        public static DataIn di;
        //FeederDemoCS.Run

        static void Main(string[] args)
        {
            // Create one joystick object and a position structure.
            joystick = new vJoy();
            iReport = new vJoy.JoystickState();
            
            
            //Server
            di = new DataIn();

            // Device ID can only be in the range 1-16
            if (args.Length > 0 && !String.IsNullOrEmpty(args[0]))
                id = Convert.ToUInt32(args[0]);
            if (id <= 0 || id > 16)
            {
                Console.WriteLine("Illegal device ID {0}\nExit!", id);
                return;
            }

            // Get the driver attributes (Vendor ID, Product ID, Version Number)
            if (!joystick.vJoyEnabled())
            {
                Console.WriteLine("vJoy driver not enabled: Failed Getting vJoy attributes.\n");
                return;
            }
            else
                Console.WriteLine("Vendor: {0}\nProduct :{1}\nVersion Number:{2}\n", joystick.GetvJoyManufacturerString(), joystick.GetvJoyProductString(), joystick.GetvJoySerialNumberString());

            // Get the state of the requested device
            VjdStat status = joystick.GetVJDStatus(id);
            switch (status)
            {
                case VjdStat.VJD_STAT_OWN:
                    Console.WriteLine("vJoy Device {0} is already owned by this feeder\n", id);
                    break;
                case VjdStat.VJD_STAT_FREE:
                    Console.WriteLine("vJoy Device {0} is free\n", id);
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    Console.WriteLine("vJoy Device {0} is already owned by another feeder\nCannot continue\n", id);
                    return;
                case VjdStat.VJD_STAT_MISS:
                    Console.WriteLine("vJoy Device {0} is not installed or disabled\nCannot continue\n", id);
                    return;
                default:
                    Console.WriteLine("vJoy Device {0} general error\nCannot continue\n", id);
                    return;
            };

            // Check which axes are supported
            bool AxisX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_X);
            bool AxisY = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Y);
            bool AxisZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Z);
            bool AxisRX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RX);
            bool AxisRZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RZ);
            // Get the number of buttons and POV Hat switchessupported by this vJoy device
            int nButtons = joystick.GetVJDButtonNumber(id);
            int ContPovNumber = joystick.GetVJDContPovNumber(id);
            int DiscPovNumber = joystick.GetVJDDiscPovNumber(id);

            // Print results
            Console.WriteLine("\nvJoy Device {0} capabilities:\n", id);
            Console.WriteLine("Numner of buttons\t\t{0}\n", nButtons);
            Console.WriteLine("Numner of Continuous POVs\t{0}\n", ContPovNumber);
            Console.WriteLine("Numner of Descrete POVs\t\t{0}\n", DiscPovNumber);
            Console.WriteLine("Axis X\t\t{0}\n", AxisX ? "Yes" : "No");
            Console.WriteLine("Axis Y\t\t{0}\n", AxisX ? "Yes" : "No");
            Console.WriteLine("Axis Z\t\t{0}\n", AxisX ? "Yes" : "No");
            Console.WriteLine("Axis Rx\t\t{0}\n", AxisRX ? "Yes" : "No");
            Console.WriteLine("Axis Rz\t\t{0}\n", AxisRZ ? "Yes" : "No");

            // Test if DLL matches the driver
            UInt32 DllVer = 0, DrvVer = 0;
            bool match = joystick.DriverMatch(ref DllVer, ref DrvVer);
            if (match)
                Console.WriteLine("Version of Driver Matches DLL Version ({0:X})\n", DllVer);
            else
                Console.WriteLine("Version of Driver ({0:X}) does NOT match DLL Version ({1:X})\n", DrvVer, DllVer);


            // Acquire the target
            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!joystick.AcquireVJD(id))))
            {
                Console.WriteLine("Failed to acquire vJoy device number {0}.\n", id);
                return;
            }
            else
                Console.WriteLine("Acquired: vJoy device number {0}.\n", id);

            Console.WriteLine("\npress enter to stat feeding");
            //Console.ReadKey(true);

            


            
            //joystick.GetVJDAxisMax(id, HID_USAGES.HID_USAGE_X, ref maxval);
            while (true) System.Threading.Thread.Sleep(10000);


        } // Main
        

        static int[] axes = new int[2];
        public class DataIn
        {
            private static List<Socket> _clientSockets = new List<Socket>();
            private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            private static byte[] _buffer = new byte[64];
            
 


           
            public DataIn()
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
                Console.Title = "Server";
                //SetupServer();
                System.Threading.Thread newThread = new System.Threading.Thread(DIn);
                newThread.Start();

            }
            
            public static void DIn()
            {
                
               
                UdpClient newsock = new UdpClient(1234);

                Console.WriteLine("Waiting for a client...");

                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

                
                while (true)
                {
                    
                    _buffer = newsock.Receive(ref sender);

                    buttons = _buffer[8];


                    {
                        axes[0] = (_buffer[0] << 24) | (_buffer[1] << 16)
                  | (_buffer[2] << 8) | _buffer[3];
                        axes[1] = (_buffer[4] << 24) | (_buffer[5] << 16)
              | (_buffer[6] << 8) | _buffer[7];

                        Console.WriteLine("Text received: " + axes[0] + " " + axes[1] + " " + Process.GetCurrentProcess().BasePriority);

                    }

                    Actions();
                    
                }
            }
 
        

            private void SetupServer()
            {
                Console.WriteLine("Setting up server...");
                _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 1234));
                _serverSocket.Listen(1);
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }

            private static void AcceptCallback(IAsyncResult AR)
            {
                Socket socket = _serverSocket.EndAccept(AR);
                _clientSockets.Add(socket);
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallBack, socket);
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }

            public static void ReceiveCallBack(IAsyncResult AR)
            {
                Socket socket = (Socket)AR.AsyncState;
                int received = socket.EndReceive(AR);
                buttons = _buffer[8];

                
                {
                    axes[0] = (_buffer[0] << 24) | (_buffer[1] << 16)
              | (_buffer[2] << 8) | _buffer[3];
                    axes[1] = (_buffer[4] << 24) | (_buffer[5] << 16)
          | (_buffer[6] << 8) | _buffer[7];

                    Console.WriteLine("Text received: " + axes[0] + " " + axes[1]);

                }



                System.Threading.Thread.Sleep(10);
                Actions();
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallBack, socket);

            }

            private static void CloseAllSockets()
            {
                foreach (Socket socket in _clientSockets)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }

                _serverSocket.Close();
            }
            public static void SetAxes(bool state)
            {
                separateAxes = state;
            }

            public static bool separateAxes= false;
           static int midpoint = 0;
            public static void Actions()
            {
                iReport.bDevice = (byte)id;

                iReport.AxisX = (int)(axes[1] * Math.Abs(axes[1])/34000 *1.5) + 16383;
                    iReport.AxisY = (int) (axes[0]*0.5);

                // Set buttons one by one
                iReport.Buttons = buttons;

                iReport.bHats = 0xFFFFFFFF; // Neutral state
                iReport.bHatsEx1 = 0xFFFFFFFF; // Neutral state
                iReport.bHatsEx2 = 0xFFFFFFFF; // Neutral state
                iReport.bHatsEx3 = 0xFFFFFFFF; // Neutral state // Neutral state

                /*** Feed the driver with the position packet - is fails then wait for input then try to re-acquire device ***/
                if (!joystick.UpdateVJD(id, ref iReport))
                {
                    Console.WriteLine("Feeding vJoy device number {0} failed - try to enable device then press enter\n", id);
                    Console.ReadKey(true);
                    joystick.AcquireVJD(id);
                }

                
            }

            public static float Clamp (float min, float max, float input)
            {
                if (input < min)
                    return min;
                else if (input > max)
                    return max;
                else
                    return input;
            }


        }
    }

#endif //CONSOLE
#if APP
    class Program
    {
        static void Main()
        {
            Application.Run(new WifiController());
        }
    }
#endif
} // namespace FeederDemoCS

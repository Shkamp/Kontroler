using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using vJoyInterfaceWrap;

namespace FeederDemoCS
{
    public partial class WifiController : Form
    {
        Controller c;
        public WifiController()
        {
            InitializeComponent();
            c = new Controller();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            
        }



        class Controller
        {
            static byte buttons;
            // Declaring one joystick (Device id 1) and a position structure. 
            static public vJoy joystick;
            static public vJoy.JoystickState iReport;
            static public uint id = 1;
            //public static DataIn di;
            //FeederDemoCS.Run

            public Controller()
            {
                // Create one joystick object and a position structure.
                joystick = new vJoy();
                iReport = new vJoy.JoystickState();
              
                // Device ID can only be in the range 1-16
                /*if (args.Length > 0 && !String.IsNullOrEmpty(args[0]))
                    id = Convert.ToUInt32(args[0]);
                if (id <= 0 || id > 16)
                {
                    Console.WriteLine("Illegal device ID {0}\nExit!", id);
                    return;
                }
                */


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

#if ROBUST
    bool res;
	// Reset this device to default values
	joystick.ResetVJD(id);

	// Feed the device in endless loop
    while (true)
    {
        // Set position of 4 axes
        res = joystick.SetAxis(di.GetAccel()[1] + 16383, id, HID_USAGES.HID_USAGE_X);
        res = joystick.SetAxis(Y, id, HID_USAGES.HID_USAGE_Y);
        res = joystick.SetAxis(Z, id, HID_USAGES.HID_USAGE_Z);
        res = joystick.SetAxis(XR, id, HID_USAGES.HID_USAGE_RX);
        res = joystick.SetAxis(di.GetAccel()[0], id, HID_USAGES.HID_USAGE_RZ);

        // Press/Release Buttons
        res = joystick.SetBtn(di.GetBtn('l'), id, 1);
        res = joystick.SetBtn(di.GetBtn('r'), id, 2);
        res = joystick.SetBtn(di.GetBtn('u'), id, 3);
        res = joystick.SetBtn(di.GetBtn('d'), id, 4);


        // If Continuous POV hat switches installed - make them go round
        // For high values - put the switches in neutral state
      /*  if (ContPovNumber>0)
        {
            if ((count * 70) < 30000)
            {
                res = joystick.SetContPov(((int)count * 70), id, 1);
                res = joystick.SetContPov(((int)count * 70) + 2000, id, 2);
                res = joystick.SetContPov(((int)count * 70) + 4000, id, 3);
                res = joystick.SetContPov(((int)count * 70) + 6000, id, 4);
            }
            else
            {
                res = joystick.SetContPov(-1, id, 1);
                res = joystick.SetContPov(-1, id, 2);
                res = joystick.SetContPov(-1, id, 3);
                res = joystick.SetContPov(-1, id, 4);
            };
        };

        // If Discrete POV hat switches installed - make them go round
        // From time to time - put the switches in neutral state
        if (DiscPovNumber>0)
        {
            if (count < 550)
            {
                joystick.SetDiscPov((((int)count / 20) + 0) % 4, id, 1);
                joystick.SetDiscPov((((int)count / 20) + 1) % 4, id, 2);
                joystick.SetDiscPov((((int)count / 20) + 2) % 4, id, 3);
                joystick.SetDiscPov((((int)count / 20) + 3) % 4, id, 4);
            }
            else
            {
                joystick.SetDiscPov(-1, id, 1);
                joystick.SetDiscPov(-1, id, 2);
                joystick.SetDiscPov(-1, id, 3);
                joystick.SetDiscPov(-1, id, 4);
            };
        };

        /*System.Threading.Thread.Sleep(20);
        X += 150; if (X > maxval) X = 0;
        Y += 250; if (Y > maxval) Y = 0;
        Z += 350; if (Z > maxval) Z = 0;
        XR += 220; if (XR > maxval) XR = 0;  
        ZR += 200; if (ZR > maxval) ZR = 0;  
        count++;
        */
        if (count > 640)
            count = 0;
        
    } // While (Robust)

#endif // ROBUST
#if EFFICIENT
            
            byte[] pov = new byte[4];

            while (true)
            {
                iReport.bDevice = (byte)id;
                iReport.AxisX = (int)(axes[1]) + 16383;
                iReport.AxisY = Y;
                iReport.AxisZ = Z;

                iReport.AxisZRot = axes[0]< 16383? axes[0]-3000 : (int)(axes[0] + axes[0]/5);
                iReport.AxisXRot = XR;

                // Set buttons one by one
                iReport.Buttons = buttons;

                if (ContPovNumber > 0)
                {
                    // Make Continuous POV Hat spin
                    iReport.bHats = (count * 70);
                    iReport.bHatsEx1 = (count * 70) + 3000;
                    iReport.bHatsEx2 = (count * 70) + 5000;
                    iReport.bHatsEx3 = 15000 - (count * 70);
                    if ((count * 70) > 36000)
                    {
                        iReport.bHats = 0xFFFFFFFF; // Neutral state
                        iReport.bHatsEx1 = 0xFFFFFFFF; // Neutral state
                        iReport.bHatsEx2 = 0xFFFFFFFF; // Neutral state
                        iReport.bHatsEx3 = 0xFFFFFFFF; // Neutral state
                    };
                }
                else
                {
                    // Make 5-position POV Hat spin

                    pov[0] = (byte)(((count / 20) + 0) % 4);
                    pov[1] = (byte)(((count / 20) + 1) % 4);
                    pov[2] = (byte)(((count / 20) + 2) % 4);
                    pov[3] = (byte)(((count / 20) + 3) % 4);

                    iReport.bHats = (uint)(pov[3] << 12) | (uint)(pov[2] << 8) | (uint)(pov[1] << 4) | (uint)pov[0];
                    if ((count) > 550)
                        iReport.bHats = 0xFFFFFFFF; // Neutral state
                }

                /*** Feed the driver with the position packet - is fails then wait for input then try to re-acquire device ***/
                if (!joystick.UpdateVJD(id, ref iReport))
                {
                    Console.WriteLine("Feeding vJoy device number {0} failed - try to enable device then press enter\n", id);
                    Console.ReadKey(true);
                    joystick.AcquireVJD(id);
                }

                //System.Threading.Thread.Sleep(5);
                 /*count++;
                 if (count > 640) count = 0;

                 X += 150; if (X > maxval) X = 0;
                 Y += 250; if (Y > maxval) Y = 0;
                 Z += 350; if (Z > maxval) Z = 0;
                 XR += 220; if (XR > maxval) XR = 0;
                 ZR += 200; if (ZR > maxval) ZR = 0;  
                  */
            } // While

            

#endif // EFFICIENT

            } // Main


            static int[] axes = new int[2];
            public class DataIn
            {
                private static List<Socket> _clientSockets = new List<Socket>();
                private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                private static byte[] _buffer = new byte[2048];





                public DataIn()
                {
                    //Console.Title = "Server";
                    SetupServer();
                    //DIn();

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
                    //byte[] dataBuff = new byte[received];
                    //Array.Copy(_buffer, 10, dataBuff, 0,1);
                    // if (received < 4)
                    buttons = _buffer[8];


                    {
                        axes[0] = (_buffer[0] << 24) | (_buffer[1] << 16)
                  | (_buffer[2] << 8) | _buffer[3];
                        axes[1] = (_buffer[4] << 24) | (_buffer[5] << 16)
              | (_buffer[6] << 8) | _buffer[7];

                        Console.WriteLine("Text received: " + axes[0] + " " + axes[1]);

                    }

                    /* if (text.StartsWith("[0-9]")){
                         string[] temp = text.Split(' ');
                         axes[0] = int.Parse(temp[0]);
                         axes[1] = int.Parse(temp[1]);
                     }*/



                    System.Threading.Thread.Sleep(10);
                    actions();
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

                public static bool separateAxes = true;
                static int midpoint = 15000;
                public static void actions()
                {
                    iReport.bDevice = (byte)id;
                    iReport.AxisX = (int)(axes[1] * 0.7) + 16383;
                    float facRotZ = ((32767 + midpoint) % 32767) / 7000;
                    float facRotY = ((32767 - midpoint) % 32767) / 7000;
                    if (axes[1] > 24576 || axes[1] < 8192)
                        facRotZ += 2;
                    //iReport.AxisY = Y;
                    //iReport.AxisZ = Z;
                    if (separateAxes)
                    {
                        iReport.AxisZRot = axes[0] > midpoint ? (int)((axes[0] - midpoint) * facRotZ) : 0;
                        iReport.AxisYRot = axes[0] < midpoint ? (int)((-axes[0] + midpoint) * facRotY) : 0;
                    }
                    else
                    {
                        iReport.AxisZRot = (int)(axes[0] * 1.2); //axes[0] < 16383 ? axes[0] - 3000 : (int)(axes[0]*1.2);
                    }
                    //iReport.AxisXRot = XR;

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

                    //System.Threading.Thread.Sleep(5);
                    /*count++;
                    if (count > 640) count = 0;

                    X += 150; if (X > maxval) X = 0;
                    Y += 250; if (Y > maxval) Y = 0;
                    Z += 350; if (Z > maxval) Z = 0;
                    XR += 220; if (XR > maxval) XR = 0;
                    ZR += 200; if (ZR > maxval) ZR = 0;  
                     */
                }

                public static float Clamp(float min, float max, float input)
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


    } // namespace FeederDemoCS


}


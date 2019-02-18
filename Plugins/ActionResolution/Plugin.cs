using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DigitalOx.SoundBoard.Plugin
{
    public class ActionData
    {
        public int Mode { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int RefreshRate { get; set; }
        public int Monitor { get; set; }
    }

    class ActionResolution : IPlugin
    {
        #region Interface members
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }

        private IPluginHost host;
        public IPluginHost Host
        {
            get { return host; }
            set
            {
                host = value;
                host.Register(this);
            }
        }
        #endregion

        #region Local members
        List<string> listOfDevices = new List<string>();
        List<string> listSettings = new List<string>();
        int currentWidth = 0;
        int currentHeight = 0;
        DEVMODE currentDevMode;
        #endregion

        public ActionResolution()
        {
            Name = "Action Resolution";
            Description = "Change screen resolution.";
            Id = "9EA1F78C-0C3D-4516-AFBF-B67C958DF0A4";
            EnumDevices();
        }

        public async Task<PluginResponse> DoWorkAsync(object actionData)
        {
            if(string.IsNullOrEmpty((string)actionData)) {
                return PluginResponse.Fail;
            }

            ActionData data = JsonConvert.DeserializeObject<ActionData>(actionData.ToString());

            //perform work
            //int devNum = listOfDevices.SelectedIndex;
            //int modeNum = listSettings.SelectedIndex;
            //int n = FindDevModeMatchingXY(data.Width, data.Height);

            DEVMODE d = GetDevmode(0, data.Mode);
            if (d.dmBitsPerPel != 0 & d.dmPelsWidth != 0 & d.dmPelsHeight != 0)
            {
                ChangeDisplaySettings(ref d, 0);
            }


            return PluginResponse.Ok;
        }

        private int FindDevModeMatchingXY(int x, int y)
        {
            listSettings.Clear();

            string devName = GetDeviceName(0);
            DEVMODE devMode = new DEVMODE();
            int modeNum = 0;
            bool result = true;
            do
            {
                result = EnumDisplaySettings(devName, modeNum, ref devMode);

                if (result)
                {
                    string item = DevmodeToString(devMode);
                    listSettings.Add(item);
                }
                modeNum++;
            } while (result);

            if (listSettings.Count > 0)
            {
                for (int n = 0; n < modeNum; n++)
                {
                    DEVMODE modeinfo = GetDevmode(0, n);
                    if(modeinfo.dmPelsWidth == x && modeinfo.dmPelsHeight == y && modeinfo.dmDisplayFrequency==29 && modeinfo.dmBitsPerPel==8)
                    {
                        return n;
                    }

                }
                
                //int selected = listSettings.IndexOf(DevmodeToString(current));
                //if (selected >= 0)
                //{
                //    currentDevMode = current;
                //    //listSettings.SelectedIndex = selected;
                //    ////   listSettings.SetSelected(selected, true);
                //}
            }

            return 0;
        }



        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAY_DEVICE
        {
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            public int StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;

            public DISPLAY_DEVICE(int flags)
            {
                cb = 0;
                StateFlags = flags;
                DeviceName = new string((char)32, 32);
                DeviceString = new string((char)32, 128);
                DeviceID = new string((char)32, 128);
                DeviceKey = new string((char)32, 128);
                cb = Marshal.SizeOf(this);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public short dmOrientation;
            public short dmPaperSize;
            public short dmPaperLength;
            public short dmPaperWidth;
            public short dmScale;
            public short dmCopies;
            public short dmDefaultSource;
            public short dmPrintQuality;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            public short dmUnusedPadding;
            public short dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
        }

        //private void listDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    int devNum = listDevices.SelectedIndex;
        //    bool isMain = MainDevice(devNum);
        //    btnSet.IsEnabled = isMain; // enable only for the main device
        //    EnumModes(devNum);
        //}

        //private void btnSet_Click(object sender, RoutedEventArgs e)
        //{ //set selected display mode
        //    int devNum = listDevices.SelectedIndex;
        //    int modeNum = listSettings.SelectedIndex;
        //    DEVMODE d = GetDevmode(devNum, modeNum);
        //    if (d.dmBitsPerPel != 0 & d.dmPelsWidth != 0
        //        & d.dmPelsHeight != 0)
        //    {
        //        ChangeDisplaySettings(ref d, 0);
        //    }
        //}

        private void EnumModes(int devNum)
        {
            listSettings.Clear();

            string devName = GetDeviceName(devNum);
            DEVMODE devMode = new DEVMODE();
            int modeNum = 0;
            bool result = true;
            do
            {
                result = EnumDisplaySettings(devName,
                    modeNum, ref devMode);

                if (result)
                {
                    string item = DevmodeToString(devMode);
                    listSettings.Add(item);
                }
                modeNum++;
            } while (result);

            if (listSettings.Count > 0)
            {
                DEVMODE current = GetDevmode(devNum, -1);

                //   int selected = listSettings.FindString(DevmodeToString(current));
                int selected = listSettings.IndexOf(DevmodeToString(current));
                if (selected >= 0)
                {
                    currentDevMode = current;
                    //listSettings.SelectedIndex = selected;
                    ////   listSettings.SetSelected(selected, true);
                }
            }
        }

        private DEVMODE GetDevmode(int devNum, int modeNum)
        { 
            //populates DEVMODE for the specified device and mode
            DEVMODE devMode = new DEVMODE();
            string devName = GetDeviceName(devNum);
            EnumDisplaySettings(devName, modeNum, ref devMode);
            return devMode;
        }

        private string DevmodeToString(DEVMODE devMode)
        {
            return $"{devMode.dmPelsWidth} x {devMode.dmPelsHeight}, {devMode.dmBitsPerPel} bits, {devMode.dmDisplayFrequency} Hz";
            //return devMode.dmPelsWidth.ToString() + " x " + devMode.dmPelsHeight.ToString() + ", " + devMode.dmBitsPerPel.ToString() + " bits, " + devMode.dmDisplayFrequency.ToString() + " Hz";
        }

        private void EnumDevices()
        { 
            //populates Display Devices list
            listOfDevices.Clear();
            DISPLAY_DEVICE d = new DISPLAY_DEVICE(0);

            int devNum = 0;
            bool result;
            do
            {
                result = EnumDisplayDevices(IntPtr.Zero, devNum, ref d, 0);

                if (result)
                {
                    string item = devNum.ToString() + ". " + d.DeviceString.Trim();
                    if ((d.StateFlags & 4) != 0)
                    {
                        item += " - main";
                        EnumModes(devNum);
                    }
                    listOfDevices.Add(item);
                }
                devNum++;
            } while (result);
        }

        private string GetDeviceName(int devNum)
        {
            DISPLAY_DEVICE d = new DISPLAY_DEVICE(0);
            bool result = EnumDisplayDevices(IntPtr.Zero, devNum, ref d, 0);
            return (result ? d.DeviceName.Trim() : "#error#");
        }

        private bool MainDevice(int devNum)
        { 
            //whether the specified device is the main device
            DISPLAY_DEVICE d = new DISPLAY_DEVICE(0);
            if (EnumDisplayDevices(IntPtr.Zero, devNum, ref d, 0))
            {
                return ((d.StateFlags & 4) != 0);
            }
            return false;
        }

        [DllImport("User32.dll")]
        private static extern bool EnumDisplayDevices(IntPtr lpDevice, int iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, int dwFlags);

        [DllImport("User32.dll")]
        private static extern bool EnumDisplaySettings(string devName, int modeNum, ref DEVMODE devMode);

        [DllImport("User32.dll")]
        public static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HidLibrary;

using IPA.DAL.RBADAL;
using IPA.DAL.RBADAL.Services;
using IPA.LoggerManager;
using IPA.CommonInterface;
using IPA.CommonInterface.Interfaces;
using IPA.CommonInterface.Helpers;
using System.Reflection;
using System.Configuration;

namespace IDTechConfigReader
{
    public partial class Form1 : Form
    {
        /********************************************************************************************************/
        // ATTRIBUTES
        /********************************************************************************************************/
        #region -- attributes --
        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        const int CONFIG_PANEL_WIDTH = 50;

        // Always on TOP
        bool tc_always_on_top;
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        IDevicePlugIn devicePlugin;

        bool formClosing = false;
        #endregion

        public Form1()
        {
            InitializeComponent();

            this.tabControlConfiguration.Width += CONFIG_PANEL_WIDTH;

            try
            {
                // Application Always on Top
                //string always_on_top = System.Configuration.ConfigurationManager.AppSettings["tc_always_on_top"] ?? "true";
                //bool.TryParse(always_on_top, out tc_always_on_top);
                tc_always_on_top = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["tc_always_on_top"] ?? "true");
            }
            catch(Exception e)
            {
                Logger.error("main: Form() - exception={0}", (object) e.Message);
            }
            SetupLogging();

            InitalizeDevice();
        }

        /********************************************************************************************************/
        // DELEGATES SECTION
        /********************************************************************************************************/
        #region -- delegates section --

        private void InitalizeDeviceUI(object sender, DeviceNotificationEventArgs e)
        {
            InitalizeDevice();
        }
        
        protected void OnDeviceNotificationUI(object sender, DeviceNotificationEventArgs args)
        {
            Debug.WriteLine("main: notification type={0}", args.NotificationType);

            switch (args.NotificationType)
            {
                case NOTIFICATION_TYPE.NT_INITIALIZE_DEVICE:
                {
                    break;
                }
                case NOTIFICATION_TYPE.NT_DEVICE_UPDATE_CONFIG:
                {
                    UpdateUI();
                    break;
                }
                case NOTIFICATION_TYPE.NT_UNLOAD_DEVICE_CONFIGDOMAIN:
                {
                    UnloadDeviceConfigurationDomainUI(sender, args);
                    break;
                }
                case NOTIFICATION_TYPE.NT_SET_DEVICE_MODE:
                {
                    SetDeviceModeUI(sender, args);
                    break;
                }
                case NOTIFICATION_TYPE.NT_SHOW_TERMINAL_DATA:
                {
                    ShowTerminalDataUI(sender, args);
                    break;
                }
                case NOTIFICATION_TYPE.NT_SHOW_AID_LIST:
                {
                    ShowAidListUI(sender, args);
                    break;
                }
                case NOTIFICATION_TYPE.NT_SHOW_CAPK_LIST:
                {
                    ShowCapKListUI(sender, args);
                    break;
                }

                case NOTIFICATION_TYPE.NT_SHOW_CONFIG_GROUP:
                {
                    ShowConfigGroupUI(sender, args);
                    break;
                }

                case NOTIFICATION_TYPE.NT_UI_ENABLE_BUTTONS:
                {
                    EnableButtonsUI(sender, args);
                    break;
                }

                case NOTIFICATION_TYPE.NT_SET_EMV_MODE_BUTTON:
                {
                    SetEmvButtonUI(sender, args);
                    break;
                }

                case NOTIFICATION_TYPE.NT_FIRMWARE_UPDATE_STEP:
                {
                    FirmwareUpdateProgressUI(sender, args);
                    break;
                }

                case NOTIFICATION_TYPE.NT_FIRMWARE_UPDATE_STATUS:
                {
                    FirmwareUpdateStatusUI(sender, args);
                    break;
                }

                case NOTIFICATION_TYPE.NT_FIRMWARE_UPDATE_FAILED:
                {
                    FirmwareUpdateFailedUI(sender, args);
                    break;
                }

                case NOTIFICATION_TYPE.NT_FIRMWARE_UPDATE_COMPLETE:
                {
                    EnableMainFormUI(sender, args);
                    break;
                }
            }
        }
        private void UnloadDeviceConfigurationDomainUI(object sender, DeviceNotificationEventArgs e)
        {
            UnloadDeviceConfigurationDomain(e.Message);
        }

        private void SetDeviceModeUI(object sender, DeviceNotificationEventArgs e)
        {
            SetDeviceMode(e.Message);
        }
        
        private void ShowTerminalDataUI(object sender, DeviceNotificationEventArgs e)
        {
            ShowTerminalData(e.Message);
        }
        
        private void ShowAidListUI(object sender, DeviceNotificationEventArgs e)
        {
            ShowAidList(e.Message);
        }

        private void ShowCapKListUI(object sender, DeviceNotificationEventArgs e)
        {
            ShowCapKList(e.Message);
        }

        private void ShowConfigGroupUI(object sender, DeviceNotificationEventArgs e)
        {
            ShowConfigGroup(e.Message);
        }

        private void EnableButtonsUI(object sender, DeviceNotificationEventArgs e)
        {
            EnableButtons();
        }

        private void SetEmvButtonUI(object sender, DeviceNotificationEventArgs e)
        {
            SetEmvButton(e.Message);
        }
        private void FirmwareUpdateProgressUI(object sender, DeviceNotificationEventArgs e)
        {
            FirmwareUpdateProgress(e.Message);
        }
        private void FirmwareUpdateStatusUI(object sender, DeviceNotificationEventArgs e)
        {
            FirmwareUpdateStatus(e.Message);
        }
        private void FirmwareUpdateFailedUI(object sender, DeviceNotificationEventArgs e)
        {
            FirmwareUpdateFailed(e.Message);
        }
        private void EnableMainFormUI(object sender, DeviceNotificationEventArgs e)
        {
            EnableMainForm(e.Message);
        }
        #endregion

        /********************************************************************************************************/
        // GUI - DELEGATE SECTION
        /********************************************************************************************************/
        #region -- gui delegate section --

        private void ClearUI()
        {
            if (InvokeRequired)
            {
                MethodInvoker Callback = new MethodInvoker(ClearUI);
                Invoke(Callback);
            }
            else
            {
            }
        }

        private void UpdateUI()
        {
            if (InvokeRequired)
            {
                MethodInvoker Callback = new MethodInvoker(UpdateUI);
                Invoke(Callback);
            }
            else
            {
                SetConfiguration();
            }
        }

        #endregion

        /********************************************************************************************************/
        // DEVICE ARTIFACTS
        /********************************************************************************************************/
        #region -- device artifacts --
        private void SetConfiguration()
        {
            Debug.WriteLine("main: update GUI elements =========================================================");

            try
            {
                string[] config = devicePlugin.GetConfig();

                if (config != null)
                {
                    // value expected: either dashed or space separated
                    if(config[1] != null)
                    {
                        this.lblFirmwareVersion.Text = config[1];
                        this.btnFirmwareUpdate.Enabled = true;
                        this.btnFirmwareUpdate.Visible = true;
                        this.FirmwareprogressBar1.Visible      = false;
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.error("main: SetConfiguration() exception={0}", (object)ex.Message);
            }
        }
        #endregion
        /********************************************************************************************************/
        // FORM ELEMENTS
        /********************************************************************************************************/
        #region -- form elements --

        private void OnFormLoad(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.MaximizeBox = false;

            if(tc_always_on_top)
            {
                SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            formClosing = true;

            if (devicePlugin != null)
            {
                try
                {
                    devicePlugin.SetFormClosing(formClosing);
                }
                catch(Exception ex)
                {
                    Logger.error("main: Form1_FormClosing() - exception={0}", (object) ex.Message);
                }
            }
        }
        #endregion

        void SetupLogging()
        {
            try
            {
                var logLevels = ConfigurationManager.AppSettings["IPA.DAL.Application.Client.LogLevel"]?.Split('|') ?? new string[0];
                if(logLevels.Length > 0)
                {
                    string fullName = Assembly.GetEntryAssembly().Location;
                    string logname = System.IO.Path.GetFileNameWithoutExtension(fullName) + ".log";
                    string path = System.IO.Directory.GetCurrentDirectory(); 
                    string filepath = path + "\\" + logname;

                    int levels = 0;
                    foreach(var item in logLevels)
                    {
                        foreach(var level in LogLevels.LogLevelsDictonary.Where(x => x.Value.Equals(item)).Select(x => x.Key))
                        {
                            levels += (int)level;
                        }
                    }

                    Logger.SetFileLoggerConfiguration(filepath, levels);

                    Logger.info( "LOGGING INITIALIZED.");

                    //Logger.info( "LOG ARG1:", "1111");
                    //Logger.info( "LOG ARG1:{0}, ARG2:{1}", "1111", "2222");
                    Logger.debug("THIS IS A DEBUG STRING");
                    Logger.warning("THIS IS A WARNING");
                    Logger.error("THIS IS AN ERROR");
                    Logger.fatal("THIS IS FATAL");
                }
            }
            catch(Exception e)
            {
                Logger.error("main: SetupLogging() - exception={0}", (object) e.Message);
            }
        }

        private void InitalizeDevice()
        {
            if(this.IsHandleCreated)
            {
                MethodInvoker mi = () =>
                {
                    this.ApplicationpicBoxWait.Enabled = false;
                    this.ApplicationpicBoxWait.Visible  = false;
                };

                if (InvokeRequired)
                {
                    BeginInvoke(mi);
                }
                else
                {
                    Invoke(mi);
                }
            }

            try
            {
                devicePlugin = new IPA.DAL.RBADAL.DeviceCfg() as IDevicePlugIn;
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    try
                    {
                        // Initialize Device
                        devicePlugin.OnDeviceNotification += new EventHandler<DeviceNotificationEventArgs>(this.OnDeviceNotificationUI);
                        devicePlugin.DeviceInit();
                        Debug.WriteLine("main: loaded plugin={0} ++++++++++++++++++++++++++++++++++++++++++++", (object)devicePlugin.PluginName);
                    }
                    catch(Exception ex)
                    {
                        Logger.error("main: exception={0}", (object)ex.Message);
                        if(ex.Message.Equals("NoDevice"))
                        {
                            WaitForDeviceToConnect();
                        }
                        else if(ex.Message.Equals("MultipleDevice"))
                        {
                            this.Invoke(new MethodInvoker(() =>
                            {
                                MessageBoxEx.Show(this, "Multiple Devices Detected\r\nDisconnect One of them !!!", "ERROR: MULTIPLE DEVICES DETECTED", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                WaitForDeviceToConnect();
                            }));
                        }
                    }

                }).Start();


            }
            catch (Exception exp)
            {
                Logger.error("main: Initalize() - exception={0}", (object) exp.Message);
            }
        }

        private void WaitForDeviceToConnect()
        {
            if(this.IsHandleCreated)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    this.ApplicationpicBoxWait.Enabled = true;
                    this.ApplicationpicBoxWait.Visible  = true;
                    this.btnFirmwareUpdate.Enabled = false;
                    this.lblFirmwareVersion.Text = "UNKNOWN";
                }));
            }

            // Wait for a new device to connect
            new Thread(() =>
            {
                bool foundit = false;
                Thread.CurrentThread.IsBackground = true;

                Debug.Write("Waiting for new device to connect");

                // Wait for a device to attach
                while (!formClosing && !foundit)
                {
                    HidDevice device = HidDevices.Enumerate(Device_IDTech.IDTechVendorID).FirstOrDefault();

                    if (device != null)
                    {
                        foundit = true;
                        device.CloseDevice();
                    }
                    else
                    {
                        Debug.Write(".");
                        Thread.Sleep(1000);
                    }
                }

                // Initialize Device
                if (!formClosing && foundit)
                {
                    Debug.WriteLine("found one!");

                    Thread.Sleep(3000);

                    // Initialize Device
                    InitalizeDeviceUI(this, new DeviceNotificationEventArgs());
                }

            }).Start();
        }

        private void UnloadDeviceConfigurationDomain(object payload)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                // Terminate Transaction Timer if running
                //TransactionTimer?.Stop();

                //ClearUI();

                // Unload The Plugin
                //appDomainCfg.UnloadPlugin(appDomainDevice);

                // wait for a new device to connect
                WaitForDeviceToConnect();

            }).Start();
        }

        private void SetDeviceMode(object payload)
        {
            // Invoker with Parameter(s)
            MethodInvoker mi = () =>
            {
                try
                {
                    string [] data = ((IEnumerable) payload).Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray();
                    this.ConfigurationPanel1btnDeviceMode.Enabled = true;
                    this.ConfigurationPanel1btnDeviceMode.Text = data[0];
                    this.ConfigurationPanel1btnEMVMode.Enabled = (this.ConfigurationPanel1btnDeviceMode.Text.Equals(USK_DEVICE_MODE.USB_HID)) ? false : true;
                }
                catch (Exception exp)
                {
                    Logger.error("main: SetDeviceMode() - exception={0}", (object) exp.Message);
                }
            };

            if (InvokeRequired)
            {
                BeginInvoke(mi);
            }
            else
            {
                Invoke(mi);
            }
        }

        private void ShowTerminalData(object payload)
        {
            // Invoker with Parameter(s)
            MethodInvoker mi = () =>
            {
                try
                {
                    string [] data = ((IEnumerable) payload)?.Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray() ?? null;

                    // Remove previous entries
                    if(ConfigurationTerminalDatalistView.Items.Count > 0)
                    {
                        ConfigurationTerminalDatalistView.Items.Clear();
                    }

                    if(data != null)
                    { 
                        // Check for ERRORS
                        if(!data[0].Equals("NO FIRMWARE VERSION MATCH"))
                        {
                            foreach(string val in data)
                            {
                                string [] tlv = val.Split(':');
                                ListViewItem item1 = new ListViewItem(tlv[0], 0);
                                item1.SubItems.Add(tlv[1]);
                                ConfigurationTerminalDatalistView.Items.Add(item1);
                            }

                            // TAB 0
                            if(!tabControlConfiguration.Contains(ConfigurationTerminalDatatabPage))
                            {
                                tabControlConfiguration.TabPages.Add(ConfigurationTerminalDatatabPage);
                            }
                            this.ConfigurationTerminalDatatabPage.Enabled = true;
                            tabControlConfiguration.SelectedTab = this.ConfigurationTerminalDatatabPage;
                            // TAB 1
                            if(!tabControlConfiguration.Contains(ConfigurationAIDStabPage))
                            {
                                tabControlConfiguration.TabPages.Add(ConfigurationAIDStabPage);
                            }
                            this.ConfigurationAIDStabPage.Enabled = true;
                            // TAB 2
                            if(!tabControlConfiguration.Contains(ConfigurationCAPKStabPage))
                            {
                                tabControlConfiguration.TabPages.Add(ConfigurationCAPKStabPage);
                            }
                            this.ConfigurationCAPKStabPage.Enabled = true;
                            // TAB 3
                            if(!tabControlConfiguration.Contains(ConfigurationGROUPStabPage))
                            {
                                tabControlConfiguration.TabPages.Add(ConfigurationGROUPStabPage);
                            }
                            this.ConfigurationGROUPStabPage.Enabled = true;
                        }
                        else
                        {
                            ListViewItem item1 = new ListViewItem("ERROR", 0);
                            item1.SubItems.Add(data[0]);
                            ConfigurationTerminalDatalistView.Items.Add(item1);
                        }
                    }
                    else
                    {
                        ListViewItem item1 = new ListViewItem("ERROR", 0);
                        item1.SubItems.Add("*** NO DATA ***");
                        ConfigurationTerminalDatalistView.Items.Add(item1);
                    }

                    ConfigurationTerminalDatalistView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    ConfigurationTerminalDatalistView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                    this.ConfigurationTerminalDatapicBoxWait.Enabled = false;
                    this.ConfigurationTerminalDatapicBoxWait.Visible  = false;
                }
                catch (Exception exp)
                {
                    Logger.error("main: ShowTerminalData() - exception={0}", (object) exp.Message);
                }
            };

            if (InvokeRequired)
            {
                BeginInvoke(mi);
            }
            else
            {
                Invoke(mi);
            }
        }

        private void ShowAidList(object payload)
        {
            // Invoker with Parameter(s)
            MethodInvoker mi = () =>
            {
                try
                {
                    string [] data = ((IEnumerable) payload).Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray();

                    // Remove previous entries
                    if(ConfigurationAIDSlistView.Items.Count > 0)
                    {
                        ConfigurationAIDSlistView.Items.Clear();
                    }

                    foreach(string item in data)
                    {
                        string [] components = item.Split('#');
                        if(components.Length == 2)
                        {
                            ListViewItem item1 = new ListViewItem(components[0], 0);
                            item1.SubItems.Add(components[1]);
                            ConfigurationAIDSlistView.Items.Add(item1);
                        }
                    }

                    ConfigurationAIDSlistView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    ConfigurationAIDSlistView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                    if(!tabControlConfiguration.Contains(ConfigurationAIDStabPage))
                    {
                        tabControlConfiguration.TabPages.Add(ConfigurationAIDStabPage);
                    }
                    this.ConfigurationAIDStabPage.Enabled = true;
                    tabControlConfiguration.SelectedTab = this.ConfigurationAIDStabPage;
                }
                catch (Exception exp)
                {
                    Logger.error("main: ShowAIDData() - exception={0}", (object) exp.Message);
                }
                finally
                {
                    this.ConfigurationAIDSpicBoxWait.Enabled = false;
                    this.ConfigurationAIDSpicBoxWait.Visible  = false;
                }
            };

            if (InvokeRequired)
            {
                BeginInvoke(mi);
            }
            else
            {
                Invoke(mi);
            }
        }

        private void ShowCapKList(object payload)
        {
            // Invoker with Parameter(s)
            MethodInvoker mi = () =>
            {
                try
                {
                    string [] data = ((IEnumerable) payload)?.Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray() ?? null;

                    // Remove previous entries
                    if(ConfigurationCAPKSlistView.Items.Count > 0)
                    {
                        ConfigurationCAPKSlistView.Items.Clear();
                    }

                    if(data != null)
                    { 
                        foreach(string item in data)
                        {
                            string [] components = item.Split('#');
                            if(components.Length == 2)
                            {
                                ListViewItem item1 = new ListViewItem(components[0], 0);
                                string [] keyvalue = components[1].Split(' ');
                                if(keyvalue.Length > 2)
                                {
                                    // RID
                                    //string [] ridvalue = keyvalue[0].Split(':');
                                    //if(ridvalue.Length == 2)
                                    //{
                                    //    item1.SubItems.Add(ridvalue[1]);
                                    //}
                                    // INDEX
                                    //string [] indexvalue = keyvalue[1].Split(':');
                                    //if(indexvalue.Length == 2)
                                    //{
                                    //    item1.SubItems.Add(indexvalue[1]);
                                    //}
                                    // MODULUS
                                    string [] modvalues = keyvalue[2].Split(':');
                                    if(modvalues.Length == 2)
                                    {
                                        item1.SubItems.Add(modvalues[1]);
                                    }
                                    // EXPONENT: FILE ONLY
                                    //string [] expvalues = keyvalue[3].Split(':');
                                    //if(expvalues.Length == 2)
                                    //{
                                    //    item1.SubItems.Add(expvalues[1]);
                                    //}
                                    // CHECKSUM: FILE ONLY
                                    //string [] checksumvalues = keyvalue[4].Split(':');
                                    //if(checksumvalues.Length == 2)
                                    //{
                                    //    item1.SubItems.Add(checksumvalues[1]);
                                    //}
                                    ConfigurationCAPKSlistView.Items.Add(item1);
                                }
                            }
                        }
                    }
                    else
                    {
                        ListViewItem item1 = new ListViewItem("N/A", 0);
                        item1.SubItems.Add("*** NO DATA ****");
                        ConfigurationCAPKSlistView.Items.Add(item1);
                    }

                    ConfigurationCAPKSlistView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    ConfigurationCAPKSlistView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                    if(!tabControlConfiguration.Contains(ConfigurationCAPKStabPage))
                    {
                        tabControlConfiguration.TabPages.Add(ConfigurationCAPKStabPage);
                    }
                    this.ConfigurationCAPKStabPage.Enabled = true;
                    tabControlConfiguration.SelectedTab = this.ConfigurationCAPKStabPage;
                }
                catch (Exception exp)
                {
                    Logger.error("main: ShowCapKList() - exception={0}", (object) exp.Message);
                }
                finally
                {
                    this.ConfigurationCAPKSpicBoxWait.Enabled = false;
                    this.ConfigurationCAPKSpicBoxWait.Visible  = false;
                }
            };

            if (InvokeRequired)
            {
                BeginInvoke(mi);
            }
            else
            {
                Invoke(mi);
            }
        }

        private void ShowConfigGroup(object payload)
        {
            // Invoker with Parameter(s)
            MethodInvoker mi = () =>
            {
                try
                {
                    // Remove previous entries
                    if(ConfigurationGROUPSlistView.Items.Count > 0)
                    {
                        ConfigurationGROUPSlistView.Items.Clear();
                    }

                    if(payload != null)
                    {
                        string [] data = ((IEnumerable) payload).Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray();

                        foreach(string item in data)
                        {
                            string [] components = item.Split(':');
                            if(components.Length == 3)
                            {
                                ListViewItem item1 = new ListViewItem(components[0], 0);
                                string keytag = components[1];
                                if(keytag.Length > 0)
                                {
                                    // TAG
                                    item1.SubItems.Add(keytag);

                                    // VALUE
                                    string keyvalue = components[2];
                                    if(keyvalue.Length > 0)
                                    {
                                        // TAG
                                        item1.SubItems.Add(keyvalue);
                                        ConfigurationGROUPSlistView.Items.Add(item1);
                                    }
                                }
                            }
                        }

                        if(!tabControlConfiguration.Contains(ConfigurationGROUPStabPage))
                        {
                            tabControlConfiguration.TabPages.Add(ConfigurationGROUPStabPage);
                        }
                        this.ConfigurationGROUPStabPage.Enabled = true;
                        tabControlConfiguration.SelectedTab = this.ConfigurationGROUPStabPage;
                    }
                    else
                    {
                        ListViewItem item1 = new ListViewItem("INFO", 0);
                        item1.SubItems.Add("UNDEFINED");
                        item1.SubItems.Add("NO GROUP DEFINITIONS IN CONFIG");
                        ConfigurationGROUPSlistView.Items.Add(item1);
                    }
                    ConfigurationGROUPSlistView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    ConfigurationGROUPSlistView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                    this.ConfigurationGROUPSpicBoxWait.Enabled = false;
                    this.ConfigurationGROUPSpicBoxWait.Visible  = false;
                }
                catch (Exception exp)
                {
                    Logger.error("main: ShowConfigGroup() - exception={0}", (object) exp.Message);
                    this.ConfigurationGROUPSpicBoxWait.Enabled = false;
                    this.ConfigurationGROUPSpicBoxWait.Visible  = false;
                }
                finally
                {
                    this.ConfigurationGROUPSpicBoxWait.Enabled = false;
                    this.ConfigurationGROUPSpicBoxWait.Visible  = false;
                }
            };

            if (InvokeRequired)
            {
                BeginInvoke(mi);
            }
            else
            {
                Invoke(mi);
            }
        }

        private void EnableButtons()
        {
            MethodInvoker mi = () =>
            {
                this.ConfigurationPanel1btnDeviceMode.Enabled = true;
                this.ConfigurationPanel1btnEMVMode.Enabled = true;
                this.ConfigurationPanel1pictureBox1.Enabled = false;
                this.ConfigurationPanel1pictureBox1.Visible = false;
            };

            if (InvokeRequired)
            {
                BeginInvoke(mi);
            }
            else
            {
                Invoke(mi);
            }
        }

        private void SetEmvButton(object payload)
        {
            MethodInvoker mi = () =>
            {
                string [] data = ((IEnumerable) payload).Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray();

                this.ConfigurationPanel1btnDeviceMode.Text = data[0];
                this.ConfigurationPanel1btnDeviceMode.Enabled = true;
                this.ApplicationpicBoxWait.Enabled = false;
                this.ApplicationpicBoxWait.Visible  = false;
            };

            if (InvokeRequired)
            {
                BeginInvoke(mi);
            }
            else
            {
                Invoke(mi);
            }
        }
        
        private void FirmwareUpdateProgress(object payload)
        {
            MethodInvoker mi = () =>
            {
                string [] data = ((IEnumerable) payload).Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray();
                this.FirmwareprogressBar1.PerformStep();
            };

            if (InvokeRequired)
            {
                BeginInvoke(mi);
            }
            else
            {
                Invoke(mi);
            }
        }

        private void FirmwareUpdateStatus(object payload)
        {
            MethodInvoker mi = () =>
            {
                string [] data = ((IEnumerable) payload).Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray();
                this.lblFirmwareVersion.Text = data[0];
            };

            if (InvokeRequired)
            {
                BeginInvoke(mi);
            }
            else
            {
                Invoke(mi);
            }
        }

        private void FirmwareUpdateFailed(object payload)
        {
            MethodInvoker mi = () =>
            {
                string [] data = ((IEnumerable) payload).Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray();
                this.lblFirmwareVersion.Text = data[0];
                Thread.Sleep(3000);
                this.btnFirmwareUpdate.Visible = true;
                this.btnFirmwareUpdate.Enabled = true;
                this.ApplicationpicBoxWait.Enabled = false;
                this.ApplicationpicBoxWait.Visible = false;
                this.FirmwareprogressBar1.Visible      = false;
            };

            if (InvokeRequired)
            {
                BeginInvoke(mi);
            }
            else
            {
                Invoke(mi);
            }
        }

        private void EnableMainForm(object payload)
        {
            MethodInvoker mi = () =>
            {
                string [] data = ((IEnumerable) payload).Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray();

                this.lblFirmwareVersion.Text = data[0];
                this.btnFirmwareUpdate.Visible = true;
                this.btnFirmwareUpdate.Enabled = false;
                this.ApplicationpicBoxWait.Enabled = false;
                this.ApplicationpicBoxWait.Visible = false;
                this.FirmwareprogressBar1.Visible      = false;
            };

            if (InvokeRequired)
            {
                BeginInvoke(mi);
            }
            else
            {
                Invoke(mi);
            }
        }

        private void ConfigurationResetLoadFromButtons()
        {
            foreach (RadioButton radio in ConfigurationGroupBox1.Controls.OfType<RadioButton>().ToList())
            {
                if (radio.Checked == true)
                {
                    radio.Checked = false;
                    break;
                }
            }
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab?.Name.Equals("ApplicationtabPage") ?? false)
            {
                // Configuration Mode
                this.Invoke(new MethodInvoker(() =>
                {
                    ConfigurationGROUPStabPagecomboBox1.SelectedIndex = -1;
                }));
            }
            else if (tabControlMain.SelectedTab?.Name.Equals("ConfigurationtabPage") ?? false)
            {
                ConfigurationResetLoadFromButtons();

                if(this.ConfigurationCollapseButton.Visible)
                {
                    this.ConfigurationCollapseButton.Visible = false;
                    this.ConfigurationPanel2.Visible = false;
                }
            }
            else if (tabControlMain.SelectedTab?.Name.Equals("FirmwaretabPage") ?? false)
            {
                // Configuration Mode
                this.Invoke(new MethodInvoker(() =>
                {
                    ConfigurationGROUPStabPagecomboBox1.SelectedIndex = -1;
                }));
            }
        }

        private void OnConfigurationListItemSelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlConfiguration.SelectedTab?.Name.Equals("ConfigurationTerminalDatatabPage") ?? false)
            {
                // Configuration Mode
                this.Invoke(new MethodInvoker(() =>
                {
                    this.ConfigurationTerminalDatapicBoxWait.Visible = true;
                    this.ConfigurationTerminalDatapicBoxWait.Enabled = true;
                    System.Windows.Forms.Application.DoEvents();
                    new Thread(() => { Thread.CurrentThread.IsBackground = true; devicePlugin.GetTerminalData(); }).Start();
                }));

                ConfigurationGROUPStabPagecomboBox1.SelectedIndex = -1;
            }
            else if (tabControlConfiguration.SelectedTab?.Name.Equals("ConfigurationAIDStabPage") ?? false)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    this.ConfigurationTerminalDatapicBoxWait.Visible = true;
                    this.ConfigurationTerminalDatapicBoxWait.Enabled = true;
                    System.Windows.Forms.Application.DoEvents();
                    new Thread(() => { Thread.CurrentThread.IsBackground = true; devicePlugin.GetAIDList(); }).Start();
                }));

                ConfigurationGROUPStabPagecomboBox1.SelectedIndex = -1;
            }
            else if (tabControlConfiguration.SelectedTab?.Name.Equals("ConfigurationCAPKStabPage") ?? false)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    this.ConfigurationCAPKSpicBoxWait.Visible = true;
                    this.ConfigurationCAPKSpicBoxWait.Enabled = true;
                    System.Windows.Forms.Application.DoEvents();
                    new Thread(() => { Thread.CurrentThread.IsBackground = true; devicePlugin.GetCapKList(); }).Start();
                }));

                ConfigurationGROUPStabPagecomboBox1.SelectedIndex = -1;
            }
            else if (tabControlConfiguration.SelectedTab?.Name.Equals("ConfigurationGROUPStabPage") ?? false)
            {
                ConfigurationGROUPStabPagecomboBox1.SelectedIndex = 0;
            }
        }

        private void OnConfigurationTabControlVisibilityChanged(object sender, EventArgs e)
        {
            if (this.ConfigurationTerminalDatatabPage.Visible == true)
            {
                this.tabControlConfiguration.SelectedIndex = -1;
                this.tabControlConfiguration.SelectedTab = this.ConfigurationTerminalDatatabPage;
                this.tabControlConfiguration.SelectedIndex = 0;
            }
        }

        private void OnCollapseConfigurationView(object sender, EventArgs e)
        {
            this.ConfigurationPanel1.Visible = false;
            this.ConfigurationExpandButton.Visible = true;
            this.tabControlConfiguration.Width -= CONFIG_PANEL_WIDTH;
            this.tabControlConfiguration.Width *= 2;
        }

        private void OnExpandConfigurationView(object sender, EventArgs e)
        {
            this.ConfigurationPanel1.Visible = true;
            this.ConfigurationExpandButton.Visible = false;
            this.tabControlConfiguration.Width /= 2;
            this.tabControlConfiguration.Width += CONFIG_PANEL_WIDTH;
        }

        private void OnLoadFromFile(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                // Load Configuration from FILE
                new Thread(() => devicePlugin.SetConfigurationMode(IPA.Core.Shared.Enums.ConfigurationModes.FROM_CONFIG)).Start();

                if (this.ConfigurationCollapseButton.Visible == false)
                {
                    this.ConfigurationCollapseButton.Visible = true;
                    this.ConfigurationPanel2.Visible = true;
                    this.tabControlConfiguration.SelectedTab = this.ConfigurationTerminalDatatabPage;
                }

                if (this.tabControlConfiguration.SelectedIndex == 0)
                {
                    this.tabControlConfiguration.SelectedIndex = -1;
                    this.tabControlConfiguration.SelectedTab = this.ConfigurationTerminalDatatabPage;
                    this.tabControlConfiguration.SelectedIndex = 0;
                }
            }
        }

        private void OnLoadFromDevice(object sender, EventArgs e)
        {
            if(((RadioButton)sender).Checked)
            {
                // Load Configuration from DEVICE
                new Thread(() => devicePlugin.SetConfigurationMode(IPA.Core.Shared.Enums.ConfigurationModes.FROM_DEVICE)).Start();

                if (this.ConfigurationCollapseButton.Visible == false)
                {
                    this.ConfigurationCollapseButton.Visible = true;
                    this.ConfigurationPanel2.Visible = true;
                    this.tabControlConfiguration.SelectedTab = this.ConfigurationTerminalDatatabPage;
                }

                if (this.tabControlConfiguration.SelectedIndex == 0)
                {
                    this.tabControlConfiguration.SelectedIndex = -1;
                    this.tabControlConfiguration.SelectedTab = this.ConfigurationTerminalDatatabPage;
                    this.tabControlConfiguration.SelectedIndex = 0;
                }
            }
        }

        private void OnLoadFactory(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                this.ConfigurationPanel1btnDeviceMode.Enabled = false;
                this.ConfigurationPanel1btnEMVMode.Enabled = false;
                if (this.ConfigurationCollapseButton.Visible)
                {
                    this.ConfigurationCollapseButton.Visible = false;
                    this.ConfigurationPanel2.Visible = false;
                }
                ConfigurationResetLoadFromButtons();
                this.ConfigurationPanel1pictureBox1.Enabled = true;
                this.ConfigurationPanel1pictureBox1.Visible = true;
                System.Windows.Forms.Application.DoEvents();
            }));

            // Reset Configuration to Factory defaults AND load from device
            new Thread(() => 
            { 
                Thread.CurrentThread.IsBackground = true; 
                devicePlugin.FactoryReset();
                // Load Configuration from DEVICE
                devicePlugin.SetConfigurationMode(IPA.Core.Shared.Enums.ConfigurationModes.FROM_DEVICE);
                this.Invoke(new MethodInvoker(() =>
                {
                    radioLoadFromDevice.Checked = true;
                }));
            }).Start();
        }

        private void OnSetDeviceMode(object sender, EventArgs e)
        {
            string mode = this.ConfigurationPanel1btnDeviceMode.Text;
            new Thread(() =>
            {
                try
                {
                    Thread.CurrentThread.IsBackground = true;
                    devicePlugin.SetDeviceMode(mode);
                }
                catch(Exception ex)
                {
                    Logger.error("main: exception={0}", (object)ex.Message);
                }

            }).Start();

            // Disable Buttons
            this.ConfigurationPanel1btnEMVMode.Enabled = false;
            this.btnFirmwareUpdate.Enabled = false;
        }

        private void OnEMVModeDisable(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    Thread.CurrentThread.IsBackground = true;
                    devicePlugin.DisableQCEmvMode();
                }
                catch(Exception ex)
                {
                    Logger.error("main: exception={0}", (object)ex.Message);
                }

            }).Start();

            // Disable Button
            this.ConfigurationPanel1btnEMVMode.Enabled = false;
        }

        private void OnConfigGroupSelectionChanged(object sender, EventArgs e)
        {
            if(ConfigurationGROUPStabPagecomboBox1.SelectedItem != null)
            {
                this.ConfigurationGROUPSpicBoxWait.Visible = true;
                this.ConfigurationGROUPSpicBoxWait.Enabled = true;
                int group = Convert.ToInt16(ConfigurationGROUPStabPagecomboBox1.SelectedItem.ToString());
                new Thread(() =>
                {
                    try
                    {
                        Thread.CurrentThread.IsBackground = true; 
                        devicePlugin.GetConfigGroup(group);
                    }
                    catch(Exception ex)
                    {
                        Logger.error("main: exception={0}", (object)ex.Message);
                    }

                }).Start();
            }
        }

        private void OnFirmwareUpdate(object sender, EventArgs e)
        {
            FirmwareopenFileDialog1.Title = "FIRMWARE UPDATE";
            FirmwareopenFileDialog1.Filter = "NGA FW Files|*.fm";
            FirmwareopenFileDialog1.InitialDirectory = System.IO.Directory.GetCurrentDirectory() + "\\Assets";

            if (FirmwareopenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                byte[] bytes = System.IO.File.ReadAllBytes(FirmwareopenFileDialog1.FileName);
                if(bytes.Length > 0)
                {
                    // Set the initial value of the ProgressBar.
	                this.FirmwareprogressBar1.Value = 0;
                    this.FirmwareprogressBar1.Maximum = bytes.Length / 1024;
                    this.FirmwareprogressBar1.Step = 1;

                    this.Invoke(new MethodInvoker(() =>
                    {
                        this.ApplicationpicBoxWait.Enabled = true;
                        this.ApplicationpicBoxWait.Visible  = true;
                        this.lblFirmwareVersion.Text = "UPDATING FIRMWARE (PLEASE DON'T INTERRUPT)...";
                        this.btnFirmwareUpdate.Visible = false;
                        this.FirmwareprogressBar1.Visible = true;
                        System.Windows.Forms.Application.DoEvents();
                    }));

                    // Firmware Update
                    new Thread(() =>
                    {
                        try
                        {
                            Thread.CurrentThread.IsBackground = true;
                            devicePlugin.FirmwareUpdate(FirmwareopenFileDialog1.FileName, bytes);
                        }
                        catch(Exception ex)
                        {
                            Logger.error("main: exception={0}", (object)ex.Message);
                        }

                    }).Start();
                }
            }
        }
    }
}

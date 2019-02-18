﻿using System;
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

            tabControl1.TabPages.Remove(tabPage2);
            tabControl1.TabPages.Remove(tabPage3);
            tabControl1.TabPages.Remove(tabPage4);
            tabControl1.TabPages.Remove(tabPage5);

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
                        this.progressBar1.Visible      = false;
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
                    this.picBoxConfigWait1.Enabled = false;
                    this.picBoxConfigWait1.Visible  = false;
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
                    this.picBoxConfigWait1.Enabled = true;
                    this.picBoxConfigWait1.Visible  = true;
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
                    this.button4.Text = data[0];
                    this.button4.Enabled = true;
                    this.button5.Enabled = (this.button4.Text.Equals(USK_DEVICE_MODE.USB_HID)) ? false : true;
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
                    string [] data = ((IEnumerable) payload).Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray();

                    // Remove previous entries
                    if(listView1.Items.Count > 0)
                    {
                        listView1.Items.Clear();
                    }

                    foreach(string val in data)
                    {
                        string [] tlv = val.Split(':');
                        ListViewItem item1 = new ListViewItem(tlv[0], 0);
                        item1.SubItems.Add(tlv[1]);
                        listView1.Items.Add(item1);
                    }

                    listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                    // TAB 2
                    if(!tabControl1.Contains(tabPage2))
                    {
                        tabControl1.TabPages.Add(tabPage2);
                    }
                    this.tabPage2.Enabled = true;
                    this.picBoxConfigWait2.Enabled = false;
                    this.picBoxConfigWait2.Visible  = false;
                    tabControl1.SelectedTab = this.tabPage2;
                    // TAB 3
                    if(!tabControl1.Contains(tabPage3))
                    {
                        tabControl1.TabPages.Add(tabPage3);
                    }
                    this.tabPage3.Enabled = true;
                    // TAB 4
                    if(!tabControl1.Contains(tabPage4))
                    {
                        tabControl1.TabPages.Add(tabPage4);
                    }
                    this.tabPage4.Enabled = true;
                    // TAB 5
                    if(!tabControl1.Contains(tabPage5))
                    {
                        tabControl1.TabPages.Add(tabPage5);
                    }
                    this.tabPage5.Enabled = true;
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
                    if(listView2.Items.Count > 0)
                    {
                        listView2.Items.Clear();
                    }

                    foreach(string item in data)
                    {
                        string [] components = item.Split('#');
                        if(components.Length == 2)
                        {
                            ListViewItem item1 = new ListViewItem(components[0], 0);
                            item1.SubItems.Add(components[1]);
                            listView2.Items.Add(item1);
                        }
                    }

                    listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                    if(!tabControl1.Contains(tabPage3))
                    {
                        tabControl1.TabPages.Add(tabPage3);
                    }
                    this.tabPage3.Enabled = true;
                    tabControl1.SelectedTab = this.tabPage3;
                }
                catch (Exception exp)
                {
                    Logger.error("main: ShowAIDData() - exception={0}", (object) exp.Message);
                }
                finally
                {
                    this.picBoxConfigWait3.Enabled = false;
                    this.picBoxConfigWait3.Visible  = false;
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
                    string [] data = ((IEnumerable) payload).Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray();

                    // Remove previous entries
                    if(listView3.Items.Count > 0)
                    {
                        listView3.Items.Clear();
                    }

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
                                listView3.Items.Add(item1);
                            }
                        }
                    }

                    listView3.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    listView3.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                    if(!tabControl1.Contains(tabPage4))
                    {
                        tabControl1.TabPages.Add(tabPage4);
                    }
                    this.tabPage4.Enabled = true;
                    tabControl1.SelectedTab = this.tabPage4;
                }
                catch (Exception exp)
                {
                    Logger.error("main: ShowCapKList() - exception={0}", (object) exp.Message);
                }
                finally
                {
                    this.picBoxConfigWait4.Enabled = false;
                    this.picBoxConfigWait4.Visible  = false;
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
                    string [] data = ((IEnumerable) payload).Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray();

                    // Remove previous entries
                    if(listView4.Items.Count > 0)
                    {
                        listView4.Items.Clear();
                    }

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
                                    listView4.Items.Add(item1);
                                }
                            }
                        }
                    }

                    listView4.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    listView4.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                    if(!tabControl1.Contains(tabPage5))
                    {
                        tabControl1.TabPages.Add(tabPage5);
                    }
                    this.tabPage5.Enabled = true;
                    tabControl1.SelectedTab = this.tabPage5;
                    this.picBoxConfigWait5.Enabled = false;
                    this.picBoxConfigWait5.Visible  = false;
                }
                catch (Exception exp)
                {
                    Logger.error("main: ShowConfigGroup() - exception={0}", (object) exp.Message);
                    this.picBoxConfigWait5.Enabled = false;
                    this.picBoxConfigWait5.Visible  = false;
                }
                finally
                {
                    this.picBoxConfigWait4.Enabled = false;
                    this.picBoxConfigWait4.Visible  = false;
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
                this.button1.Enabled = true;
                this.button2.Enabled = true;
                this.button3.Enabled = true;

                this.picBoxConfigWait1.Enabled = false;
                this.picBoxConfigWait1.Visible  = false;
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

                this.button5.Text = data[0];
                this.button5.Enabled = true;
                this.picBoxConfigWait1.Enabled = false;
                this.picBoxConfigWait1.Visible  = false;
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
                this.progressBar1.PerformStep();
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

        private void EnableMainForm(object payload)
        {
            MethodInvoker mi = () =>
            {
                string [] data = ((IEnumerable) payload).Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray();

                this.lblFirmwareVersion.Text = data[0];
                this.btnFirmwareUpdate.Visible = true;
                this.btnFirmwareUpdate.Enabled = false;
                this.picBoxConfigWait1.Enabled = false;
                this.picBoxConfigWait1.Visible = false;
                this.progressBar1.Visible      = false;
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

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Name.Equals("tabPage1"))
            {
                // Configuration Mode
                this.Invoke(new MethodInvoker(() =>
                {
                    if(tabControl1.Contains(tabPage2))
                    {
                        tabControl1.TabPages.Remove(tabPage2);
                    }
                    if(tabControl1.Contains(tabPage3))
                    {
                        tabControl1.TabPages.Remove(tabPage3);
                    }
                    if(tabControl1.Contains(tabPage4))
                    {
                        tabControl1.TabPages.Remove(tabPage4);
                    }
                    if(tabControl1.Contains(tabPage5))
                    {
                        tabControl1.TabPages.Remove(tabPage5);
                    }
                }));
            }
            else if (tabControl1.SelectedTab.Name.Equals("tabPage2"))
            {
                // Configuration Mode
                this.Invoke(new MethodInvoker(() =>
                {
                    this.picBoxConfigWait2.Visible = true;
                    this.picBoxConfigWait2.Enabled = true;
                    System.Windows.Forms.Application.DoEvents();
                    new Thread(() => { Thread.CurrentThread.IsBackground = true; devicePlugin.GetTerminalData(); } ).Start();
                }));
            }
            else if (tabControl1.SelectedTab.Name.Equals("tabPage3"))
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    this.picBoxConfigWait3.Visible = true;
                    this.picBoxConfigWait3.Enabled = true;
                    System.Windows.Forms.Application.DoEvents();
                    new Thread(() => { Thread.CurrentThread.IsBackground = true; devicePlugin.GetAIDList(); } ).Start();
                }));
            }
            else if (tabControl1.SelectedTab.Name.Equals("tabPage4"))
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    this.picBoxConfigWait4.Visible = true;
                    this.picBoxConfigWait4.Enabled = true;
                    System.Windows.Forms.Application.DoEvents();
                    new Thread(() => { Thread.CurrentThread.IsBackground = true; devicePlugin.GetCapKList(); } ).Start();
                }));
            }
            else if (tabControl1.SelectedTab.Name.Equals("tabPage5"))
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Load Configuration from FILE
            new Thread(() => devicePlugin.SetConfigurationMode(IPA.Core.Shared.Enums.ConfigurationModes.FROM_CONFIG)).Start();

            this.Invoke(new MethodInvoker(() =>
            {
                if(!tabControl1.Contains(tabPage2))
                {
                    tabControl1.TabPages.Add(tabPage2);
                }
                this.tabPage2.Enabled = true;
                tabControl1.SelectedTab = this.tabPage2;
                // FILE load modifies DEVICE configuration
                this.button2.Enabled = false;
                this.picBoxConfigWait2.Visible = true;
                this.picBoxConfigWait2.Enabled = true;
                System.Windows.Forms.Application.DoEvents();
            }));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Load Configuration from DEVICE
            new Thread(() => devicePlugin.SetConfigurationMode(IPA.Core.Shared.Enums.ConfigurationModes.FROM_DEVICE)).Start();

            this.Invoke(new MethodInvoker(() =>
            {
                if(!tabControl1.Contains(tabPage2))
                {
                    tabControl1.TabPages.Add(tabPage2);
                }
                this.tabPage2.Enabled = true;
                tabControl1.SelectedTab = this.tabPage2;
            }));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Reset Configuration to Factory defaults
            new Thread(() => { Thread.CurrentThread.IsBackground = true; devicePlugin.FactoryReset(); } ).Start();

            this.Invoke(new MethodInvoker(() =>
            {
                this.button1.Enabled = false;
                this.button2.Enabled = false;
                this.button3.Enabled = false;
                this.picBoxConfigWait1.Enabled = true;
                this.picBoxConfigWait1.Visible  = true;
                System.Windows.Forms.Application.DoEvents();
            }));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string mode = this.button4.Text;
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
            this.button4.Enabled = false;
            this.button5.Enabled = false;
        }

        private void button5_Click(object sender, EventArgs e)
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
            this.button5.Enabled = false;
        }

        private void OnConfigGroupSelectionChanged(object sender, EventArgs e)
        {
            this.picBoxConfigWait5.Visible = true;
            this.picBoxConfigWait5.Enabled = true;
            int group = Convert.ToInt16(comboBox1.SelectedItem.ToString());
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

        private void button6_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "FIRMWARE UPDATE";
            openFileDialog1.Filter = "NGA FW Files|*.fm";
            openFileDialog1.InitialDirectory = System.IO.Directory.GetCurrentDirectory() + "\\Assets";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                byte[] bytes = System.IO.File.ReadAllBytes(openFileDialog1.FileName);
                if(bytes.Length > 0)
                {
                    // Set the initial value of the ProgressBar.
	                this.progressBar1.Value = 0;
                    this.progressBar1.Maximum = bytes.Length / 1024;
                    this.progressBar1.Step = 1;

                    // Firmware Update
                    new Thread(() =>
                    {
                        try
                        {
                            Thread.CurrentThread.IsBackground = true;
                            devicePlugin.FirmwareUpdate(openFileDialog1.FileName, bytes);
                        }
                        catch(Exception ex)
                        {
                            Logger.error("main: exception={0}", (object)ex.Message);
                        }

                    }).Start();

                    this.Invoke(new MethodInvoker(() =>
                    {
                        this.picBoxConfigWait1.Enabled = true;
                        this.picBoxConfigWait1.Visible  = true;
                        this.lblFirmwareVersion.Text = "UPDATING FIRMWARE (PLEASE DON'T INTERRUPT)...";
                        this.btnFirmwareUpdate.Visible = false;
                        this.progressBar1.Visible = true;
                        System.Windows.Forms.Application.DoEvents();
                    }));
                }
            }
        }
    }
}

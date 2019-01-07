﻿using IPA.CommonInterface;
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

namespace IDTechConfigReader
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        // Always on TOP
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        IDevicePlugIn devicePlugin;

        public Form1()
        {
            InitializeComponent();

            tabControl1.TabPages.Remove(tabPage2);
            tabControl1.TabPages.Remove(tabPage3);
            tabControl1.TabPages.Remove(tabPage4);

            InitalizeDevice();
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.MaximizeBox = false;

            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }

        private void InitalizeDevice()
        {
            try
            {
                devicePlugin = new IPA.DAL.RBADAL.DeviceCfg() as IDevicePlugIn;
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    Debug.WriteLine("\nmain: new device detected! +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n");

                    devicePlugin.OnDeviceNotification += new EventHandler<DeviceNotificationEventArgs>(this.OnDeviceNotificationUI);
                    devicePlugin.DeviceInit();
                }).Start();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("main: Initalize() - exception={0}", (object) exp.Message);
            }
        }

        protected void OnDeviceNotificationUI(object sender, DeviceNotificationEventArgs args)
        {
            Debug.WriteLine("device: notification type={0}", args.NotificationType);

            switch (args.NotificationType)
            {
                case NOTIFICATION_TYPE.NT_INITIALIZE_DEVICE:
                {
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

                case NOTIFICATION_TYPE.NT_UI_ENABLE_BUTTONS:
                {
                    EnableButtonsUI(sender, args);
                    break;
                }
            }
        }

        private void ShowTerminalDataUI(object sender, DeviceNotificationEventArgs e)
        {
            ShowTerminalData(e.Message);
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
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("main: ShowTerminalData() - exception={0}", (object) exp.Message);
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

        private void ShowAidListUI(object sender, DeviceNotificationEventArgs e)
        {
            ShowAidList(e.Message);
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
                    Debug.WriteLine("main: ShowAIDData() - exception={0}", (object) exp.Message);
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

        private void ShowCapKListUI(object sender, DeviceNotificationEventArgs e)
        {
            ShowCapKList(e.Message);
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
                    Debug.WriteLine("main: ShowCapKList() - exception={0}", (object) exp.Message);
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

        private void EnableButtonsUI(object sender, DeviceNotificationEventArgs e)
        {
            EnableButtons();
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
    }
}

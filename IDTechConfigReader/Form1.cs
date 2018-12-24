using IPA.CommonInterface;
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

                    foreach(string val in data)
                    {
                        string [] tlv = val.Split(':');
                        ListViewItem item1 = new ListViewItem(tlv[0], 0);
                        item1.SubItems.Add(tlv[1]);
                        listView1.Items.Add(item1);
                    }

                    listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
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

                    foreach(string val in data)
                    {
                        string [] tlv = val.Split('#');
                        ListViewItem item1 = new ListViewItem(tlv[0], 0);
                        item1.SubItems.Add(tlv[1]);
                        listView2.Items.Add(item1);
                    }

                    listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("main: ShowAIDData() - exception={0}", (object) exp.Message);
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

                    foreach(string val in data)
                    {
                        string [] tlv = val.Split('#');
                        ListViewItem item1 = new ListViewItem(tlv[0], 0);
                        item1.SubItems.Add(tlv[1]);
                        listView3.Items.Add(item1);
                    }

                    listView3.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    listView3.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("main: ShowCapKList() - exception={0}", (object) exp.Message);
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

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Name.Equals("tabPage1"))
            {
                // Configuration Mode
                this.Invoke(new MethodInvoker(() =>
                {
                    this.tabPage1.Enabled = true;
                }));
            }
            else if (tabControl1.SelectedTab.Name.Equals("tabPage2"))
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    devicePlugin.GetAIDList();
                }));
            }
            else if (tabControl1.SelectedTab.Name.Equals("tabPage3"))
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    devicePlugin.GetCapKList();
                }));
            }
        }
    }
}

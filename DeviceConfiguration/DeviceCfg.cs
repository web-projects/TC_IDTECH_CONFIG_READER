using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Management;
using System.Threading.Tasks;
using System.Reflection;
using IDTechSDK;

using IPA.CommonInterface;

namespace IPA.DAL.RBADAL
{
  [Serializable]
  public class DeviceCfg : MarshalByRefObject, IDevicePlugIn
  {
    /********************************************************************************************************/
    // ATTRIBUTES
    /********************************************************************************************************/
     #region -- attributes --

    private IDTechSDK.IDT_DEVICE_Types deviceType;
    private DEVICE_INTERFACE_Types     deviceConnect;
    private DEVICE_PROTOCOL_Types      deviceProtocol;

    // Device Events back to Main Form
    public event EventHandler<NotificationEventArgs> OnDeviceNotification;

    private bool useUniversalSDK;
    private bool attached;
    private bool connected;
    private bool formClosing;

    private readonly object discoveryLock = new object();

    internal static System.Timers.Timer MSRTimer { get; set; }

    private string DevicePluginName;
    public string PluginName { get { return DevicePluginName; } }

    // Configuration handler
    ConfigSerializer serializer;

    // EMV Transactions
    int exponent;
    byte[] additionalTags;
    string amount;

    #endregion

    /********************************************************************************************************/
    // CONSTRUCTION AND INITIALIZATION
    /********************************************************************************************************/
    #region -- construction and initialization --

    public DeviceCfg()
    {
      deviceType = IDT_DEVICE_Types.IDT_DEVICE_NONE;
///      cardReader = new CardReader();
    }

    public void DeviceInit()
    {
      DevicePluginName = "DeviceCfg";

      // Device Discovery
      connected = false;

      // Update Device Configuration
      string [] message = { "COMPLETED" };
      NotificationRaise(new NotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_INITIALIZE_DEVICE, Message = message });

      serializer = new ConfigSerializer();
      serializer.ReadConfig();
      message = serializer.GetTerminalData();
      NotificationRaise(new NotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_SHOW_TERMINAL_DATA, Message = message });
    }

    public ConfigSerializer GetConfigSerializer()
    {
        return serializer;
    }

    #endregion

    /********************************************************************************************************/
    // MAIN INTERFACE
    /********************************************************************************************************/
    #region -- main interface --

    public void SetFormClosing(bool state)
    {
      formClosing = state;
    }
    
    public void NotificationRaise(NotificationEventArgs e)
    {
        OnDeviceNotification?.Invoke(null, e);
    }

    #endregion

    /********************************************************************************************************/
    // CONFIGURATION ACTIONS
    /********************************************************************************************************/
    #region -- configuration actions --

    public void GetTerminalData()
    {
    }

        public string[] GetConfig()
        {
            throw new NotImplementedException();
        }

        public void GetCardData()
        {
            throw new NotImplementedException();
        }

        public void GetDeviceConfiguration()
        {
            throw new NotImplementedException();
        }

        public void SetDeviceConfiguration(object data)
        {
            throw new NotImplementedException();
        }

        public void SetDeviceMode(string mode)
        {
            throw new NotImplementedException();
        }

        public string DeviceCommand(string command, bool notify)
        {
            throw new NotImplementedException();
        }
        public string GetErrorMessage(string data)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

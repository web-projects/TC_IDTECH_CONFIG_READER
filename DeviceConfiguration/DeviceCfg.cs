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

    private RETURN_CODE SetupVivoCommand(byte[] command, out byte[] response)
    {
        response = new byte[1];
        return RETURN_CODE.RETURN_CODE_DO_SUCCESS;
    }

    private RETURN_CODE SetVP3000DeviceHidMode()
    {
        RETURN_CODE rt = RETURN_CODE.RETURN_CODE_DO_SUCCESS;
        // POLL COMMAND REPORT:    "V     i     V     O     t     e     c     h     2"         CC    SC    LM    LL    DT    CL    CM
        byte[] command = { 0x01, 0x56, 0x69, 0x56, 0x4f, 0x74, 0x65, 0x63, 0x68, 0x32, 0x00, 0x01, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00 };
        var payload = new byte[64];
        Array.Copy(command, 1, payload, 0, command.Length - 1);
        Crc16Ccitt crc16 = new Crc16Ccitt(InitialCrcValue.NONZERO1);

        // TEST ONLY: SHOULD RETURN byte[] { 0xB3, 0xCD }
        //byte[] crcBytes = crc16.ComputeChecksumBytes(new byte[]{ 0x56, 0x69, 0x56, 0x4f, 0x74, 0x65, 0x63, 0x68, 0x32, 0x00, 0x18, 0x01, 0x00, 0x00 });

        byte[] crcBytes = crc16.ComputeChecksumBytes(payload);
        command[16] = crcBytes[0];
        command[17] = crcBytes[1];

        var readConfig = new byte[64];
        Array.Copy(command, readConfig, command.Length);

        byte[] result;
        var status = SetupVivoCommand(readConfig, out result);

        return rt;
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

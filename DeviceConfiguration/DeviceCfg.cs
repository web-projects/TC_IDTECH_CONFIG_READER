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
using HidLibrary;
using IDTechSDK;

using IPA.CommonInterface;
using IPA.CommonInterface.Helpers;
using IPA.Core.Shared.Helpers.StatusCode;
using IPA.Core.Shared.Enums;
using IPA.DAL;
using IPA.DAL.RBADAL.Services;
using IPA.DAL.RBADAL.Services.Devices.IDTech;
using IPA.DAL.RBADAL.Services.Devices.IDTech.Models;
using IPA.LoggerManager;

namespace IPA.DAL.RBADAL
{
  [Serializable]
  public class DeviceCfg : MarshalByRefObject, IDevicePlugIn
  {
    /********************************************************************************************************/
    // ATTRIBUTES
    /********************************************************************************************************/
     #region -- attributes --

    HidDevice device;
    Device Device = new Device();

    private IDTechSDK.IDT_DEVICE_Types deviceType;
    private DEVICE_INTERFACE_Types     deviceConnect;
    private DEVICE_PROTOCOL_Types      deviceProtocol;

    private static DeviceInformation deviceInformation;

    // Device Events back to Main Form
    public event EventHandler<DeviceNotificationEventArgs> OnDeviceNotification;

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

    // configuration modes
    IPA.Core.Shared.Enums.ConfigurationModes configurationMode = ConfigurationModes.FROM_CONFIG;

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

      // Create Device info object
      deviceInformation = new DeviceInformation();

      try
      {
          // Initialize Device
          device = HidDevices.Enumerate(Device_IDTech.IDTechVendorID).FirstOrDefault();

          if (device != null)
          {
            // Get Capabilities
            Debug.WriteLine("");
            Debug.WriteLine("device capabilities ----------------------------------------------------------------");
			Debug.WriteLine("  Usage                          : " + Convert.ToString(device.Capabilities.Usage, 16));
			Debug.WriteLine("  Usage Page                     : " + Convert.ToString(device.Capabilities.UsagePage, 16));
			Debug.WriteLine("  Input Report Byte Length       : " + device.Capabilities.InputReportByteLength);
			Debug.WriteLine("  Output Report Byte Length      : " + device.Capabilities.OutputReportByteLength);
			Debug.WriteLine("  Feature Report Byte Length     : " + device.Capabilities.FeatureReportByteLength);
			Debug.WriteLine("  Number of Link Collection Nodes: " + device.Capabilities.NumberLinkCollectionNodes);
			Debug.WriteLine("  Number of Input Button Caps    : " + device.Capabilities.NumberInputButtonCaps);
			Debug.WriteLine("  Number of Input Value Caps     : " + device.Capabilities.NumberInputValueCaps);
			Debug.WriteLine("  Number of Input Data Indices   : " + device.Capabilities.NumberInputDataIndices);
			Debug.WriteLine("  Number of Output Button Caps   : " + device.Capabilities.NumberOutputButtonCaps);
			Debug.WriteLine("  Number of Output Value Caps    : " + device.Capabilities.NumberOutputValueCaps);
			Debug.WriteLine("  Number of Output Data Indices  : " + device.Capabilities.NumberOutputDataIndices);
			Debug.WriteLine("  Number of Feature Button Caps  : " + device.Capabilities.NumberFeatureButtonCaps);
			Debug.WriteLine("  Number of Feature Value Caps   : " + device.Capabilities.NumberFeatureValueCaps);
			Debug.WriteLine("  Number of Feature Data Indices : " + device.Capabilities.NumberFeatureDataIndices);

            // Using the device notifier to detect device removed event
            device.Removed += DeviceRemovedHandler;
            Device.OnNotification += OnNotification;

            Device.Init(SerialPortService.GetAvailablePorts(), ref deviceInformation.deviceMode);

            // Notify Main Form
            SetDeviceMode(deviceInformation.deviceMode);

            if(deviceInformation.emvConfigSupported)
            {
                Debug.WriteLine("DeviceCfg::DeviceInit(): - device TYPE={0}", IDT_Device.getDeviceType());

                // Initialize Universal SDK
                IDT_Device.setCallback(MessageCallBack);
                //IDT_Device.setCallbackIP(MessageCallBackIP);
                IDT_Device.startUSBMonitoring();
            }
            else
            {
                // connect to device
                Device.Connect();
            }
            // Set as Attached
            attached = true;
          }
          else
          {
              throw new Exception("NoDevice");
          }
      }
      catch (Exception xcp)
      {
          throw xcp;
      }

      // Update Device Configuration
      string [] message = { "COMPLETED" };
      NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_INITIALIZE_DEVICE, Message = message });

      // Initialize Configuration Serializer
      serializer = new ConfigSerializer();
      serializer.ReadConfig();
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
      if(formClosing)
      {
            if(deviceInformation.emvConfigSupported)
            {
                Debug.WriteLine("DeviceCfg::DISCONNECTING FOR device TYPE={0}", IDT_Device.getDeviceType());
                Device.CloseDevice();
            }
      }
    }
    
    public void NotificationRaise(DeviceNotificationEventArgs e)
    {
        OnDeviceNotification?.Invoke(null, e);
    }

    #endregion

    /********************************************************************************************************/
    // DEVICE EVENTS INTERFACE
    /********************************************************************************************************/
    #region -- device event interface ---

    private void DeviceRemovedHandler()
    {
      Debug.WriteLine("\ndevice: removed !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\n");

      connected = false;

      if(attached)
      {
          attached = false;
          
          // Last device was USDK Type
          if(useUniversalSDK)
          {
              IDT_Device.stopUSBMonitoring();
          }

          // Unload Device Domain
          NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_UNLOAD_DEVICE_CONFIGDOMAIN });
      }
    }

    private void DeviceAttachedHandler()
    {
      Debug.WriteLine("device: attached ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
    }

    protected void OnNotification(object sender, Models.NotificationEventArgs args)
    {
        Debug.WriteLine("device: notification type={0}, event={1}", args.NotificationType, args.DeviceEvent);

        switch (args.NotificationType)
        {
            case NotificationType.DeviceEvent:
            {
                switch(args.DeviceEvent)
                {
                    case DeviceEvent.DeviceDisconnected:
                    {
                        DeviceRemovedHandler();
                        break;
                    }
                    case DeviceEvent.CardReadComplete:
                    {
                        /*Core.Data.Entity.Other.CreditCard cc = Transaction.PaymentXO.Request.CreditCard;
                        string last4 = cc.EncryptedTracks;
                        NotificationRaise(
                            new DeviceNotificationEventArgs()
                            {
                                NotificationType = NOTIFICATION_TYPE.NT_PROCESS_CARDDATA,
                                Message = new object[] { "Card Read " + last4 },
                            }
                        );*/
                        break;
                    }
                }
                break;
            }
        }
    }

    #endregion

    /********************************************************************************************************/
    // UNIVERSAL SDK INTERFACE
    /********************************************************************************************************/
    #region -- universal sdk interface --

    private void SetDeviceConfig()
    {
      if(IDT_DEVICE_Types.IDT_DEVICE_NONE != deviceType)
      {
           object [] settings = { deviceType, deviceConnect };

           Device.Configure(settings);

           // Create Device info object
           if(deviceInformation == null)
           {
              deviceInformation = new DeviceInformation();
           }

           DeviceInfo devInfo = Device.GetDeviceInfo();

           if(devInfo != null)
           {
              deviceInformation.SerialNumber     = devInfo.SerialNumber;
              deviceInformation.FirmwareVersion  = devInfo.FirmwareVersion;
              deviceInformation.EMVKernelVersion = devInfo.EMVKernelVersion;
              deviceInformation.ModelName        = devInfo.ModelName;
              deviceInformation.ModelNumber      = devInfo.ModelNumber;
              deviceInformation.Port             = devInfo.Port;
           }

           // Update Device Configuration
           object [] message = { "COMPLETED" };
           NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_DEVICE_UPDATE_CONFIG, Message = message });
      }
    }

    private void MessageCallBack(IDTechSDK.IDT_DEVICE_Types type, DeviceState state, byte[] data, IDTTransactionData cardData, EMV_Callback emvCallback, RETURN_CODE transactionResultCode)
    {
      // Setup Connection
      IDTechComm comm = Profile.getComm(type, DEVICE_INTERFACE_Types.DEVICE_INTERFACE_USB);

      if (comm == null)
      {
        comm = Profile.getComm(type, DEVICE_INTERFACE_Types.DEVICE_INTERFACE_SERIAL);
      }

      if (comm == null)
      {
        comm = Profile.getComm(type, DEVICE_INTERFACE_Types.DEVICE_INTERFACE_BT);
      }

      if (comm == null)
      {
        comm = Profile.getComm(type, DEVICE_INTERFACE_Types.DEVICE_INTERFACE_AUDIO_JACK);
      }
      
      deviceConnect = DEVICE_INTERFACE_Types.DEVICE_INTERFACE_UNKNOWN;
      deviceProtocol = DEVICE_PROTOCOL_Types.DEVICE_PROTOCOL_UNKNOWN;

      if (comm != null)
      {
          deviceConnect  = comm.getDeviceConnection();
          deviceProtocol = comm.getDeviceProtocol();
      }

      //Debug.WriteLine("device discovery: state={0}", state);

      switch (state)
      {
        case DeviceState.ToConnect:
        {
          deviceType = type;
          Debug.WriteLine("device connecting: {0}", (object) deviceType.ToString());
          break;
        }

        case DeviceState.Connected:
        {
          deviceType = type;
          Debug.WriteLine("device connected: {0}", (object) IDTechSDK.Profile.IDT_DEVICE_String(type, deviceConnect));

          // Populate Device Configuration
          new Thread(() =>
          {
             Thread.CurrentThread.IsBackground = false;

             // Get Device Configuration
             SetDeviceConfig();

             Thread.Sleep(100);
             connected = true;

             // Get Device Information
             ///GetDeviceInformation();

          }).Start();

          break;
        }

        case DeviceState.DefaultDeviceTypeChange:
        {
          break;
        }

        case DeviceState.Notification:
        {
            if (cardData.Notification == EVENT_NOTIFICATION_Types.EVENT_NOTIFICATION_Card_Not_Seated)
            {
                Debug.WriteLine("Notification: ICC Card not Seated\n");
            }
            if (cardData.Notification == EVENT_NOTIFICATION_Types.EVENT_NOTIFICATION_Card_Seated)
            {
                Debug.WriteLine("Notification: ICC Card Seated\n");
            }
            if (cardData.Notification == EVENT_NOTIFICATION_Types.EVENT_NOTIFICATION_Swipe_Card)
            {
                Debug.WriteLine("Notification: MSR Swipe Card\n");
            }

            break;
        }

        case DeviceState.TransactionData:
        {
          if (cardData == null) 
          {
              break;
          }

          //lastCardData = cardData;

          if (type == IDT_DEVICE_Types.IDT_DEVICE_AUGUSTA && deviceProtocol == DEVICE_PROTOCOL_Types.DEVICE_PROTOCOL_KB)
          {
              if (cardData.msr_rawData != null)
              {
                  if (cardData.msr_rawData.Length == 1 && cardData.msr_rawData[0] == 0x18)
                  {
                      Debug.WriteLine("Get MSR Complete! \n");
                      Debug.WriteLine("Get MSR Complete! \n");
                  }
              }

              //clearCallbackData(ref data, ref cardData);

              return;
          }

          if (cardData.Event != EVENT_TRANSACTION_DATA_Types.EVENT_TRANSACTION_PIN_DATA       && 
              cardData.Event != EVENT_TRANSACTION_DATA_Types.EVENT_TRANSACTION_DATA_CARD_DATA && 
              cardData.Event != EVENT_TRANSACTION_DATA_Types.EVENT_TRANSACTION_DATA_EMV_DATA)
          {
             //SoftwareController.MSR_LED_RED_SOLID();
             Debug.WriteLine("MSR Error " + cardData.msr_errorCode.ToString() + "\n");
             Debug.WriteLine("MSR Error " + cardData.msr_errorCode.ToString());
          }
          else
          {
            if (cardData.Event != EVENT_TRANSACTION_DATA_Types.EVENT_TRANSACTION_DATA_EMV_DATA)
            {
              //SoftwareController.MSR_LED_GREEN_SOLID();
            }

            //output parsed card data
            Debug.WriteLine("Return Code: " + transactionResultCode.ToString() + "\r\n");

            // Data Received Processing
            ///ProcessCardData(cardData);
          }

          break;
        }

        case DeviceState.DataReceived: 
        {
          //SetOutputTextLog(GetTimestamp() +  " IN: " + Common.getHexStringFromBytes(data));
          break;
        }

        case DeviceState.DataSent:
        {
          //SetOutputTextLog(GetTimestamp() + " OUT: " + Common.getHexStringFromBytes(data));
          break;
        }

        case DeviceState.CommandTimeout:
        {
          Debug.WriteLine("Command Timeout\n");
          break;
        }

        case DeviceState.CardAction:
        {
          if (data != null & data.Length > 0)
          {
              CARD_ACTION action = (CARD_ACTION)data[0];
              StringBuilder sb = new StringBuilder("Card Action Request: ");

              if ((action & CARD_ACTION.CARD_ACTION_INSERT) == CARD_ACTION.CARD_ACTION_INSERT)
              {
                sb.Append("INSERT ");
              }

              if ((action & CARD_ACTION.CARD_ACTION_REINSERT) == CARD_ACTION.CARD_ACTION_REINSERT)
              {
                sb.Append("REINSERT ");
              }

              if ((action & CARD_ACTION.CARD_ACTION_REMOVE) == CARD_ACTION.CARD_ACTION_REMOVE)
              {  
                sb.Append("REMOVE ");
              }

              if ((action & CARD_ACTION.CARD_ACTION_SWIPE) == CARD_ACTION.CARD_ACTION_SWIPE)
              {  
                sb.Append("SWIPE ");
              }

              if ((action & CARD_ACTION.CARD_ACTION_SWIPE_AGAIN) == CARD_ACTION.CARD_ACTION_SWIPE_AGAIN)
              {  
                sb.Append("SWIPE_AGAIN ");
              }

              if ((action & CARD_ACTION.CARD_ACTION_TAP) == CARD_ACTION.CARD_ACTION_TAP)
              {  
                sb.Append("TAP ");
              }

              if ((action & CARD_ACTION.CARD_ACTION_TAP_AGAIN) == CARD_ACTION.CARD_ACTION_TAP_AGAIN)
              {  
                sb.Append("TAP_AGAIN ");
              }

              Debug.WriteLine(sb.ToString() + "\n");
          }

          break;
        }

        case DeviceState.MSRDecodeError:
        {
          //SoftwareController.MSR_LED_RED_SOLID();
          Debug.WriteLine("MSR Decode Error\n");
          break;
        }

        case DeviceState.SwipeTimeout:
        {
          Debug.WriteLine("Swipe Timeout\n");
          break;
        }

        case DeviceState.TransactionCancelled:
        {
          Debug.WriteLine("TransactionCancelled.");
          //Debug.WriteLine("");
          //Debug.WriteLine(DeviceTerminalInfo.getDisplayMessage(DeviceTerminalInfo.MSC_ID_WELCOME));
          break;
        }

        case DeviceState.DeviceTimeout:
        {
          Debug.WriteLine("Device Timeout\n");
          break;
        }

        case DeviceState.TransactionFailed:
        {
          if ((int)transactionResultCode == 0x8300)
          {
            //SoftwareController.MSR_LED_RED_SOLID();
          }

          string text =  IDTechSDK.errorCode.getErrorString(transactionResultCode);
          Debug.WriteLine("Transaction Failed: {0}\r\n", (object) text);

          // Allow for GUI Recovery
          string [] message = { "" };
          message[0] = "***** TRANSACTION FAILED: " + text + " *****";
          NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_PROCESS_CARDDATA_ERROR, Message = message });
          break;
        }
      }
    }
    
    private void MessageCallBackIP(IDTechSDK.IDT_DEVICE_Types type, DeviceState state, byte[] data, IDTTransactionData cardData, EMV_Callback emvCallback, RETURN_CODE transactionResultCode, string IP2)
    {
        MessageCallBack(type, state, data, cardData, emvCallback, transactionResultCode);
    }
    #endregion

    /********************************************************************************************************/
    // CONFIGURATION ACTIONS
    /********************************************************************************************************/
    #region -- configuration actions --

    public void SetConfigurationMode(IPA.Core.Shared.Enums.ConfigurationModes mode)
    {
        configurationMode = mode;
    }

    public void GetTerminalData()
    {
        string [] message = null;
        if(configurationMode == ConfigurationModes.FROM_DEVICE)
        {
            message = Device.GetTerminalData();
        }
        else
        {
            if(serializer == null)
            {
                serializer = new ConfigSerializer();
                serializer.ReadConfig();        
            }

            Device.ValidateTerminalData(serializer);

            message = serializer.GetTerminalDataString(deviceInformation.SerialNumber, deviceInformation.EMVKernelVersion);
        }

        NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_SHOW_TERMINAL_DATA, Message = message });
    }

    public void GetAIDList()
    {
        string [] message = null;
        if(configurationMode == ConfigurationModes.FROM_DEVICE)
        {
            message = Device.GetAidList();
        }
        else
        {
            Device.ValidateAidList(serializer);
            message = serializer.GetAIDCollection();
        }

        NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_SHOW_AID_LIST, Message = message });
    }

    public void GetCapKList()
    {
        string [] message = null;
        if(configurationMode == ConfigurationModes.FROM_DEVICE)
        {
            message = Device.GetCapKList();
        }
        else
        {
            Device.ValidateCapKList(serializer);
            message = serializer.GetCapKCollection();
        }

        NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_SHOW_CAPK_LIST, Message = message });
    }

    public void GetConfigGroup(int group)
    {
        string [] message = null;
        if(configurationMode == ConfigurationModes.FROM_DEVICE)
        {
           message = Device.GetConfigGroup(group);
        }
        else
        {
            Device.ValidateConfigGroup(serializer, group);
            message = serializer.GetConfigGroupCollection(group);
        }

        NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_SHOW_CONFIG_GROUP, Message = message });
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

    private void SetDeviceMode(IDTECH_DEVICE_PID mode)
    {
        object [] message = { "" };

        switch(mode)
        {
            case IDTECH_DEVICE_PID.AUGUSTA_KYB:
            {
                useUniversalSDK = true;
                deviceInformation.deviceMode = IDTECH_DEVICE_PID.AUGUSTA_KYB;
                deviceInformation.emvConfigSupported = false;
                message[0] = USK_DEVICE_MODE.USB_HID;
                break;
            }

            case IDTECH_DEVICE_PID.AUGUSTA_HID:
            {
                useUniversalSDK = true;
                deviceInformation.deviceMode = IDTECH_DEVICE_PID.AUGUSTA_HID;
                deviceInformation.emvConfigSupported = true;
                message[0] = USK_DEVICE_MODE.USB_KYB;
                break;
            }

            case IDTECH_DEVICE_PID.AUGUSTAS_KYB:
            {
                useUniversalSDK = true;
                deviceInformation.deviceMode = IDTECH_DEVICE_PID.AUGUSTAS_KYB;
                deviceInformation.emvConfigSupported = false;
                message[0] = USK_DEVICE_MODE.USB_HID;
                break;
            }

            case IDTECH_DEVICE_PID.AUGUSTAS_HID:
            {
                useUniversalSDK = true;
                deviceInformation.deviceMode = IDTECH_DEVICE_PID.AUGUSTAS_HID;
                deviceInformation.emvConfigSupported = true;
                message[0] = USK_DEVICE_MODE.USB_KYB;
                break;
            }

            case IDTECH_DEVICE_PID.VP3000_HID:
            {
                useUniversalSDK = true;
                deviceInformation.deviceMode = IDTECH_DEVICE_PID.VP3000_HID;
                deviceInformation.emvConfigSupported = true;
                message[0] = USK_DEVICE_MODE.USB_KYB;
                break;
            }

            case IDTECH_DEVICE_PID.VP3000_KYB:
            {
                useUniversalSDK = false;
                deviceInformation.deviceMode = IDTECH_DEVICE_PID.VP3000_KYB;
                deviceInformation.emvConfigSupported = false;
                message[0] = USK_DEVICE_MODE.USB_HID;
                break;
            }

            case IDTECH_DEVICE_PID.VP5300_HID:
            {
                useUniversalSDK = true;
                deviceInformation.deviceMode = IDTECH_DEVICE_PID.VP5300_HID;
                deviceInformation.emvConfigSupported = true;
                message[0] = USK_DEVICE_MODE.USB_KYB;
                break;
            }

            case IDTECH_DEVICE_PID.VP5300_KYB:
            {
                useUniversalSDK = false;
                deviceInformation.deviceMode = IDTECH_DEVICE_PID.VP5300_KYB;
                deviceInformation.emvConfigSupported = false;
                message[0] = USK_DEVICE_MODE.USB_HID;
                break;
            }

            default:
            {
                message[0] = USK_DEVICE_MODE.UNKNOWN;
                break;
            }
        }

        // Notify Main Form
        NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_SET_DEVICE_MODE, Message = message });
    }

    public void SetDeviceMode(string mode)
    {
        try
        {
            Debug.WriteLine("device: SET MODE={0} ++++++++++++++++++++++++++++++++++++++++", (object) mode);

            if(mode.Equals(USK_DEVICE_MODE.USB_HID))
            {
               if(deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTA_KYB ||
                  deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTAS_KYB)
               {
                    // Set Device to HID MODE
                    if(!Device.SetUSBHIDMode())
                    {
                        DeviceRemovedHandler();
                    }
                    //if(status == EntryModeStatus.Success)
                    //{
                        //status = DeviceSoftReset();
                        //Debug.WriteLine("DeviceCfg::SetDeviceMode(): - RESET status={0}", status);
                    //}
               }
               else if(deviceInformation.deviceMode == IDTECH_DEVICE_PID.VP3000_KYB)
               {
                    Device.SetVP3000DeviceHidMode();
               }
            }
            else if(mode.Equals(USK_DEVICE_MODE.USB_KYB))
            {
               if(deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTA_HID  ||
                  deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTAS_HID)
               {
                    // TURN ON QUICK CHIP MODE
                    string command = USDK_CONFIGURATION_COMMANDS.ENABLE_QUICK_CHIP_MODE;
                    DeviceCommand(command, true);
                    // Set Device to HID MODE
                    RETURN_CODE rt = IDT_Augusta.SharedController.msr_switchUSBInterfaceMode(true);
                    Debug.WriteLine("DeviceCfg::SetDeviceMode(): - status={0}", rt);

                    // Restart device discovery
                    DeviceRemovedHandler();
               }
               else if(deviceInformation.deviceMode == IDTECH_DEVICE_PID.VP3000_HID)
               {
                  RETURN_CODE rt = IDT_VP3300.SharedController.device_setPollMode(3);
                  if(rt != RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                  {
                    Debug.WriteLine("DeviceCfg::SetDeviceMode(): VP3000 - error={0}", rt);
                  }
               }
               else if(deviceInformation.deviceMode == IDTECH_DEVICE_PID.VP5300_HID)
               {
                  RETURN_CODE rt = IDT_NEO2.SharedController.device_setPollMode(3);
                  if(rt != RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                  {
                    Debug.WriteLine("DeviceCfg::SetDeviceMode(): VP5300 - error={0}", rt);
                  }
               }
            }
        }
        catch(Exception exp)
        {
           Logger.error("DeviceCfg::SetDeviceMode(): - exception={0}", (object)exp.Message);
        }
    }    

    public void DisableQCEmvMode()
    {
        try
        {
            // Disable QC Mode
            string command = USDK_CONFIGURATION_COMMANDS.DISABLE_QUICK_CHIP_MODE;
            DeviceCommand(command, true);
            // Disable ICC
            RETURN_CODE rt = IDT_Augusta.SharedController.icc_disable();
            // Remove EMV settings
            rt = IDT_Augusta.SharedController.emv_removeTerminalData();
            // Remove All AID
            rt = IDT_Augusta.SharedController.emv_removeAllApplicationData();
            // Remove All CAPK
            rt = IDT_Augusta.SharedController.emv_removeAllCAPK();
            // Set Device to HID MODE
            rt = IDT_Augusta.SharedController.msr_switchUSBInterfaceMode(true);
            Debug.WriteLine("DeviceCfg::DisableQCEmvMode() - status={0}", rt);

            //string [] message = { "Enable" };
            //NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_SET_EMV_MODE_BUTTON, Message = message });

            // Restart device discovery
            DeviceRemovedHandler();
        }
        catch(Exception exp)
        {
           Logger.error("DeviceCfg::DisableQCEmvMode(): - exception={0}", (object)exp.Message);
        }
    }    
    
    public string DeviceCommand(string command, bool notify)
    {
        string [] message = { "" };

        if(useUniversalSDK)
        {
            byte[] response = null;
            RETURN_CODE rt = IDT_Augusta.SharedController.device_sendDataCommand(command, true, ref response);
            if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
            {
                message[0] = BitConverter.ToString(response).Replace("-", string.Empty);
            }
            else
            {
                if(response != null)
                {
                    message[0] = "COMMAND EXECUTE FAILED - CODE=" + BitConverter.ToString(response).Replace("-", string.Empty);
                }
                else
                {
                    message[0] = "COMMAND EXECUTE FAILED - CODE=0x" + string.Format("{0:X}", rt);
                }
            }

            if(notify)
            {
                NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_SET_EXECUTE_RESULT, Message = message });
            }
         }
        else
        {
            //TODO:
        }

        return message[0];
    }
    public string GetErrorMessage(string data)
    {
        throw new NotImplementedException();
    }
    public void FactoryReset()
    {
        try
        {
            Device.FactoryReset();
        }
        catch(Exception exp)
        {
           Logger.error("device: FactoryReset() - exception={0}", (object)exp.Message);
        }
        finally
        {
            NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_UI_ENABLE_BUTTONS });
        }
    }
    #endregion
  }
  #region -- main interface --
  internal class DeviceInformation
  {
    internal string SerialNumber;
    internal string FirmwareVersion;
    internal string EMVKernelVersion;
    internal string ModelName;
    internal string ModelNumber;
    internal string Port;
    internal IDTECH_DEVICE_PID deviceMode;
    internal bool emvConfigSupported;
  }
  public static class USK_DEVICE_MODE
  {
    public const string USB_HID = "USB-HID";
    public const string USB_KYB = "USB-KB";
    public const string UNKNOWN = "UNKNOWN";
    public const string OLDIDTECHHID = "OLDIDTECHHID";
    public const string OLDIDTECHKYB = "OLDIDTECHKB";
  }

  public static class USK_EMV_MODE
  {
    public const string EMV_ENABLE  = "Enable";
    public const string EMV_DISABLE = "Disable";
  }

  internal static class TerminalMajorConfiguration
  {
    internal const string CONFIG_2C = "2C";
    internal const string CONFIG_5C = "5C";
    internal const string TERMCFG_2 = "32";
    internal const string TERMCFG_5 = "35";
  }

  internal static class USDK_CONFIGURATION_COMMANDS
  {
     internal const string GET_MAJOR_TERMINAL_CFG  = "72 52 01 28";
     internal const string SET_TERMINAL_MAJOR_2C   = "72 53 01 28 01 32";
     internal const string SET_TERMINAL_MAJOR_5C   = "72 53 01 28 01 35";
     internal const string GET_EMV_TERMINAL_DATA   = "72 46 02 01";
     internal const string DISABLE_QUICK_CHIP_MODE = "72 53 01 29 01 30";
     internal const string ENABLE_QUICK_CHIP_MODE  = "72 53 01 29 01 31";
  }

  #endregion
}

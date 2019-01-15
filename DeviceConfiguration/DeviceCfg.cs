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
using IPA.Core.Shared.Helpers.StatusCode;
using IPA.Core.Shared.Enums;
using IPA.DAL;
using IPA.DAL.RBADAL.Services;
using IPA.DAL.RBADAL.Services.Devices.IDTech;
using IPA.DAL.RBADAL.Services.Devices.IDTech.Models;

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

            if(deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTA_HID  ||
               deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTAS_HID ||
               deviceInformation.deviceMode == IDTECH_DEVICE_PID.VP3000_HID   ||
               deviceInformation.deviceMode == IDTECH_DEVICE_PID.VP5300_HID)
            {
                Debug.WriteLine("DeviceCfg::DeviceInit(): - device TYPE={0}", IDT_Device.getDeviceType());

                // Initialize Universal SDK
                IDT_Device.setCallback(MessageCallBack);
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
             ///SetDeviceConfig();

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
    #endregion

    /********************************************************************************************************/
    // DEVICE CONFIGURATION
    /********************************************************************************************************/
    #region -- device configuration --
    
    private string [] DeviceGetTerminalData()
    {
        string [] data = null;

        if(deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTA_HID  ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTAS_HID ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.VP3000_HID   ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.VP5300_HID)
        {
            try
            {
                byte [] tlv = null;
                RETURN_CODE rt = IDT_Augusta.SharedController.emv_retrieveTerminalData(ref tlv);
                
                if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                {
                    List<string> collection = new List<string>();

                    Debug.WriteLine("DEVICE TERMINAL DATA ----------------------------------------------------------------------");
                    Dictionary<string, Dictionary<string, string>> dict = Common.processTLV(tlv);
                    foreach(Dictionary<string, string> devCollection in dict.Where(x => x.Key.Equals("unencrypted")).Select(x => x.Value))
                    {
                        foreach(var devTag in devCollection)
                        {
                            collection.Add(string.Format("{0}:{1}", devTag.Key, devTag.Value).ToUpper());
                        }
                    }
                    data = collection.ToArray();
                }
                else
                {
                    Debug.WriteLine("TERMINAL DATA: emv_retrieveTerminalData() - ERROR={0}", rt);
                }
            }
            catch(Exception exp)
            {
                Debug.WriteLine("DeviceCfg::GetTerminalData(): - exception={0}", (object)exp.Message);
            }
        }

        return data;
    }

    private void ValidateTerminalData()
    {
        if(deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTA_HID  ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTAS_HID ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.VP3000_HID   ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.VP5300_HID)
        {
            try
            {
                if(serializer != null)
                {
                    byte [] tlv = null;
                    RETURN_CODE rt = IDT_Augusta.SharedController.emv_retrieveTerminalData(ref tlv);
                
                    if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                    {
                        Debug.WriteLine("VALIDATE TERMINAL DATA ----------------------------------------------------------------------");

                        // Get Configuration File AID List
                        Dictionary<string, string> cfgTerminalData = serializer.GetTerminalData();
                        Dictionary<string, Dictionary<string, string>> dict = Common.processTLV(tlv);

                        bool update = false;

                        // TAGS from device
                        foreach(Dictionary<string, string> devCollection in dict.Where(x => x.Key.Equals("unencrypted")).Select(x => x.Value))
                        {
                            foreach(var devTag in devCollection)
                            {
                                string devTagName = devTag.Key;
                                string cfgTagValue = "";
                                bool tagfound = false;
                                bool tagmatch = false;
                                foreach(var cfgTag in cfgTerminalData)
                                {
                                    // Matching TAGNAME: compare keys
                                    if(devTag.Key.Equals(cfgTag.Key, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        tagfound = true;
                                        //Debug.Write("key: " + devTag.Key);

                                        // Compare value
                                        if(cfgTag.Value.Equals(devTag.Value, StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            tagmatch = true;
                                            //Debug.WriteLine(" matches value: {0}", (object) devTag.Value);
                                        }
                                        else
                                        {
                                            //Debug.WriteLine(" DOES NOT match value: {0}!={1}", devTag.Value, cfgTag.Value);
                                            cfgTagValue = cfgTag.Value;
                                            update = true;
                                        }
                                        break;
                                    }
                                    if(tagfound)
                                    {
                                        break;
                                    }
                                }
                                if(tagfound)
                                {
                                    Debug.WriteLine("TAG: {0} FOUND AND IT {1}", devTagName.PadRight(6,' '), (tagmatch ? "MATCHES" : "DOES NOT MATCH"));
                                    if(!tagmatch)
                                    {
                                        Debug.WriteLine("{0}!={1}", devTag.Value, cfgTagValue);
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("TAG: {0} NOT FOUND", (object) devTagName.PadRight(6,' '));
                                    update = true;
                                }
                            }
                        }

                        // Update Terminal Data
                        if(update)
                        {
                            rt = IDT_Augusta.SharedController.emv_setTerminalMajorConfiguration(5);
                            if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                            {
                                List<byte[]> collection = new List<byte[]>();
                                foreach(var item in cfgTerminalData)
                                {
                                    string payload = string.Format("{0}{1:X2}{2}", item.Key, item.Value.Length / 2, item.Value).ToUpper();
                                    byte [] bytes = Device_IDTech.HexStringToByteArray(payload);
                                    collection.Add(bytes);
                                }
                                var flattenedList = collection.SelectMany(bytes => bytes);
                                byte [] terminalData = flattenedList.ToArray();
                                Dictionary<string, Dictionary<string, string>> worker = Common.processTLV(terminalData);
                                rt = IDT_Augusta.SharedController.emv_setTerminalData(terminalData);
                                if(rt != RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                                {
                                    Debug.WriteLine("emv_setTerminalData() error: {0}", rt);
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.WriteLine("TERMINAL DATA: emv_retrieveTerminalData() - ERROR={0}", rt);
                    }
                }
            }
            catch(Exception exp)
            {
                Debug.WriteLine("DeviceCfg::ValidateTerminalData(): - exception={0}", (object)exp.Message);
            }
        }
    }

    private string [] DeviceGetAidList()
    {
        string [] data = null;

        if(deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTA_HID  ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTAS_HID ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.VP3000_HID   ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.VP5300_HID)
        {
            try
            {
                byte [][] keys = null;
                RETURN_CODE rt = IDT_Augusta.SharedController.emv_retrieveAIDList(ref keys);
                
                if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                {
                    List<string> collection = new List<string>();

                    Debug.WriteLine("DEVICE AID LIST ----------------------------------------------------------------------");

                    foreach(byte[] aidName in keys)
                    {
                        byte[] value = null;

                        rt = IDT_Augusta.SharedController.emv_retrieveApplicationData(aidName, ref value);

                        if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                        {
                            string devAidName = BitConverter.ToString(aidName).Replace("-", string.Empty).ToUpper();
                            Debug.WriteLine("AID: {0} ===============================================", (object) devAidName);

                            Dictionary<string, Dictionary<string, string>> dict = Common.processTLV(value);
                            List<string> valCollection = new List<string>();

                            // Compare values and replace if not the same
                            foreach(Dictionary<string, string> devCollection in dict.Where(x => x.Key.Equals("unencrypted")).Select(x => x.Value))
                            {
                                foreach(var devTag in devCollection)
                                {
                                    valCollection.Add(string.Format("{0}:{1}", devTag.Key, devTag.Value).ToUpper());
                                }
                            }
                            collection.Add(string.Format("{0}#{1}", devAidName, String.Join(" ", valCollection.ToArray())));
                        }
                    }
                    data = collection.ToArray();
                }
                else
                {
                    Debug.WriteLine("TERMINAL DATA: emv_retrieveAIDList() - ERROR={0}", rt);
                }
            }
            catch(Exception exp)
            {
                Debug.WriteLine("DeviceCfg::GetTerminalData(): - exception={0}", (object)exp.Message);
            }
        }

        return data;
    }

    private void ValidateAidList()
    {
        if(deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTA_HID  ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTAS_HID ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.VP3000_HID   ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.VP5300_HID)
        {
            try
            {
                if(serializer != null)
                {
                    byte [][] keys = null;
                    RETURN_CODE rt = IDT_Augusta.SharedController.emv_retrieveAIDList(ref keys);
                
                    if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                    {
                        Debug.WriteLine("VALIDATE AID LIST ----------------------------------------------------------------------");

                        // Get Configuration File AID List
                        AIDList aid = serializer.GetAIDList();

                        List<Aid> AidList = new List<Aid>();

                        foreach(byte[] aidName in keys)
                        {
                            bool delete = true;
                            bool found  = false;
                            bool update = false;
                            KeyValuePair<string, Dictionary<string, string>> cfgCurrentItem = new KeyValuePair<string, Dictionary<string, string>>();
                            string devAidName = BitConverter.ToString(aidName).Replace("-", string.Empty);

                            Debug.WriteLine("AID: {0} ===============================================", (object) devAidName);

                            // Is this item in the approved list?
                            foreach(var cfgItem in aid.Aid)
                            {
                                cfgCurrentItem = cfgItem;
                                if(cfgItem.Key.Equals(devAidName, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    found  = true;
                                    byte[] value = null;

                                    rt = IDT_Augusta.SharedController.emv_retrieveApplicationData(aidName, ref value);

                                    if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                                    {
                                        Dictionary<string, Dictionary<string, string>> dict = Common.processTLV(value);

                                        // Compare values and replace if not the same
                                        foreach(Dictionary<string, string> devCollection in dict.Where(x => x.Key.Equals("unencrypted")).Select(x => x.Value))
                                        {
                                            foreach(var cfgTag in cfgItem.Value)
                                            {
                                                bool tagfound = false;
                                                string cfgTagName = cfgTag.Key;
                                                string cfgTagValue = cfgTag.Value;
                                                foreach(var devTag in devCollection)
                                                {
                                                    // Matching TAGNAME: compare keys
                                                    if(devTag.Key.Equals(cfgTag.Key, StringComparison.CurrentCultureIgnoreCase))
                                                    {
                                                        tagfound = true;
                                                        //Debug.Write("key: " + devTag.Key);
                                                        update = !cfgTag.Value.Equals(devTag.Value, StringComparison.CurrentCultureIgnoreCase);

                                                        // Compare value and fix it if mismatched
                                                        if(cfgTag.Value.Equals(devTag.Value, StringComparison.CurrentCultureIgnoreCase))
                                                        {
                                                            //Debug.WriteLine("TAG: {0} FOUND AND IT MATCHES", (object) cfgTagName.PadRight(6,' '));
                                                            //Debug.WriteLine(" matches value: {0}", (object) devTag.Value);
                                                        }
                                                        else
                                                        {
                                                            Debug.WriteLine("TAG: {0} FOUND AND IT DOES NOT match value: {1}!={2}", cfgTagName.PadRight(6,' '), cfgTag.Value, devTag.Value);
                                                        }
                                                        break;
                                                    }
                                                }
                                                // No need to continue validating the remaing tags
                                                if(!tagfound || update)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    delete = false;

                                    if(update)
                                    {
                                        byte[] tagCfgName = Device_IDTech.HexStringToByteArray(cfgCurrentItem.Key);

                                        List<byte[]> collection = new List<byte[]>();
                                        foreach(var item in cfgCurrentItem.Value)
                                        {
                                            string payload = string.Format("{0}{1:X2}{2}", item.Key, item.Value.Length / 2, item.Value).ToUpper();
                                            byte [] bytes = Device_IDTech.HexStringToByteArray(payload);
                                            collection.Add(bytes);
                                        }
                                        var flattenedList = collection.SelectMany(bytes => bytes);
                                        byte [] tagCfgValue = flattenedList.ToArray();
                                        Aid cfgAid = new Aid(tagCfgName, tagCfgValue);
                                        AidList.Add(cfgAid);
                                    }
                                }
                            }

                            // DELETE THIS AID
                            if(delete)
                            {
                                Debug.WriteLine("AID: {0} - DELETE (NOT FOUND)", (object)devAidName.PadRight(14,' '));
                                byte[] tagName = Device_IDTech.HexStringToByteArray(devAidName);
                                rt = IDT_Augusta.SharedController.emv_removeApplicationData(tagName);
                                if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                                {
                                    Debug.WriteLine("AID: {0} DELETED", (object) devAidName.PadRight(6,' '));
                                }
                            }
                            else if(!found)
                            {
                                byte[] tagCfgName = Device_IDTech.HexStringToByteArray(cfgCurrentItem.Key);

                                List<byte[]> collection = new List<byte[]>();
                                foreach(var item in cfgCurrentItem.Value)
                                {
                                    string payload = string.Format("{0}{1:X2}{2}", item.Key, item.Value.Length / 2, item.Value).ToUpper();
                                    byte [] bytes = Device_IDTech.HexStringToByteArray(payload);
                                    collection.Add(bytes);
                                }
                                var flattenedList = collection.SelectMany(bytes => bytes);
                                byte [] tagCfgValue = flattenedList.ToArray();
                                Aid cfgAid = new Aid(tagCfgName, tagCfgValue);
                                AidList.Add(cfgAid);
                            }
                        }

                        // Add missing AID(s)
                        foreach(var aidElement in AidList)
                        {
                            byte [] aidName = aidElement.GetAidName();
                            byte [] aidValue = aidElement.GetAidValue();
                            rt = IDT_Augusta.SharedController.emv_setApplicationData(aidName, aidValue);
                            if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                            {
                                string cfgTagName = BitConverter.ToString(aidName).Replace("-", string.Empty);
                                string cfgTagValue = BitConverter.ToString(aidValue).Replace("-", string.Empty);
                                Debug.WriteLine("AID: {0} UPDATED WITH VALUE: {1}", cfgTagName.PadRight(6,' '), cfgTagValue);
                            }
                        }
                    }
                    else
                    {
                        Debug.WriteLine("TERMINAL DATA: emv_retrieveAIDList() - ERROR={0}", rt);
                    }
                }
            }
            catch(Exception exp)
            {
                Debug.WriteLine("DeviceCfg::ValidateAidList(): - exception={0}", (object)exp.Message);
            }
        }
    }
    
    private string [] DeviceGetCapKList()
    {
        string [] data = null;

        if(deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTA_HID  ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTAS_HID ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.VP3000_HID   ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.VP5300_HID)
        {
            try
            {
                byte [] keys = null;
                RETURN_CODE rt = IDT_Augusta.SharedController.emv_retrieveCAPKList(ref keys);
                
                if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                {
                    List<string> collection = new List<string>();

                    Debug.WriteLine("DEVICE CAPK LIST ----------------------------------------------------------------------");

                    List<byte[]> capkNames = new List<byte[]>();

                    // Convert array to array of arrays
                    for(int i = 0; i < keys.Length; i += 6)
                    {
                        byte[] result = new byte[6];
                        Array.Copy(keys, i, result, 0, 6);
                        capkNames.Add(result); 
                    }

                    foreach(byte[] capkName in capkNames)
                    {
                        string devCapKName = BitConverter.ToString(capkName).Replace("-", string.Empty);
                        Debug.WriteLine("CAPK: {0} ===============================================", (object) devCapKName);

                        byte[] key = null;
                        rt = IDT_Augusta.SharedController.emv_retrieveCAPK(capkName, ref key);
                        if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                        {
                            Capk capk = new Capk(key);
                            string RID = devCapKName.Substring(0, 10);
                            string Idx = devCapKName.Substring(10, 2);
                            string payload = string.Format("{0}:{1} ", "RID", RID).ToUpper();
                            payload += string.Format("{0}:{1} ", "INDEX", Idx).ToUpper();
                            payload += string.Format("{0}:{1} ", "MODULUS", capk.GetModulus()).ToUpper();
                            collection.Add(string.Format("{0}#{1}", (RID + "-" + Idx), payload).ToUpper());
                            Debug.WriteLine("MODULUS: {0}", (object) capk.GetModulus().ToUpper());
                        }
                    }

                    data = collection.ToArray();
                }
                else
                {
                    Debug.WriteLine("AID: emv_retrieveCAPKList() - ERROR={0}", rt);
                }
            }
            catch(Exception exp)
            {
                Debug.WriteLine("DeviceCfg::GetTerminalData(): - exception={0}", (object)exp.Message);
            }
        }

        return data;
    }

    private void ValidateCapKList()
    {
        if(deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTA_HID  ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTAS_HID ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.VP3000_HID   ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.VP5300_HID)
        {
            try
            {
                if(serializer != null)
                {
                    byte [] keys = null;
                    RETURN_CODE rt = IDT_Augusta.SharedController.emv_retrieveCAPKList(ref keys);
                
                    if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                    {
                        Debug.WriteLine("VALIDATE CAPK LIST ----------------------------------------------------------------------");

                        // Get Configuration File AID List
                        CapKList capK = serializer.GetCapKList();

                        List<Capk> CapKList = new List<Capk>();
                        List<byte[]> capkNames = new List<byte[]>();

                        // Convert array to array of arrays
                        for(int i = 0; i < keys.Length; i += 6)
                        {
                            byte[] result = new byte[6];
                            Array.Copy(keys, i, result, 0, 6);
                            capkNames.Add(result); 
                        }

                        foreach(byte[] capkName in capkNames)
                        {
                            bool delete = true;
                            bool found  = false;
                            bool update = false;
                            KeyValuePair<string, Dictionary<string, string>> cfgCurrentItem = new KeyValuePair<string, Dictionary<string, string>>();
                            string devCapKName = BitConverter.ToString(capkName).Replace("-", string.Empty);

                            Debug.WriteLine("CAPK: {0} ===============================================", (object) devCapKName);

                            // Is this item in the approved list?
                            foreach(var cfgItem in capK.Capk)
                            {
                                cfgCurrentItem = cfgItem;
                                string devRID = cfgItem.Value.Where(x => x.Key.Equals("RID")).Select(x => x.Value).First();
                                string devIdx = cfgItem.Value.Where(x => x.Key.Equals("Index")).Select(x => x.Value).First();
                                string devItem = devRID + devIdx;
                                if(devItem.Equals(devCapKName, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    found  = true;
                                    byte[] value = null;
                                    Capk capk = null;

                                    rt = IDT_Augusta.SharedController.emv_retrieveCAPK(capkName, ref value);

                                    if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                                    {
                                        capk = new Capk(value);

                                        // compare modulus
                                        string modulus = cfgItem.Value.Where(x => x.Key.Equals("modulus", StringComparison.CurrentCultureIgnoreCase)).Select(x => x.Value).First();
                                        update = !modulus.Equals(capk.GetModulus(), StringComparison.CurrentCultureIgnoreCase);
                                        if(!update)
                                        {
                                            // compare exponent
                                            string exponent = cfgItem.Value.Where(x => x.Key.Equals("exponent", StringComparison.CurrentCultureIgnoreCase)).Select(x => x.Value).First();
                                            update = !exponent.Equals(capk.GetExponent(), StringComparison.CurrentCultureIgnoreCase);
                                        }
                                    }

                                    delete = false;

                                    if(update && capk != null)
                                    {
                                        CapKList.Add(capk);
                                    }
                                    else
                                    {
                                        Debug.WriteLine("    : UP-TO-DATE");
                                    }
                                }
                            }

                            // DELETE CAPK(s)
                            if(delete)
                            {
                                byte[] tagName = Device_IDTech.HexStringToByteArray(devCapKName);
                                rt = IDT_Augusta.SharedController.emv_removeCAPK(tagName);
                                if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                                {
                                    Debug.WriteLine("CAPK: {0} DELETED (NOT FOUND)", (object) devCapKName);
                                }
                                else
                                {
                                    Debug.WriteLine("CAPK: {0} DELETE FAILED, ERROR={1}", devCapKName, rt);
                                }
                            }
                            else if(!found)
                            {
                                byte[] tagCfgName = Device_IDTech.HexStringToByteArray(cfgCurrentItem.Key);

                                List<byte[]> collection = new List<byte[]>();
                                foreach(var item in cfgCurrentItem.Value)
                                {
                                    string payload = string.Format("{0}{1:X2}{2}", item.Key, item.Value.Length / 2, item.Value).ToUpper();
                                    byte [] bytes = Device_IDTech.HexStringToByteArray(payload);
                                    collection.Add(bytes);
                                }
                                var flattenedList = collection.SelectMany(bytes => bytes);
                                byte [] tagCfgValue = flattenedList.ToArray();
                                Capk cfgCapK = new Capk(tagCfgName);
                                CapKList.Add(cfgCapK);
                            }
                        }

                        // Add/Update CAPK(s)
                        foreach(var capkElement in CapKList)
                        {
                            //capkElement.ShowCapkValues();
                            byte [] capkValue = capkElement.GetCapkValue();
                            rt = IDT_Augusta.SharedController.emv_setCAPK(capkValue);
                            if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                            {
                                Debug.WriteLine("CAPK: {0} UPDATED", (object) capkElement.GetCapkName());
                            }
                            else
                            {
                                Debug.WriteLine("CAPK: {0} FAILED TO UPDATE - ERROR={1}", capkElement.GetCapkName(), rt);
                            }
                        }
                    }
                    else
                    {
                        Debug.WriteLine("CAPK: emv_retrieveCAPKList() - ERROR={0}", rt);
                    }
                }
            }
            catch(Exception exp)
            {
                Debug.WriteLine("DeviceCfg::ValidateAidList(): - exception={0}", (object)exp.Message);
            }
        }
    }

    public void FactoryReset()
    {
        if(deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTA_HID  ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.AUGUSTAS_HID ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.VP3000_HID   ||
           deviceInformation.deviceMode == IDTECH_DEVICE_PID.VP5300_HID)
        {
            try
            {
                // TERMINAL DATA
                TerminalDataFactory tf = new TerminalDataFactory();
                byte[] term = tf.GetFactoryTerminalData5C();
                RETURN_CODE rt = IDT_Augusta.SharedController.emv_setTerminalData(term);
                if (rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                {
                    Debug.WriteLine("TERMINAL DATA [DEFAULT] ----------------------------------------------------------------------");
                }
                else
                {
                    Debug.WriteLine("TERMINAL DATA [DEFAULT] failed with error code: 0x{0:X}", (ushort) rt);
                }

                // AID
                AidFactory factoryAids = new AidFactory();
                Dictionary<byte [], byte []> aid = factoryAids.GetFactoryAids();
                Debug.WriteLine("AID LIST [DEFAULT] ----------------------------------------------------------------------");
                foreach(var item in aid)
                {
                    byte [] name  = item.Key;
                    byte [] value = item.Value;
                    rt = IDT_Augusta.SharedController.emv_setApplicationData(name, value);
                
                    if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                    {
                        Debug.WriteLine("AID: {0}", (object) BitConverter.ToString(name).Replace("-", string.Empty));
                    }
                    else
                    {
                        Debug.WriteLine("CAPK: {0} failed Error Code: 0x{1:X}", (ushort) rt);
                    }
                }

                // CAPK
                CapKFactory factoryCapk = new CapKFactory();
                Dictionary<byte [], byte []> capk = factoryCapk.GetFactoryCapK();
                Debug.WriteLine("CAPK LIST [DEFAULT] ----------------------------------------------------------------------");
                foreach(var item in capk)
                {
                    byte [] name  = item.Key;
                    byte [] value = item.Value;
                    rt = IDT_Augusta.SharedController.emv_setCAPK(value);

                    if (rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                    {
                        Debug.WriteLine("CAPK: {0}", (object) BitConverter.ToString(name).Replace("-", string.Empty).ToUpper());
                    }
                    else
                    {
                        Debug.WriteLine("CAPK: {0} failed Error Code: 0x{1:X}", (ushort) rt);
                    }
                }
            }
            catch(Exception exp)
            {
                Debug.WriteLine("DeviceCfg::FactoryReset(): - exception={0}", (object)exp.Message);
            }
            finally
            {
                NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_UI_ENABLE_BUTTONS });
            }
        }
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
            message = DeviceGetTerminalData();
        }
        else
        {
            if(serializer == null)
            {
                serializer = new ConfigSerializer();
                serializer.ReadConfig();        
            }

            ValidateTerminalData();

            message = serializer.GetTerminalDataString();
        }

        NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_SHOW_TERMINAL_DATA, Message = message });
    }

    public void GetAIDList()
    {
        string [] message = null;
        if(configurationMode == ConfigurationModes.FROM_DEVICE)
        {
            message = DeviceGetAidList();
        }
        else
        {
            ValidateAidList();
            message = serializer.GetAIDCollection();
        }

        NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_SHOW_AID_LIST, Message = message });
    }

    public void GetCapKList()
    {
        string [] message = null;
        if(configurationMode == ConfigurationModes.FROM_DEVICE)
        {
            message = DeviceGetCapKList();
        }
        else
        {
            ValidateCapKList();
            message = serializer.GetCapKCollection();
        }

        NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_SHOW_CAPK_LIST, Message = message });
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
                message[0] = USK_DEVICE_MODE.USB_HID;
                break;
            }

            case IDTECH_DEVICE_PID.AUGUSTA_HID:
            {
                useUniversalSDK = true;
                deviceInformation.deviceMode = IDTECH_DEVICE_PID.AUGUSTA_HID;
                message[0] = USK_DEVICE_MODE.USB_KYB;
                break;
            }

            case IDTECH_DEVICE_PID.AUGUSTAS_KYB:
            {
                useUniversalSDK = true;
                deviceInformation.deviceMode = IDTECH_DEVICE_PID.AUGUSTAS_KYB;
                message[0] = USK_DEVICE_MODE.USB_HID;
                break;
            }

            case IDTECH_DEVICE_PID.AUGUSTAS_HID:
            {
                useUniversalSDK = true;
                deviceInformation.deviceMode = IDTECH_DEVICE_PID.AUGUSTAS_HID;
                message[0] = USK_DEVICE_MODE.USB_KYB;
                break;
            }

            case IDTECH_DEVICE_PID.VP3000_HID:
            {
                useUniversalSDK = true;
                deviceInformation.deviceMode = IDTECH_DEVICE_PID.VP3000_HID;
                message[0] = USK_DEVICE_MODE.USB_KYB;
                break;
            }

            case IDTECH_DEVICE_PID.VP3000_KYB:
            {
                useUniversalSDK = false;
                deviceInformation.deviceMode = IDTECH_DEVICE_PID.VP3000_KYB;
                message[0] = USK_DEVICE_MODE.USB_HID;
                break;
            }

            case IDTECH_DEVICE_PID.VP5300_HID:
            {
                useUniversalSDK = true;
                deviceInformation.deviceMode = IDTECH_DEVICE_PID.VP5300_HID;
                message[0] = USK_DEVICE_MODE.USB_KYB;
                break;
            }

            case IDTECH_DEVICE_PID.VP5300_KYB:
            {
                useUniversalSDK = false;
                deviceInformation.deviceMode = IDTECH_DEVICE_PID.VP5300_KYB;
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
            }
        }
        catch(Exception exp)
        {
           Debug.WriteLine("DeviceCfg::SetDeviceMode(): - exception={0}", (object)exp.Message);
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
           Debug.WriteLine("DeviceCfg::DisableQCEmvMode(): - exception={0}", (object)exp.Message);
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

        }

        return message[0];
    }
    public string GetErrorMessage(string data)
    {
        throw new NotImplementedException();
    }
    #endregion
  }
  #region -- main interface --
  internal class DeviceInformation
  {
    internal string SerialNumber;
    internal string FirmwareVersion;
    internal string ModelName;
    internal string ModelNumber;
    internal string Port;
    internal IDTECH_DEVICE_PID deviceMode;
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

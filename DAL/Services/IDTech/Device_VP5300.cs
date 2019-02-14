using IDTechSDK;
using IPA.CommonInterface;
using IPA.CommonInterface.Factory;
using IPA.Core.Shared.Enums;
using IPA.DAL.RBADAL.Interfaces;
using IPA.LoggerManager;
using IPA.DAL.RBADAL.Services.Devices.IDTech;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IPA.DAL.RBADAL.Services
{
    class Device_VP5300 : Device_IDTech
    {
        internal static string _HASH_SHA1_ID_STR = "01";
        internal static string _ENC_RSA_ID_STR   = "01";

        private IDTechSDK.IDT_DEVICE_Types deviceType;
        private DEVICE_INTERFACE_Types     deviceConnect;
        private DEVICE_PROTOCOL_Types      deviceProtocol;
        private IDTECH_DEVICE_PID          deviceMode;

        private string serialNumber = "";
        private string EMVKernelVer = "";
        private static DeviceInfo deviceInfo = null;

        public Device_VP5300(IDTECH_DEVICE_PID mode) : base(mode)
        {
            deviceType = IDT_DEVICE_Types.IDT_DEVICE_NEO2;
            deviceMode = mode;
            Debug.WriteLine("device: VP5300 instantiated with PID={0}", deviceMode);
        }

        public override void Configure(object[] settings)
        {
            deviceType    = (IDT_DEVICE_Types) settings[0];
            deviceConnect = (DEVICE_INTERFACE_Types) settings[1];

            // Create Device info object
            deviceInfo = new DeviceInfo();

            PopulateDeviceInfo();
        }

        private bool PopulateDeviceInfo()
        {
            serialNumber = "";
            RETURN_CODE rt = IDT_NEO2.SharedController.config_getSerialNumber(ref serialNumber);
            //TODO: ???
            if (rt == RETURN_CODE.RETURN_CODE_BUSY)
            {
                IDT_NEO2.SharedController.emv_cancelTransaction();
                rt = IDT_NEO2.SharedController.config_getSerialNumber(ref serialNumber);
            }
            if (rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
            {
                deviceInfo.SerialNumber = serialNumber;
                Debug.WriteLine("device INFO[Serial Number]     : {0}", (object) deviceInfo.SerialNumber);
            }
            else
            {
                Debug.WriteLine("device: PopulateDeviceInfo() - failed to get serialNumber reason={0}", rt);
            }

            string firmwareVersion = "";
            rt = IDT_NEO2.SharedController.device_getFirmwareVersion(ref firmwareVersion);
            if (rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
            {
                //deviceInfo.FirmwareVersion = ParseFirmwareVersion(firmwareVersion);
                deviceInfo.FirmwareVersion = firmwareVersion;
                Debug.WriteLine("device INFO[Firmware Version]  : {0}", (object) deviceInfo.FirmwareVersion);

                //deviceInfo.Port = firmwareVersion.Substring(firmwareVersion.IndexOf("USB", StringComparison.Ordinal), 7);
                deviceInfo.Port = "HID/USB";
                Debug.WriteLine("device INFO[Port]              : {0}", (object) deviceInfo.Port);

                deviceInfo.ModelNumber = firmwareVersion.Split(' ')[0] ?? "";
                Debug.WriteLine("device INFO[Model Number]      : {0}", (object) deviceInfo.ModelNumber);
            }
            else
            {
                Debug.WriteLine("device: PopulateDeviceInfo() - failed to get Firmware version reason={0}", rt);
            }

            deviceInfo.ModelName = IDTechSDK.Profile.IDT_DEVICE_String(deviceType, deviceConnect);
            Debug.WriteLine("device INFO[Model Name]        : {0}", (object) deviceInfo.ModelName);

            //rt = IDT_NEO2.SharedController.config_getModelNumber(ref deviceInfo.ModelNumber);
            //if (rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
            //{
            //    deviceInfo.ModelNumber = deviceInfo?.ModelNumber?.Split(' ')[0] ?? "";
            //    Debug.WriteLine("device INFO[Model Number]      : {0}", (object) deviceInfo.ModelNumber);
            //}
            //else
            //{
            //    Debug.WriteLine("device: PopulateDeviceInfo() - failed to get Model number reason={0}", rt);
            //}

            EMVKernelVer = "";
            rt = IDT_NEO2.SharedController.emv_getEMVKernelVersion(ref EMVKernelVer);
            if (rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
            {
                deviceInfo.EMVKernelVersion = EMVKernelVer;
                Debug.WriteLine("device INFO[EMV KERNEL V.]     : {0}", (object) deviceInfo.EMVKernelVersion);
            }
            else
            {
                Debug.WriteLine("device: PopulateDeviceInfo() - failed to get Model number reason={0}", rt);
            }

            return true;
        }

        private IDTSetStatus DeviceReset()
        {
            var configStatus = new IDTSetStatus { Success = true };
            // WIP: no resets for these device types
            return configStatus;
        }

        public override string GetFirmwareVersion()
        {
            string firmwareVersion = "";
            RETURN_CODE rt = IDT_NEO2.SharedController.device_getFirmwareVersion(ref firmwareVersion);
            if (rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
            {
                //deviceInfo.FirmwareVersion = ParseFirmwareVersion(firmwareVersion);
                //firmwareVersion = deviceInfo.FirmwareVersion;
                deviceInfo.FirmwareVersion = firmwareVersion;
                Debug.WriteLine("device INFO[Firmware Version]  : {0}", (object) deviceInfo.FirmwareVersion);
            }
            else
            {
                Debug.WriteLine("device: GetDeviceFirmwareVersion() - failed to get Firmware version reason={0}", rt);
            }
            return firmwareVersion;
        }
        public override string ParseFirmwareVersion(string firmwareInfo)
        {
            // VP5300 format has no space after V: V1.00
            // Validate the format firmwareInfo see if the version # exists
            var version = firmwareInfo?.Substring(firmwareInfo.IndexOf('V') + 1,
                                                  firmwareInfo.Length - firmwareInfo.IndexOf('V') - 1).Trim() ?? "";
            if(version.Length > 0)
            {
                var mReg = Regex.Match(version, @"[0-9]+\.[0-9]+");

                // If the parse succeeded 
                if (mReg.Success)
                {
                    version = mReg.Value;
                }
            }

            return version;
        }

        public override string GetSerialNumber()
        {
           string serialNumber = "";
           RETURN_CODE rt = IDT_NEO2.SharedController.config_getSerialNumber(ref serialNumber);

          if (RETURN_CODE.RETURN_CODE_DO_SUCCESS == rt)
          {
              deviceInfo.SerialNumber = serialNumber;
              Debug.WriteLine("device::GetSerialNumber(): {0}", (object) deviceInfo.SerialNumber);
          }
          else
          {
            Debug.WriteLine("device::GetSerialNumber(): failed to get serialNumber e={0}", rt);
          }

          return serialNumber;
        }

        public override DeviceInfo GetDeviceInfo()
        {
            if(deviceMode == IDTECH_DEVICE_PID.VP5300_HID)
            {
                return deviceInfo;
            }

            return base.GetDeviceInfo();
        }

        /********************************************************************************************************/
        // DEVICE CONFIGURATION
        /********************************************************************************************************/
        #region -- device configuration --

        public void GetTerminalInfo(ConfigSerializer serializer)
        {
            try
            {
                string response = null;
                RETURN_CODE rt = IDT_NEO2.SharedController.device_getFirmwareVersion(ref response);

                if (RETURN_CODE.RETURN_CODE_DO_SUCCESS == rt && !string.IsNullOrWhiteSpace(response))
                {
                    //serializer.terminalCfg.general_configuration.Terminal_info.firmware_ver = response;
                }
                response = "";
                rt = IDT_NEO2.SharedController.emv_getEMVKernelVersion(ref response);
                if(RETURN_CODE.RETURN_CODE_DO_SUCCESS == rt && !string.IsNullOrWhiteSpace(response))
                {
                    //serializer.terminalCfg.general_configuration.Terminal_info.contact_emv_kernel_ver = response;
                }
                response = "";
                rt = IDT_NEO2.SharedController.emv_getEMVKernelCheckValue(ref response);
                if(RETURN_CODE.RETURN_CODE_DO_SUCCESS == rt && !string.IsNullOrWhiteSpace(response))
                {
                    //serializer.terminalCfg.general_configuration.Terminal_info.contact_emv_kernel_checksum = response;
                }
                response = "";
                rt = IDT_NEO2.SharedController.emv_getEMVConfigurationCheckValue(ref response);
                if(RETURN_CODE.RETURN_CODE_DO_SUCCESS == rt && !string.IsNullOrWhiteSpace(response))
                {
                    //serializer.terminalCfg.general_configuration.Terminal_info.contact_emv_kernel_configuration_checksum = response;
                }
            }
            catch(Exception exp)
            {
                Debug.WriteLine("device: GetTerminalInfo() - exception={0}", (object)exp.Message);
            }
         }

        public override string [] GetTerminalData()
         {
            string [] data = null;

            try
            {
                byte [] tlv = null;
                RETURN_CODE rt = IDT_NEO2.SharedController.emv_retrieveTerminalData(ref tlv);
                
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
                Debug.WriteLine("device: GetTerminalData() - exception={0}", (object)exp.Message);
            }

            return data;
        }

        public override void ValidateTerminalData(ConfigSerializer serializer)
         {
            try
            {
                if(serializer != null)
                {
                    byte [] tlv = null;
                    RETURN_CODE rt = IDT_NEO2.SharedController.emv_retrieveTerminalData(ref tlv);
                
                    if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                    {
                        Debug.WriteLine("VALIDATE TERMINAL DATA ----------------------------------------------------------------------");

                        // Get Configuration File AID List
                        SortedDictionary<string, string> cfgTerminalData = serializer.GetTerminalData(serialNumber, EMVKernelVer);
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
                            TerminalSettings termsettings = serializer.GetTerminalSettings();
                            string workerstr = termsettings.MajorConfiguration;
                            string majorcfgstr = Regex.Replace(workerstr, "[^0-9.]", string.Empty);
                            int majorcfgint = 5;
                            if(Int32.TryParse(majorcfgstr, out majorcfgint))
                            {
                                rt = IDT_NEO2.SharedController.emv_setTerminalMajorConfiguration(majorcfgint);
                                if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                                {
                                    try
                                    {
                                        List<byte[]> collection = new List<byte[]>();
                                        foreach(var item in cfgTerminalData)
                                        {
                                            byte [] bytes = null;
                                            string payload = string.Format("{0}{1:X2}{2}", item.Key, item.Value.Length / 2, item.Value).ToUpper();
                                            if (System.Text.RegularExpressions.Regex.IsMatch(item.Value, @"[g-zG-Z\x20\x2E]+"))
                                            {
                                                List<byte> byteArray = new List<byte>();
                                                byteArray.AddRange(Device_IDTech.HexStringToByteArray(item.Key));
                                                byte [] item1 = Encoding.ASCII.GetBytes(item.Value);
                                                byte itemLen = Convert.ToByte(item1.Length);
                                                byte [] item2 = new byte[]{ itemLen };
                                                byteArray.AddRange(item2);
                                                byteArray.AddRange(item1);
                                                bytes = new byte[byteArray.Count];
                                                byteArray.CopyTo(bytes);
                                                //Logger.debug( "device: ValidateTerminalData() DATA={0}", BitConverter.ToString(bytes).Replace("-", string.Empty));
                                            }
                                            else
                                            {
                                                bytes = Device_IDTech.HexStringToByteArray(payload);
                                            }
                                            collection.Add(bytes);
                                        }
                                        var flattenedList = collection.SelectMany(bytes => bytes);
                                        byte [] terminalData = flattenedList.ToArray();
                                        rt = IDT_NEO2.SharedController.emv_setTerminalData(terminalData);
                                        if(rt != RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                                        {
                                            Debug.WriteLine("emv_setTerminalData() error: {0}", rt);
                                            Logger.error( "device: ValidateTerminalData() error={0} DATA={1}", rt, BitConverter.ToString(terminalData).Replace("-", string.Empty));
                                        }
                                    }
                                    catch(Exception exp)
                                    {
                                        Debug.WriteLine("device: ValidateTerminalData() - exception={0}", (object)exp.Message);
                                    }
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
                Debug.WriteLine("device: ValidateTerminalData() - exception={0}", (object)exp.Message);
            }
        }
        public override string [] GetAidList()
         {
            string [] data = null;

                try
                {
                    byte [][] keys = null;
                    RETURN_CODE rt = IDT_NEO2.SharedController.emv_retrieveAIDList(ref keys);
                
                    if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                    {
                        List<string> collection = new List<string>();

                        Debug.WriteLine("DEVICE AID LIST ----------------------------------------------------------------------");

                        foreach(byte[] aidName in keys)
                        {
                            byte[] value = null;

                            rt = IDT_NEO2.SharedController.emv_retrieveApplicationData(aidName, ref value);

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
                    Debug.WriteLine("device: GetAidList() - exception={0}", (object)exp.Message);
                }

            return data;
         }

        public override void ValidateAidList(ConfigSerializer serializer)
         {
            try
            {
                if(serializer != null)
                {
                    byte [][] keys = null;
                    RETURN_CODE rt = IDT_NEO2.SharedController.emv_retrieveAIDList(ref keys);
                
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

                                    rt = IDT_NEO2.SharedController.emv_retrieveApplicationData(aidName, ref value);

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
                                rt = IDT_NEO2.SharedController.emv_removeApplicationData(tagName);
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
                            rt = IDT_NEO2.SharedController.emv_setApplicationData(aidName, aidValue);
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
                Debug.WriteLine("device: ValidateAidList() - exception={0}", (object)exp.Message);
            }
         }
    
        public override string [] GetCapKList()
         {
            string [] data = null;

            try
            {
                byte [] keys = null;
                RETURN_CODE rt = IDT_NEO2.SharedController.emv_retrieveCAPKList(ref keys);

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
                        rt = IDT_NEO2.SharedController.emv_retrieveCAPK(capkName, ref key);
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
                    Debug.WriteLine("device: emv_retrieveCAPKList() - ERROR={0}", rt);
                }
            }
            catch(Exception exp)
            {
                Debug.WriteLine("device: GetCapKList() - exception={0}", (object)exp.Message);
            }

            return data;
         }

        public override void ValidateCapKList(ConfigSerializer serializer)
         {
            try
            {
                if(serializer != null)
                {
                    byte [] keys = null;
                    RETURN_CODE rt = IDT_NEO2.SharedController.emv_retrieveCAPKList(ref keys);
                
                    if (rt == RETURN_CODE.RETURN_CODE_NO_CA_KEY)
                    {
                        keys = new byte[0];
                        rt = RETURN_CODE.RETURN_CODE_DO_SUCCESS;
                    }
                    if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                    {
                        Debug.WriteLine("VALIDATE CAPK LIST ----------------------------------------------------------------------");

                        // Get Configuration File AID List
                        CAPKList capK = serializer.GetCapKList();

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
                            KeyValuePair<string, CAPK> cfgCurrentItem = new KeyValuePair<string, CAPK>();
                            string devCapKName = BitConverter.ToString(capkName).Replace("-", string.Empty);

                            Debug.WriteLine("CAPK: {0} ===============================================", (object) devCapKName);

                            // Is this item in the approved list?
                            foreach(var cfgItem in capK.CAPK)
                            {
                                cfgCurrentItem = cfgItem;
                                string devRID = cfgItem.Value.RID;
                                string devIdx = cfgItem.Value.Index;
                                string devItem = devRID + devIdx;
                                if(devItem.Equals(devCapKName, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    found  = true;
                                    byte[] value = null;
                                    Capk capk = null;

                                    rt = IDT_NEO2.SharedController.emv_retrieveCAPK(capkName, ref value);

                                    if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                                    {
                                        capk = new Capk(value);

                                        // compare modulus
                                        string modulus = cfgItem.Value.Modulus;
                                        update = !modulus.Equals(capk.GetModulus(), StringComparison.CurrentCultureIgnoreCase);
                                        if(!update)
                                        {
                                            // compare exponent
                                            string exponent = cfgItem.Value.Exponent;
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
                                rt = IDT_NEO2.SharedController.emv_removeCAPK(tagName);
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
                                string payload = string.Format("{0}{1}{2}{3}{4}{5}{6:X2}{7:X2}{8}",
                                                                cfgCurrentItem.Value.RID, cfgCurrentItem.Value.Index,
                                                                _HASH_SHA1_ID_STR, _ENC_RSA_ID_STR,
                                                                cfgCurrentItem.Value.Checksum, cfgCurrentItem.Value.Exponent,
                                                                (cfgCurrentItem.Value.Modulus.Length / 2) % 256, (cfgCurrentItem.Value.Modulus.Length / 2) / 256,
                                                                cfgCurrentItem.Value.Modulus);
                                byte[] tagCfgValue = Device_IDTech.HexStringToByteArray(payload);
                                Capk cfgCapK = new Capk(tagCfgValue);
                                CapKList.Add(cfgCapK);
                            }
                        }

                        // Add/Update CAPK(s)
                        foreach(var capkElement in CapKList)
                        {
                            //capkElement.ShowCapkValues();
                            byte [] capkValue = capkElement.GetCapkValue();
                            rt = IDT_NEO2.SharedController.emv_setCAPK(capkValue);
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
                Debug.WriteLine("device: ValidateAidList() - exception={0}", (object)exp.Message);
            }
        }

        public override string [] GetConfigGroup(int group)
         {
            string [] data = null;

            try
            {
                byte [] tlv = null;
                RETURN_CODE rt = IDT_NEO2.SharedController.ctls_getConfigurationGroup(group, ref tlv);

                if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                {
                    Debug.WriteLine("DEVICE GROUP{0} LIST ----------------------------------------------------------------------", group);

                    Dictionary<string, Dictionary<string, string>> dict = Common.processTLV(tlv);
                    List<string> collection = new List<string>();

                    // Compare values and replace if not the same
                    foreach(Dictionary<string, string> devCollection in dict.Where(x => x.Key.Equals("unencrypted")).Select(x => x.Value))
                    {
                        foreach(var devTag in devCollection)
                        {
                            collection.Add(string.Format("{0}:{1}:{2}", "N/A", devTag.Key, devTag.Value).ToUpper());
                        }
                    }
                    data = collection.ToArray();
                }
                else
                {
                    Debug.WriteLine("device: ctls_getConfigurationGroup() - ERROR={0}", rt);
                }
            }
            catch(Exception exp)
            {
                Debug.WriteLine("device: GetConfigGroup() - exception={0}", (object)exp.Message);
            }

            return data;
         }

        public override void ValidateConfigGroup(ConfigSerializer serializer, int group)
         {
            try
            {
                if(serializer != null)
                {
                    byte [] tlv = null;
                    RETURN_CODE rt = IDT_NEO2.SharedController.ctls_getConfigurationGroup(group, ref tlv);
                
                    if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                    {
                        Debug.WriteLine("VALIDATE GROUP{0} LIST ----------------------------------------------------------------------", group);

                        // GROUP TLV
                        Dictionary<string, string> dict = Common.processTLVUnencrypted(tlv);

                        // Get Configuration Group
                        EMVGroupTags tags = serializer.GetConfigGroup(group);
                        AIDList aid = serializer.GetAIDList();
                        bool update = false;
                        bool found  = false;

                        foreach(var item in tags.Tags.Where(x => x.Key.Equals(Convert.ToString(group))).Select(x => x.Value))
                        {
                            foreach(var tag in item)
                            {
                                // Value in DEVICE CONFIG GROUP
                                foreach(var wtlv in dict.Where(x => x.Key.Equals(tag)).Select(x => x.Value))
                                {
                                    Debug.WriteLine("DEVICE TAG={0}, VALUE={1}", tag, wtlv);
                                    found = true;
                                    update = true;
                                    string value = "";
                                    // Value is in AID
                                    foreach(var wAid in aid.Aid)
                                    {
                                        foreach(var val in wAid.Value.Where(x => x.Key.Equals(tag)).Select(x => x.Value))
                                        {
                                            value = val;
                                            if(val.Equals(wtlv))
                                            {
                                                update = false;
                                                Logger.debug("CONFIG GROUP{0} CURRENT WITH TAG={1}, VALUE={2}", group, tag, wtlv);
                                            }
                                            break;
                                        }
                                    }
                                    if(update || !found)
                                    {
                                        string updater = string.Format("DFEE2D{0,2:00}{1}{2,2:00}{3}", group, tag, value.Length / 2, value);
                                        byte[] cfgGroupTLV = Device_IDTech.HexStringToByteArray(updater);
                                        Debug.WriteLine("UPDATE NEEDED: {0}", (object) updater);
                                        Logger.debug( "device: ValidateConfigGroup() GROUP={0}, DATA=[{1}]", group, BitConverter.ToString(cfgGroupTLV).Replace("-", string.Empty));
                                        rt = IDT_NEO2.SharedController.ctls_setConfigurationGroup(cfgGroupTLV);
                                        if(rt == RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                                        {
                                            Logger.debug("CONFIG GROUP{0} UPDATED : [{1}]", group, updater);
                                        }
                                        else
                                        {
                                            Logger.error("GROUP{0} FAILED UPDATE WITH ERROR={1}", group, errorCode.getErrorString(rt));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.WriteLine("GROUP: ctls_getConfigurationGroup() - ERROR={0}", rt);
                    }
                }
            }
            catch(Exception exp)
            {
                Debug.WriteLine("device: ValidateAidList() - exception={0}", (object)exp.Message);
            }
        }

        public override void CloseDevice()
         {
            if (Profile.deviceIsInitialized(IDT_DEVICE_Types.IDT_DEVICE_NEO2, DEVICE_INTERFACE_Types.DEVICE_INTERFACE_USB))
            {
                Profile.closeDevice(IDT_DEVICE_Types.IDT_DEVICE_NEO2, DEVICE_INTERFACE_Types.DEVICE_INTERFACE_USB);
            }
            IDT_Device.stopUSBMonitoring();
         }

        public override void FactoryReset()
         {
            try
            {
                // TERMINAL DATA
                TerminalDataFactory tf = new TerminalDataFactory();
                byte[] term = tf.GetFactoryTerminalData5C();
                RETURN_CODE rt = IDT_NEO2.SharedController.emv_setTerminalData(term);
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
                    rt = IDT_NEO2.SharedController.emv_setApplicationData(name, value);
                
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
                    rt = IDT_NEO2.SharedController.emv_setCAPK(value);

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
                Debug.WriteLine("device: FactoryReset() - exception={0}", (object)exp.Message);
            }
        }
        #endregion
    }
}

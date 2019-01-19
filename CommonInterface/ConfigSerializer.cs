using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using IPA.CommonInterface.Helpers;

namespace IPA.CommonInterface
{
    [Serializable]
    public class ConfigSerializer
    {
        private const string JSON_CONFIG = "configuration.json";
        private const string TERMINAL_CONFIG = "TerminalData";

        public TerminalConfiguration terminalCfg;
        private string fileName;
        
        // Accessors
        private Config device_config;
        private ModelFirmware mf = new ModelFirmware();
        private string[] md = new string[0];
        private AIDList aid = new AIDList();
        private CAPKList capk = new CAPKList();
        private TerminalSettings termSettings = new TerminalSettings();
        private EMVTrans transactionValues = new EMVTrans();

        private void DisplayCollection(List<string> collection, string name)
        {
            Debug.WriteLine("device configuration: {0} ----------------------------------- ", (object) name);
            foreach(var item in collection)
            {
                Debug.WriteLine("{0}", (object) item);
            }
        }

        private void DisplayCollection(string [] collection, string name)
        {
            Debug.WriteLine("device configuration: {0} ----------------------------------- ", (object) name);
            foreach(var item in collection)
            {
                Debug.WriteLine("{0}", (object) item);
            }
        }

        private void DisplayCollection(Dictionary<string, string> collection, string name)
        {
            Debug.WriteLine("device configuration: {0} ----------------------------------- ", (object) name);
            foreach(var item in collection)
            {
                Debug.WriteLine("{0}:{1}", item.Key, item.Value);
            }
        }
        private void DisplayCollection(Dictionary<string, string[]> collection, string name)
        {
            foreach(var item in collection)
            {
                Debug.Write("device configuration: " + name + " -------------  =" + item.Key + ": [");
                foreach(var (v, i) in item.Value.Enumerate())
                {
                    Debug.Write(i);
                    if(Convert.ToInt32(v) + 1 < item.Value.Length)
                    {
                        Debug.Write(", ");
                    }
                }
                Debug.WriteLine("]");
            }
        }

        private void DisplayCollection(Dictionary<string,Dictionary<string, string>> collection, string name)
        {
            Debug.WriteLine("device configuration: {0} ----------------------------------- ", (object) name);
            foreach(var item in collection)
            {
                Debug.WriteLine("\n{0}", (object) item.Key);
                Debug.WriteLine("========================================================");
                foreach(var val in item.Value)
                {
                    Debug.WriteLine("{0}:{1}", val.Key, val.Value);
                }
            }
        }

        private SortedDictionary<string, string> GetAllTerminalData(string serialNumber, string EMVKernelVer)
        {
            SortedDictionary<string, string> allTerminalTags = termSettings.TerminalData;
            allTerminalTags["9F1E"] = serialNumber?.Substring(Math.Max(0, serialNumber.Length - 8)) ?? "";
            allTerminalTags["9F1C"] = EMVKernelVer?.Substring(Math.Max(0, EMVKernelVer.Length - 8)) ?? "";
            string [] tagsRequested = termSettings.TransactionTagsRequested;
            allTerminalTags["DFEF5A"] = string.Join("", tagsRequested);

            return allTerminalTags;
        }
        
        public SortedDictionary<string, string> GetTerminalData(string serialNumber, string EMVKernelVer)
        {
            return GetAllTerminalData(serialNumber, EMVKernelVer);
        }

        public string[] GetTerminalDataString(string serialNumber, string EMVKernelVer)
        {
            SortedDictionary<string, string> allTerminalTags = GetAllTerminalData(serialNumber, EMVKernelVer);
            List<string> collection = new List<string>();
            foreach(var item in allTerminalTags)
            {
                collection.Add(string.Format("{0}:{1}", item.Key, item.Value).ToUpper());
            }
            collection.Sort();
            string [] data = collection.ToArray();
            return data;
        }

        public string[] GetAIDCollection()
        {
            List<string> collection = new List<string>();
            foreach(var item in aid.Aid)
            {
                string payload = "";
                foreach(var val in item.Value)
                {
                    payload += string.Format("{0}:{1} ", val.Key, val.Value).ToUpper();
                }
                collection.Add(string.Format("{0}#{1}", item.Key, payload).ToUpper());
            }
            string [] data = collection.ToArray();
            return data;
        }

        public AIDList GetAIDList()
        {
            return aid;
        }

        public string[] GetCapKCollection()
        {
            List<string> collection = new List<string>();
            foreach(var item in capk.CAPK)
            {
                CAPK value = item.Value;
                string payload = "";
                payload += string.Format("{0}:{1} ", "RID", value.RID);
                payload += string.Format("{0}:{1} ", "Index", value.Index);
                payload += string.Format("{0}:{1} ", "Modulus", value.Modulus);
                payload += string.Format("{0}:{1} ", "Exponent", value.Exponent);
                payload += string.Format("{0}:{1}", "Checksum", value.Checksum);
                
                collection.Add(string.Format("{0}#{1}", item.Key, payload).ToUpper());
            }
            string [] data = collection.ToArray();
            return data;
        }

        public CAPKList GetCapKList()
        {
            return capk;
        }

        public TerminalSettings GetTerminalSettings()
        {
            return termSettings;
        }

        public void ReadConfig()
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                string path = System.IO.Directory.GetCurrentDirectory(); 
                fileName = path + "\\" + JSON_CONFIG;
                string FILE_CFG = File.ReadAllText(fileName);

                //string s = @"{ ""ModelFirmware"": { ""VP5300"": ""VP5300 FW v1.00.028.0192.S"", ""IDEM-85XX"": ""ID TECH Augusta S USB-HID V1.02.S"" } }";
                //var Json = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(s);

                terminalCfg = JsonConvert.DeserializeObject<TerminalConfiguration>(FILE_CFG);

                if(terminalCfg != null)
                {
                    // devConfig
                    device_config = terminalCfg.Configuration.First();
                    // Manufacturer
                    Debug.WriteLine("device configuration: manufacturer ----------------: [{0}]", (object) device_config.ConfigurationID.Manufacturer);
                    // Models
                    md = device_config.ConfigurationID.Models;
                    //DisplayCollection(mf.modelFirmware, "modelFirmware");
                    // AID List
                    aid.Aid = device_config.EMVConfiguration.AIDList;
                    //DisplayCollection(aid.Aid, "AIDList");
                    // CAPK List
                    capk.CAPK = device_config.EMVConfiguration.CAPKList;
                    //DisplayCollection(capk.Capk, "CapkList");
                    // Terminal Settings
                    termSettings = device_config.EMVConfiguration.TerminalSettings;
                    //Debug.WriteLine("device configuration: Terminal Settings --------------");
                    //Debug.WriteLine("MajorConfiguration        : {0}", (object) termSettings.MajorConfiguration);
                    //Debug.WriteLine("MajorConfigurationChecksum: {0}", (object) termSettings.MajorConfigurationChecksum[0]);
                    // SerialNumberTag
                    //Debug.WriteLine("device configuration: Serial Number TAG -----------: [{0}]", (object) termSettings.SerialNumberTag);
                    // TerminalData
                    //DisplayCollection(termSettings.TerminalData, "Terminal Data");
                    // TransactionTagsRequested
                    //DisplayCollection(termSettings.TransactionTags, "TransactionTagsRequested");
                    // TransactionValues
                    transactionValues = device_config.EMVTransactionData;
                    //DisplayCollection(transactionValues.EMVKernelMapping, "EMVKernelMapping");
                    //DisplayCollection(transactionValues.TransactionStartTags, "TransactionStartTags");
                    //DisplayCollection(transactionValues.TransactionAuthenticateTags, "TransactionAuthenticateTags");
                    //DisplayCollection(transactionValues.TransactionCompleteTags, "TransactionCompleteTags");
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("JsonSerializer: exception: {0}", (object) ex.Message);
            }
        }

        public void WriteConfig()
        {
            try
            {
                if(terminalCfg != null)
                {
                    // Update timestamp
                    DateTime timenow = DateTime.UtcNow;
                    //user_configuration.last_update_timestamp = JsonConvert.SerializeObject(timenow).Trim('"');
                    //Debug.WriteLine(user_configuration.last_update_timestamp);

                    JsonSerializer serializer = new JsonSerializer();
                    string path = System.IO.Directory.GetCurrentDirectory(); 
                    fileName = path + "\\" + JSON_CONFIG;

                    using (StreamWriter sw = new StreamWriter(fileName))
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                       serializer.Formatting = Formatting.Indented;
                       serializer.Serialize(writer, terminalCfg);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("JsonSerializer: exception: {0}", ex);
            }
        }

        public void WriteTerminalDataConfig()
        {
            try
            {
            }
            catch(Exception ex)
            {
                Debug.WriteLine("JsonSerializer: exception: {0}", ex);
            }
        }

        public string GetFileName()
        {
            return fileName;
        }
    }
}

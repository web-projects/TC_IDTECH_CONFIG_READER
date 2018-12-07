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
        private DeviceConfig device_config;
        private ModelFirmware mf = new ModelFirmware();
        private AIDList aid = new AIDList();
        private CapKList capk = new CapKList();
        private TerminalSettings termSettings = new TerminalSettings();
        private TransactionValues transactionValues = new TransactionValues();

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
        public string[] GetTerminalData()
        {
            List<string> collection = new List<string>();
            foreach(var item in termSettings.TerminalData)
            {
                collection.Add(string.Format("{0}:{1}", item.Key, item.Value).ToUpper());
            }
            string [] data = collection.ToArray();
            return data;
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
                    device_config = terminalCfg.DeviceConfiguration.First();
                    // Manufacturer
                    Debug.WriteLine("device configuration: manufacturer ----------------: [{0}]", (object) device_config.Manufacturer);
                    // ModelFirmware
                    mf.modelFirmware = device_config.ModelFirmware;
                    DisplayCollection(mf.modelFirmware, "modelFirmware");
                    // AID List
                    aid.Aid = device_config.AIDList;
                    DisplayCollection(aid.Aid, "AIDList");
                    // CAPK List
                    capk.Capk = device_config.CapKList;
                    DisplayCollection(capk.Capk, "CapkList");
                    // Terminal Settings
                    termSettings = device_config.TerminalSettings;
                    Debug.WriteLine("device configuration: Terminal Settings --------------");
                    Debug.WriteLine("MajorConfiguration        : {0}", (object) termSettings.MajorConfiguration);
                    Debug.WriteLine("MajorConfigurationChecksum: {0}", (object) termSettings.MajorConfigurationChecksum[0]);
                    // SerialNumberTag
                    Debug.WriteLine("device configuration: Serial Number TAG -----------: [{0}]", (object) termSettings.SerialNumberTag);
                    // TerminalData
                    DisplayCollection(termSettings.TerminalData, "Terminal Data");
                    // TransactionTagsRequested
                    DisplayCollection(termSettings.TransactionTags, "TransactionTagsRequested");
                    // TransactionValues
                    transactionValues = termSettings.TransactionValues;
                    DisplayCollection(transactionValues.EMVKernelMapping, "EMVKernelMapping");
                    DisplayCollection(transactionValues.TransactionStartTags, "TransactionStartTags");
                    DisplayCollection(transactionValues.TransactionAuthenticateTags, "TransactionAuthenticateTags");
                    DisplayCollection(transactionValues.TransactionCompleteTags, "TransactionCompleteTags");
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

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
    public class TerminalConfiguration
    {
        public List<DeviceConfig> DeviceConfiguration { get; set; }
    }

    [Serializable]
    public class DeviceConfig
    {
        [JsonProperty(PropertyName = "Manufacturer", Order = 1)]
        public string Manufacturer { get; set; }
        [JsonProperty(PropertyName = "ModelFirmware", Order = 2)]
        public Dictionary<string, string[]> ModelFirmware { get; set; }
        [JsonProperty(PropertyName = "AIDList", Order = 3)]
        public Dictionary<string, Dictionary<string, string>> AIDList { get; set; }
        [JsonProperty(PropertyName = "CAPKList", Order = 4)]
        public Dictionary<string, Dictionary<string, string>> CapKList { get; set; }
        [JsonProperty(PropertyName = "TerminalSettings", Order = 5)]
        public TerminalSettings TerminalSettings { get; set; }
    }

    [Serializable]
    public class ModelFirmware
    {
        public Dictionary<string, string[]> modelFirmware;
    }

    [Serializable]
    public class AIDList
    {
        public Dictionary<string, Dictionary<string, string>> Aid { get; set; }
    }

    [Serializable]
    public class CapKList
    {
        public Dictionary<string, Dictionary<string, string>> Capk { get; set; }
    }

    [Serializable]
    public class TerminalSettings
    {
        [JsonProperty(PropertyName = "MajorConfiguration", Order = 1)]
        public string MajorConfiguration { get; set; }
        public List<String> MajorConfigurationChecksum { get; set; }
        [JsonProperty(PropertyName = "SerialNumberTag", Order = 2)]
        public string SerialNumberTag { get; set; }
        [JsonProperty(PropertyName = "TerminalData", Order = 3)]
        public Dictionary<string, string> TerminalData { get; set; }
        [JsonProperty(PropertyName = "TransactionTagsRequested", Order = 4)]
        public string [] TransactionTags { get; set; }
        [JsonProperty(PropertyName = "TransactionValues", Order = 5)]
        public TransactionValues TransactionValues { get; set; }
    }

    [Serializable]
    public class TransactionValues
    {
        [JsonProperty(PropertyName = "EMVKernelMapping", Order = 1)]
        public string [] EMVKernelMapping { get; set; }
        //TransactionStartTags
        //TransactionAuthenticateTags
        //TransactionCompleteTags
    }
}

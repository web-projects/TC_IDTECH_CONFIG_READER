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
        public List<Config> Configuration { get; set; }
    }

    [Serializable]
    public class Config
    {
        [JsonProperty(PropertyName = "ConfigurationID", Order = 1)]
        public ConfigurationID ConfigurationID { get; set; }
        [JsonProperty(PropertyName = "EMVConfiguration", Order = 2)]
        public EMVConfig EMVConfiguration { get; set; }
        [JsonProperty(PropertyName = "EMVTransactionData", Order = 3)]
        public EMVTrans EMVTransactionData { get; set; }
    }

    [Serializable]
    public class ConfigurationID
    {
        [JsonProperty(PropertyName = "Manufacturer", Order = 1)]
        public string Manufacturer { get; set; }
        [JsonProperty(PropertyName = "Models", Order = 2)]
        public string[] Models { get; set; }
        [JsonProperty(PropertyName = "Platform", Order = 3)]
        public string Platform { get; set; }
        [JsonProperty(PropertyName = "CardEnvironment", Order = 4)]
        public string CardEnvironment { get; set; }
        [JsonProperty(PropertyName = "EntryModes", Order = 5)]
        public string[] EntryModes { get; set; }
    }

    [Serializable]
    public class ModelFirmware
    {
        public Dictionary<string, string[]> modelFirmware;
    }

    [Serializable]
    public class EMVConfig
    {
        [JsonProperty(PropertyName = "AIDList", Order = 1)]
        public Dictionary<string, Dictionary<string, string>> AIDList { get; set; }
        [JsonProperty(PropertyName = "CAPKList", Order = 2)]
        public Dictionary<string, CAPK> CAPKList { get; set; }
        [JsonProperty(PropertyName = "CRLList", Order = 3)]
        public Dictionary<string, CRL> CRLList { get; set; }
        [JsonProperty(PropertyName = "TerminalSettings", Order = 4)]
        public TerminalSettings TerminalSettings { get; set; }
    }

    [Serializable]
    public class AIDList
    {
        public Dictionary<string, Dictionary<string, string>> Aid { get; set; }
    }

    [Serializable]
    public class CAPKList
    {
        public Dictionary<string, CAPK> CAPK { get; set; }
    }

    [Serializable]
    public class CAPK
    {
        [JsonProperty(PropertyName = "RID", Order = 1)]
        public string RID { get; set; }
        [JsonProperty(PropertyName = "Index", Order = 2)]
        public string Index { get; set; }
        [JsonProperty(PropertyName = "Modulus", Order = 3)]
        public string Modulus { get; set; }
        [JsonProperty(PropertyName = "Exponent", Order = 4)]
        public string Exponent { get; set; }
        [JsonProperty(PropertyName = "Checksum", Order = 5)]
        public string Checksum { get; set; }
    }

    [Serializable]
    public class CRL
    {
        [JsonProperty(PropertyName = "RID", Order = 1)]
        public string RID { get; set; }
        [JsonProperty(PropertyName = "Index", Order = 2)]
        public string Index { get; set; }
        [JsonProperty(PropertyName = "SerialNumber", Order = 3)]
        public string SerialNumber { get; set; }
    }

    [Serializable]
    public class TerminalSettings
    {
        [JsonProperty(PropertyName = "MajorConfiguration", Order = 1)]
        public string MajorConfiguration { get; set; }
        public List<string> MajorConfigurationChecksum { get; set; }
        [JsonProperty(PropertyName = "SerialNumberTag", Order = 2)]
        public string SerialNumberTag { get; set; }
        [JsonProperty(PropertyName = "KernelVersionTag", Order = 3)]
        public string KernelVersionTag { get; set; }
        [JsonProperty(PropertyName = "TerminalData", Order = 4)]
        public Dictionary<string, string> TerminalData { get; set; }
        [JsonProperty(PropertyName = "TransactionTagsRequested", Order = 5)]
        public string [] TransactionTagsRequested { get; set; }
    }

    [Serializable]
    public class EMVTrans
    {
        [JsonProperty(PropertyName = "EMVKernelMapping", Order = 1)]
        public Dictionary<string, string> EMVKernelMapping { get; set; }
        [JsonProperty(PropertyName = "TransactionStartTags", Order = 2)]
        public List<string> TransactionStartTags { get; set; }
        [JsonProperty(PropertyName = "TransactionAuthenticateTags", Order = 3)]
        public List<string> TransactionAuthenticateTags { get; set; }
        [JsonProperty(PropertyName = "TransactionCompleteTags", Order = 4)]
        public List<string> TransactionCompleteTags { get; set; }
    }
}

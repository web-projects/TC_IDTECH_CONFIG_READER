﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPA.CommonInterface.Helpers;
using IPA.CommonInterface.ConfigSphere;

namespace IPA.CommonInterface.Interfaces
{
  public interface IDevicePlugIn
  {
    // Device Events back to Main Form
    event EventHandler<DeviceNotificationEventArgs> OnDeviceNotification;

    // INITIALIZATION
    string PluginName { get; }
    void DeviceInit();
    ConfigSerializer GetConfigSerializer();
    // GUI UPDATE
    string [] GetConfig();
    // NOTIFICATION
    void SetFormClosing(bool state);
    // MSR READER
    void GetCardData();
    // Settings
    void GetDeviceConfiguration();
    // Configuration
    void SetDeviceConfiguration(object data);
    // USB/KYB Mode
    void SetDeviceMode(string mode);
    // QC EMV Mode
    void DisableQCEmvMode();
    string DeviceCommand(string command, bool notify);
    // Messaging
    string GetErrorMessage(string data);
    // Configuration Source
    void SetConfigurationMode(IPA.Core.Shared.Enums.ConfigurationModes mode);
    // TERMINAL DATA
    void GetTerminalData();
    // AID
    void GetAIDList();
    // CAPK
    void GetCapKList();
    void GetConfigGroup(int group);
    // Firmware update
    void FirmwareUpdate(string filename, byte[] bytes);
    // Factory Reset
    void FactoryReset();
  }
}

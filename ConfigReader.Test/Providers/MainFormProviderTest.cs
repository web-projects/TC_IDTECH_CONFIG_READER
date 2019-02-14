using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using IPA.DAL.RBADAL;
using IPA.CommonInterface;
using IPA.CommonInterface.Helpers;

namespace IDTechConfigReader
{
    [TestClass]
    public class MainFormProviderTest
    {
        Mock<IDevicePlugIn> fakedevicePlugin;

        [TestInitialize]
        public void Before()
        {
            fakedevicePlugin = new Mock<IDevicePlugIn>();
            //fakedevicePlugin = new Mock<IPA.DAL.RBADAL.DeviceCfg() as IDevicePlugIn>;
        }

        [TestMethod]
        public void ShouldDisplayFirmwareUpdateProgressBar()
        {
            string file = "C:\\Development\\CSharp\\DEVICES\\IDTech\\Augusta\\IDTechConfigReader\\IDTechConfigReader\\bin\\Debug\\Assets\\VP5300 v1.00.029.0193.S.fm";
            byte [] bytes = System.IO.File.ReadAllBytes(file);;
            if(bytes.Length > 0)
            {
                Thread.CurrentThread.IsBackground = false;

                for(int i = 1; i <= bytes.Length / 1024; i++)
                {
                    Debug.WriteLine("device: sent block {0} of {1}", i.ToString(), (bytes.Length / 1024).ToString());
                    string [] message = { i.ToString() };
                    //fakedevicePlugin.NotificationRaise(new DeviceNotificationEventArgs { NotificationType = NOTIFICATION_TYPE.NT_FIRMWARE_UPDATE_STEP, Message = message });
                    Thread.Sleep(10);
                }
                Thread.Sleep(1000);
                //fakedevicePlugin.SetDeviceFirmwareVersion();
            }
        }
    }
}

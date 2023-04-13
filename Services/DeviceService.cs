using System.Xml;
using PblMauiShipment.ViewModels;

namespace PblMauiShipment.Services {
  public class DeviceService {
    public string DeviceModel { get; set; }
    public string DeviceManufacturer { get; set; }
    public string DeviceName { get; set; }
    public string DeviceOSVersion { get; set; }
    public IDeviceInfo DeviceIDeviceInfo { get; set; }
    public DeviceIdiom DeviceDeviceIdiom { get; set; }
    public DevicePlatform DeviceDevicePlatform { get; set; }
    public bool DeviceIsVirtual { get; set; }

    public void ReadDeviceInfo() {

      DeviceModel = DeviceInfo.Current.Model;
      DeviceManufacturer = DeviceInfo.Current.Manufacturer;
      DeviceName = DeviceInfo.Name;
      DeviceOSVersion = DeviceInfo.VersionString;
      DeviceIDeviceInfo = DeviceInfo.Current;
      DeviceDeviceIdiom = DeviceInfo.Current.Idiom;
      DeviceDevicePlatform = DeviceInfo.Current.Platform;


      DeviceIsVirtual = DeviceInfo.Current.DeviceType switch {
        DeviceType.Physical => false,
        DeviceType.Virtual => true,
        _ => false
      };

    }

  }
}

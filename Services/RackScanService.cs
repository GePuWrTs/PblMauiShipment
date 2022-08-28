using System.Xml;
using PblMauiShipment.ViewModels;

namespace PblMauiShipment.Services {
  public class RackScanService {
    private string DataDirectory;

   

    public RackScanService() {
      DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "data");
      Directory.CreateDirectory(DataDirectory);
      ReadDeviceInfo();
    }

    List<RackScan> rackScanList = new();

    public async Task SaveRackScanListToXml(List<RackScan> rackScanList, ScanType scanType) {
      if (rackScanList != null) {
        XmlDocument racks = new();
      }
      await Task.CompletedTask;
    }

    public string CreateRackScanFileName(ScanType scanType) {
      DateTime creationDate = DateTime.Now;
      string filename = string.Empty;

      switch (scanType) {
        case ScanType.incoming: 
          filename = "RAI";
          break;
        case ScanType.outgoing:
          filename = "RAO";
          break;
        case ScanType.create:
          filename = "REG";
          break;
      }

      filename = string.Format("{0}-{1}-{2}.{3}.{4}-{5}.xml",
                                filename,
                                    DeviceName,
                                        creationDate.Year,
                                             creationDate.Month.ToString().PadLeft(2, '0'),
                                                creationDate.Day.ToString().PadLeft(2, '0'),
                                                     creationDate.ToLongTimeString().Replace(':', '.'));

      return filename; 
    }


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

    public async Task<List<RackScan>> GetMockRackScanList() {
      rackScanList.Clear();
      rackScanList.Add(new RackScan() { ItemID = 1, Barcode = "00123", Type = ScanType.incoming, Scanned = DateTime.Parse("01.08.2022 08:01:00") });
      rackScanList.Add(new RackScan() { ItemID = 2, Barcode = "00124", Type = ScanType.incoming, Scanned = DateTime.Parse("01.08.2022 09:02:00") });
      rackScanList.Add(new RackScan() { ItemID = 3, Barcode = "00125", Type = ScanType.incoming, Scanned = DateTime.Parse("01.08.2022 10:03:00") });

      rackScanList.Add(new RackScan() { ItemID = 4, Barcode = "00223", Type = ScanType.outgoing, Scanned = DateTime.Parse("02.08.2022 08:01:00") });
      rackScanList.Add(new RackScan() { ItemID = 5, Barcode = "00224", Type = ScanType.outgoing, Scanned = DateTime.Parse("02.08.2022 09:02:00") });
      rackScanList.Add(new RackScan() { ItemID = 6, Barcode = "00225", Type = ScanType.outgoing, Scanned = DateTime.Parse("02.08.2022 10:03:00") });

      rackScanList.Add(new RackScan() { ItemID = 7, Barcode = "00323", Type = ScanType.create, Scanned = DateTime.Parse("03.08.2022 08:01:00"), OwnerID = 1, OwnerName = "Porta" });
      rackScanList.Add(new RackScan() { ItemID = 8, Barcode = "00324", Type = ScanType.create, Scanned = DateTime.Parse("03.08.2022 09:02:00"), OwnerID = 2, OwnerName = "Schuett" });
      rackScanList.Add(new RackScan() { ItemID = 9, Barcode = "00325", Type = ScanType.create, Scanned = DateTime.Parse("03.08.2022 10:03:00"), OwnerID = 3, OwnerName = "Scholl" });

      rackScanList.Add(new RackScan() { ItemID = 10, Barcode = "00423", Type = ScanType.incoming, Scanned = DateTime.Parse("01.08.2022 08:01:00") });
      rackScanList.Add(new RackScan() { ItemID = 11, Barcode = "00424", Type = ScanType.incoming, Scanned = DateTime.Parse("01.08.2022 09:02:00") });
      rackScanList.Add(new RackScan() { ItemID = 12, Barcode = "00425", Type = ScanType.incoming, Scanned = DateTime.Parse("01.08.2022 10:03:00") });

      rackScanList.Add(new RackScan() { ItemID = 13, Barcode = "00523", Type = ScanType.outgoing, Scanned = DateTime.Parse("02.08.2022 08:01:00") });
      rackScanList.Add(new RackScan() { ItemID = 14, Barcode = "00524", Type = ScanType.outgoing, Scanned = DateTime.Parse("02.08.2022 09:02:00") });
      rackScanList.Add(new RackScan() { ItemID = 15, Barcode = "00525", Type = ScanType.outgoing, Scanned = DateTime.Parse("02.08.2022 10:03:00") });

      return await Task.FromResult(rackScanList);
    }


  }
}

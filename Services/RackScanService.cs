using System.Globalization;
using System.Text;
using System.Xml;
using PblMauiShipment.ViewModels;

namespace PblMauiShipment.Services {
  public class RackScanService {
    private string mDataDirectory;
    private Device mDeviceInfo = new();
    CultureInfo mProvider = new CultureInfo("de-DE");


    public RackScanService() {
      mDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "data");
      Directory.CreateDirectory(mDataDirectory);
      mDeviceInfo.ReadDeviceInfo();
    }

    List<RackScan> rackScanList = new();

    public async Task<bool> SaveRackScanListToXml(ObservableCollection<RackScan> rackScanList, ScanType scanType) {
      bool result = false;
      if ((rackScanList != null) && (rackScanList.Count > 0)) {

        StringBuilder sb = new StringBuilder(Path.Combine(mDataDirectory, BuildRackScanFileName(scanType)));
        string XRootNodeName = String.Empty;

        FileInfo FileInfoXDoc = new FileInfo(sb.ToString());
        // FileInfoXDoc.Create();

        switch (scanType) {
          case ScanType.incoming:
            XRootNodeName = "RAI";
            break;
          case ScanType.outgoing:
            XRootNodeName = "RAO";
            break;
          case ScanType.create:
            XRootNodeName = "REG";
            break;
          default:
            XRootNodeName = "ERROR";
            break;
        }

        XmlDocument XDoc = new XmlDocument();
        XmlElement XRoot = XDoc.CreateElement(XRootNodeName);
        XDoc.AppendChild(XRoot);
        XmlElement deviceID = XDoc.CreateElement("DeviceID");
        deviceID.InnerText = mDeviceInfo.DeviceName;
        XRoot.AppendChild(deviceID);
        XmlElement created = XDoc.CreateElement("Created");
        created.InnerText = DateTime.Now.ToString(mProvider);
        XRoot.AppendChild(created);
        XmlElement closed = XDoc.CreateElement("Closed");
        closed.InnerText = DateTime.MinValue.ToString(mProvider);
        XRoot.AppendChild(closed);
        XmlElement XLines = XDoc.CreateElement("Items"); ;

        int index = 0;
        foreach (RackScan rack in rackScanList) {
          index += 1;
          if (index == 1) {
            created.InnerText = rack.Scanned.ToString(mProvider);
          }
          XmlElement XItem = XDoc.CreateElement("Item");
          XmlElement ItemId = XDoc.CreateElement("ItemID");
          ItemId.InnerText = index.ToString();
          XItem.AppendChild(ItemId);
          XmlElement Barcode = XDoc.CreateElement("Barcode");
          Barcode.InnerText = rack.Barcode;
          XItem.AppendChild(Barcode);
          XmlElement Scanned = XDoc.CreateElement("Scanned");
          Scanned.InnerText = rack.Scanned.ToString(mProvider);
          XItem.AppendChild(Scanned);
          XLines.AppendChild(XItem);
        }
        XRoot.AppendChild(XLines);

        closed.InnerText = DateTime.Now.ToString(mProvider);
        XDoc.Save(FileInfoXDoc.FullName);
        if (FileInfoXDoc.Exists)
          result = true;
      }
      return await Task.FromResult(result);

    }


    public string BuildRackScanFileName(ScanType scanType) {
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
                                    mDeviceInfo.DeviceName,
                                        creationDate.Year,
                                             creationDate.Month.ToString().PadLeft(2, '0'),
                                                creationDate.Day.ToString().PadLeft(2, '0'),
                                                     creationDate.ToLongTimeString().Replace(':', '.'));

      return filename;
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

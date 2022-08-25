

namespace PblMauiShipment.Services {
  public class RackScanService {

    public RackScanService() {

    }

    List<RackScan> rackScanList = new();

    public async Task<List<RackScan>> GetMockRackScanList() {
      rackScanList.Clear();
      rackScanList.Add(new RackScan() { ItemID = 1, Barcode = "00123", Type = ScanType.incoming, Scanned = DateTime.Parse("01.08.2022 08:01:00") });
      rackScanList.Add(new RackScan() { ItemID = 2, Barcode = "00124", Type = ScanType.incoming, Scanned = DateTime.Parse("01.08.2022 09:02:00") });
      rackScanList.Add(new RackScan() { ItemID = 3, Barcode = "00125", Type = ScanType.incoming, Scanned = DateTime.Parse("01.08.2022 10:03:00") });

      rackScanList.Add(new RackScan() { ItemID = 4, Barcode = "00223", Type = ScanType.outgoing, Scanned = DateTime.Parse("02.08.2022 08:01:00") });
      rackScanList.Add(new RackScan() { ItemID = 5, Barcode = "00224", Type = ScanType.outgoing, Scanned = DateTime.Parse("02.08.2022 09:02:00") });
      rackScanList.Add(new RackScan() { ItemID = 6, Barcode = "00225", Type = ScanType.outgoing, Scanned = DateTime.Parse("02.08.2022 10:03:00") });

      rackScanList.Add(new RackScan() {
        ItemID = 7,
        Barcode = "00323",
        Type = ScanType.create,
        Scanned = DateTime.Parse("03.08.2022 08:01:00"),
        OwnerID = 1,
        OwnerName = "Porta"
      });
      rackScanList.Add(new RackScan() {
        ItemID = 8,
        Barcode = "00324",
        Type = ScanType.create,
        Scanned = DateTime.Parse("03.08.2022 09:02:00"),
        OwnerID = 2,
        OwnerName = "Schuett"
      });
      rackScanList.Add(new RackScan() {
        ItemID = 9,
        Barcode = "00325",
        Type = ScanType.create,
        Scanned = DateTime.Parse("03.08.2022 10:03:00"),
        OwnerID = 3,
        OwnerName = "Scholl"
      });


      rackScanList.Add(new RackScan() { ItemID = 1, Barcode = "00123", Type = ScanType.incoming, Scanned = DateTime.Parse("01.08.2022 08:01:00") });
      rackScanList.Add(new RackScan() { ItemID = 2, Barcode = "00124", Type = ScanType.incoming, Scanned = DateTime.Parse("01.08.2022 09:02:00") });
      rackScanList.Add(new RackScan() { ItemID = 3, Barcode = "00125", Type = ScanType.incoming, Scanned = DateTime.Parse("01.08.2022 10:03:00") });

      rackScanList.Add(new RackScan() { ItemID = 4, Barcode = "00223", Type = ScanType.outgoing, Scanned = DateTime.Parse("02.08.2022 08:01:00") });
      rackScanList.Add(new RackScan() { ItemID = 5, Barcode = "00224", Type = ScanType.outgoing, Scanned = DateTime.Parse("02.08.2022 09:02:00") });
      rackScanList.Add(new RackScan() { ItemID = 6, Barcode = "00225", Type = ScanType.outgoing, Scanned = DateTime.Parse("02.08.2022 10:03:00") });

      return await Task.FromResult(rackScanList);
    }


  }
}

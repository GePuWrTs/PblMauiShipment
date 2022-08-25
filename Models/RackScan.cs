
namespace PblMauiShipment.Models {

  public enum ScanType { incoming, outgoing, create }
  public class RackScan {
    public int ItemID { get; set; }
    public ScanType Type { get; set; }
    public string Barcode { get; set; }
    public DateTime Scanned { get; set; }
    public int OwnerID { get; set; }
    public string OwnerName { get; set; }
  }
}

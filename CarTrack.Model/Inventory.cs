using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarTrack.Model
{
    public class InventoryBeacon
    {
        public string BeaconID { get; set; }

        public string EmailAddress { get; set; }

        public bool? IsRegistered { get; set; }

        public bool InventoryStatus { get; set; }

        public DateTime? InventoryIncDate { get; set; }

        public string BeaconUDID { get; set; }

        public decimal? Major { get; set; }

        public decimal? Minor { get; set; }

        public string ProximityUDID { get; set; }

        public string Color { get; set; }
    }
    public class AvailableBeaconResponse : ServiceResponseBase
    {
        public List<InventoryBeacon> Beacons { get; set; }
    }
    public class AvailableBeaconRequest : ServiceRequestBase
    {
        public string Beacons { get; set; }
    }
}

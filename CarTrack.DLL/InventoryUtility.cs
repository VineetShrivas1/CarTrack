using CarTrack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarTrack.DAL
{
    public class InventoryUtility
    {

        public static List<InventoryBeacon> GetUnregisteredBeacons()
        {
            List<InventoryBeacon> lst = new List<InventoryBeacon>();
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                var beacons = myContext.DeviceInventories.Where(x => x.IsRegistered == false);
                if (beacons != null && beacons.Count() > 0)
                {
                    foreach (var b in beacons)
                    {
                        InventoryBeacon beacon = new InventoryBeacon();
                        beacon.BeaconID = b.BeaconID;
                        beacon.EmailAddress = b.EmailAddress;
                        beacon.InventoryIncDate = b.InventoryIncDate;
                        beacon.InventoryStatus = b.InventoryStatus;
                        beacon.IsRegistered = b.IsRegistered;
                        beacon.BeaconUDID = b.BeaconUDID;
                        lst.Add(beacon);
                    }
                }
            }
            return lst;
        }

        public static ServiceResponseBase AddInventory(DeviceInventory inv)
        {
            string errorCode = "0";
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                var old = myContext.DeviceInventories.Where(x => x.BeaconID.ToLower() == inv.BeaconID.ToLower() ||
                x.BeaconUDID.ToLower() == inv.BeaconUDID.ToLower()).FirstOrDefault();
                if (old != null)
                {
                    errorCode = "Err0015";//Beacon ID alrealy added.
                }
                else
                {
                    myContext.DeviceInventories.Add(inv);
                    myContext.SaveChanges();
                }

            }
            return CommonUtility.GetStandardErrorMessage<ServiceResponseBase>(errorCode);
        }

        public static List<InventoryBeacon> GetRegisteredBeacons()
        {
            List<InventoryBeacon> lst = new List<InventoryBeacon>();
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                var beacons = myContext.DeviceInventories.Where(x => x.IsRegistered == true);
                if (beacons != null && beacons.Count() > 0)
                {
                    foreach (var b in beacons)
                    {
                        InventoryBeacon beacon = new InventoryBeacon();
                        beacon.BeaconID = b.BeaconID;
                        beacon.EmailAddress = b.EmailAddress;
                        beacon.InventoryIncDate = b.InventoryIncDate;
                        beacon.InventoryStatus = b.InventoryStatus;
                        beacon.IsRegistered = b.IsRegistered;
                        beacon.BeaconUDID = b.BeaconUDID;
                        lst.Add(beacon);
                    }
                }
            }
            return lst;
        }

        public static List<InventoryBeacon> GetAllBeacons()
        {
            List<InventoryBeacon> lst = new List<InventoryBeacon>();
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                var beacons = myContext.DeviceInventories;
                if (beacons != null && beacons.Count() > 0)
                {
                    foreach (var b in beacons)
                    {
                        InventoryBeacon beacon = new InventoryBeacon();
                        beacon.BeaconID = b.BeaconID;
                        beacon.EmailAddress = b.EmailAddress;
                        beacon.InventoryIncDate = b.InventoryIncDate;
                        beacon.InventoryStatus = b.InventoryStatus;
                        beacon.IsRegistered = b.IsRegistered;
                        beacon.BeaconUDID = b.BeaconUDID;
                        lst.Add(beacon);
                    }
                }
            }
            return lst;
        }

    }
}

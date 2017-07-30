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
                        beacon.Minor = b.Minor;
                        beacon.Major = b.Major;
                        beacon.Color = b.Color;
                        beacon.ProximityUDID = b.ProximityUDID;
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

        public static ServiceResponseBase AddInventory(List<InventoryBeacon> lst)
        {
            string errorCode = "0";
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                var inv = (from l in lst
                           select new DeviceInventory()
                           {
                               BeaconID = l.BeaconID,
                               BeaconUDID = l.BeaconUDID,
                               Color = l.Color,
                               InventoryIncDate = DateTime.Now,
                               InventoryStatus = true,
                               IsRegistered = false,
                               Major = int.Parse(l.Major.ToString()),
                               Minor = int.Parse(l.Minor.ToString()),
                               ProximityUDID = l.ProximityUDID

                           }).ToList();
                foreach (var i in inv)
                    myContext.DeviceInventories.Add(i);

                myContext.SaveChanges();

            }
            return CommonUtility.GetStandardErrorMessage<ServiceResponseBase>(errorCode);
        }

        public static LoginResponse Login(LoginModel loginModel)
        {
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                LoginResponse response = new LoginResponse();

                string errorCode = "0";
                ErrorMessageMaster error = null;
                var user = myContext.AdminUserInfoes.FirstOrDefault(x => x.UserID == loginModel.UserID);

                if (user == null || (user != null && user.UserPassword != loginModel.UserPassword))
                {
                    errorCode = "Err0009";//Invalid User ID/ Password.
                }
                else
                {
                    response.LastLoginDate = user.LastLoginDate;
                    response.UserID = user.UserID;
                    user.LastLoginDate = DateTime.Now;
                    myContext.SaveChanges();
                }
                error = CommonUtility.GetStandardErrorMessage(errorCode);
                if (error != null)
                {
                    response.ErrorCode = error.ErrorCode;
                    response.ErrorFound = error.ErrorFound;
                    response.ErrorMessage = error.ErrorMessage;

                    return response;
                }
            }

            return new LoginResponse()
            {
                ErrorCode = "ErrAPI01",
                ErrorMessage = "Something went wrong.", //string.Join(",", errors),
                ErrorFound = true
            };

        }
    }
}

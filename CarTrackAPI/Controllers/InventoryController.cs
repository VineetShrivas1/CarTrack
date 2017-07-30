using CarTrack.DAL;
using CarTrack.Model;
using CarTrackAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace CarTrackAPI.Controllers
{
    public class InventoryController : ApiController
    {
        JavaScriptSerializer serialize = new JavaScriptSerializer();
        string json = string.Empty;

        public List<InventoryBeacon> GetUnregisteredBeacons()
        {
            return InventoryUtility.GetUnregisteredBeacons();
        }
        public List<InventoryBeacon> GetRegisteredBeacons()
        {
            return InventoryUtility.GetRegisteredBeacons();
        }
        public List<InventoryBeacon> GetAllBeacons()
        {
            return InventoryUtility.GetAllBeacons();
        }
        public ServiceResponseBase AddInventory(InventoryBeacon inventory)
        {
            if (inventory != null)
            {
                DeviceInventory inv = new DeviceInventory();
                inv.BeaconID = inventory.BeaconID;
                inv.BeaconUDID = inventory.BeaconUDID;
                inv.EmailAddress = inventory.EmailAddress;
                inv.InventoryIncDate = DateTime.Now;
                inv.IsRegistered = false;
                inv.InventoryStatus = false;

                return InventoryUtility.AddInventory(inv);
            }

            return null;
        }

        public AvailableBeaconResponse AvailableBeacons([FromBody]AvailableBeaconRequest request)
        {
            json = serialize.Serialize(request);
            LoggingUtility.Log(json, true);

            //LoggingUtility.Log(request.Beacons);
            ErrorMessageMaster error = null;
            AvailableBeaconResponse response = new AvailableBeaconResponse();
            string errorCode = "0";
            try
            {
                UserInformation user = AccountUtility.GetUserInfo(request.UserID);
                if (user != null)
                {

                    var beacons = InventoryUtility.GetUnregisteredBeacons();
                    if (beacons != null)
                    {
                        var lst = request.Beacons.ToLower().Split(",".ToCharArray());
                        beacons = (from b in beacons where lst.Contains(b.BeaconID.ToLower()) || lst.Contains(b.BeaconUDID.ToLower()) select b).ToList();

                        response.Beacons = beacons;
                    }
                }
                else
                {
                    errorCode = "Err0008";
                }
            }
            catch (Exception ex)
            {
                return new AvailableBeaconResponse()
                {
                    ErrorCode = "ErrAPI01",
                    ErrorMessage = ex.Message,
                    ErrorFound = false
                };
            }
            error = CommonUtility.GetStandardErrorMessage(errorCode);
            if (error != null)
            {
                response.ErrorCode = error.ErrorCode;
                response.ErrorFound = error.ErrorFound;
                response.ErrorMessage = error.ErrorMessage;

                return response;
            }

            return new AvailableBeaconResponse()
            {
                ErrorCode = "ErrAPI01",
                ErrorMessage = "Something went wrong.",
                ErrorFound = false
            };
        }
    }
}

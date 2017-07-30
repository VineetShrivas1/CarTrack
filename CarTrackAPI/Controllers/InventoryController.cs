using CarTrack.DAL;
using CarTrack.Model;
using CarTrackAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
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
        //public ServiceResponseBase AddInventory(InventoryBeacon inventory)
        //{
        //    if (inventory != null)
        //    {
        //        DeviceInventory inv = new DeviceInventory();
        //        inv.BeaconID = inventory.BeaconID;
        //        inv.BeaconUDID = inventory.BeaconUDID;
        //        inv.EmailAddress = inventory.EmailAddress;
        //        inv.InventoryIncDate = DateTime.Now;
        //        inv.IsRegistered = false;
        //        inv.InventoryStatus = false;

        //        return InventoryUtility.AddInventory(inv);
        //    }

        //    return null;
        //}

        public AvailableBeaconResponse AvailableBeacons([FromBody]AvailableBeaconRequest
            request)
        {
            json = serialize.Serialize(request);
            LoggingUtility.Log(json, true);

            ErrorMessageMaster error = null;
            AvailableBeaconResponse response = new AvailableBeaconResponse();
            string errorCode = "0";
            try
            {
                UserInformation user = AccountUtility.GetUserInfo(request.UserID);
                if (user != null)
                {

                    var beacons = InventoryUtility.GetUnregisteredBeacons();
                    var deletedBeacons = CarUtility.GetDeletedDevices();
                    var defaultSetting = AccountUtility.GetDefaultSettings();
                    if (beacons != null)
                    {

                        var lst = request.Beacons.ToLower().Split(",".ToCharArray());
                        var deletedBeaconId = (from b in deletedBeacons select b.BeaconID).ToList();

                        var available = (from b in beacons
                                         where (lst.Contains(b.BeaconID.ToLower())
                       || lst.Contains(b.BeaconUDID.ToLower()))
                       && !deletedBeaconId.Contains(b.BeaconID)
                                         select b).ToList();

                        var filterDeleted = (from d in deletedBeacons
                                             join b in beacons on d.BeaconID equals b.BeaconID
                                             where
                                             (lst.Contains(b.BeaconID.ToLower())
                                              || lst.Contains(b.BeaconUDID.ToLower()))
                                                &&
                                             (defaultSetting != null && defaultSetting.AddDeviceAfterDays.HasValue
                                             && d.UserID != request.UserID
                                                && d.DeletionDate.HasValue
                                                && DateTime.Now.Date >=
                                                d.DeletionDate.Value.AddDays(defaultSetting.AddDeviceAfterDays.Value).Date
                                                || d.UserID == request.UserID)
                                             select new InventoryBeacon
                                             {
                                                 BeaconID = d.BeaconID,
                                                 EmailAddress = request.UserID,
                                                 IsRegistered = false,
                                                 InventoryStatus = true,
                                                 BeaconUDID = b.BeaconUDID,
                                                 InventoryIncDate = b.InventoryIncDate,
                                                 ProximityUDID = b.ProximityUDID,
                                                 Major = b.Major,
                                                 Minor = b.Minor,
                                                 Color = b.Color

                                             }).ToList();

                        available.AddRange(filterDeleted);
                        response.Beacons = available;

                        // response.Beacons.ForEach(x =>

                        //{
                        //    x.Major = Convert.ToDecimal(x.BeaconUDID.Replace(":", "").Split("++".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0]);
                        //    x.Minor = Convert.ToDecimal(x.BeaconUDID.Replace(":", "").Split("++".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1]);
                        //}
                        // );
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

        public AvailableBeaconResponse FilterBeacons([FromBody]AvailableBeaconRequest
            request)
        {
            json = serialize.Serialize(request);
            LoggingUtility.Log(json, true);

            ErrorMessageMaster error = null;
            AvailableBeaconResponse response = new AvailableBeaconResponse();
            string errorCode = "0";
            try
            {
                var beacons = InventoryUtility.GetAllBeacons();
                if (beacons != null)
                {
                    var lst = request.Beacons.ToLower().Split(",".ToCharArray());

                    var beaconids = (from f in beacons select f.BeaconID).ToList();

                    var available = (from b in lst
                                     where !beaconids.Contains(b.ToLower())

                                     select new InventoryBeacon()
                                     {
                                         BeaconID = b
                                     }).ToList();

                    response.Beacons = available;
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

        public ServiceResponseBase AddInventory([FromBody] List<InventoryBeacon> request)
        {
            json = serialize.Serialize(request);
            LoggingUtility.Log(json, true);

            try
            {
                return InventoryUtility.AddInventory(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponseBase()
                {
                    ErrorCode = "ErrAPI01",
                    ErrorMessage = ex.Message,
                    ErrorFound = false
                };
            }

        }

        public LoginResponse Login([FromBody] LoginModel request)
        {
            json = serialize.Serialize(request);
            LoggingUtility.Log("Admin Login--" + json, true);

            APILog log = new APILog() { RequestDate = DateTime.Now };
            LoginResponse response = null;
            try
            {
                #region Validation

                if (!ModelState.IsValid)
                {
                    List<string> errors = new List<string>();
                    foreach (ModelState modelState in ModelState.Values)
                    {
                        foreach (ModelError e in modelState.Errors.Where(x => !String.IsNullOrEmpty(x.ErrorMessage)))
                        {
                            errors.Add(e.ErrorMessage);
                            break;
                        }
                    }
                    if (errors.Count > 0)
                    {
                        response = CommonUtility.GetStandardErrorMessage<LoginResponse>(errors.FirstOrDefault());

                    }

                }

                #endregion

                if (response == null || !response.ErrorFound)
                {
                    string outMsg = "";
                    request.UserPassword = CryptoUtility.Encrypt(Constants.Key, request.UserPassword, out outMsg);

                    if (String.IsNullOrEmpty(outMsg))
                    {
                        response = InventoryUtility.Login(request);
                    }
                    else
                    {
                        response = new LoginResponse()
                        {
                            ErrorCode = "ErrAPI01",
                            ErrorMessage = outMsg,
                            ErrorFound = true
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                response = new LoginResponse()
                {
                    ErrorCode = "ErrAPI01",
                    ErrorMessage = ex.Message, //string.Join(",", errors),
                    ErrorFound = true
                };
            }
            finally
            {
                log.APICode = 2001;
                log.ErrorCode = response.ErrorCode;
                log.ErrorFound = response.ErrorFound;
                log.APIResponse = response.ErrorMessage;
                log.IPAddress = request.IPAddress;
                log.ResponseDate = DateTime.Now;
                log.Source = request.Source;
                log.UserID = request.UserID;
                log.UserDeviceID = request.UserDeviceID;

                LogUtility.Log(log);

                json = serialize.Serialize(response);
                LoggingUtility.Log("Admin Login--" + json, false);
            }
            return response;

        }
    }
}

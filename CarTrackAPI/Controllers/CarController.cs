
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CarTrack.Model;
using System.Web.Http.ModelBinding;
using CarTrackAPI.Utilities;
using System.Runtime.Caching;
using CarTrack.DAL;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Web.Script.Serialization;

namespace CarTrackAPI.Controllers
{
    //public class MyActionFilter : ActionFilterAttribute
    //{
    //    public override void OnActionExecuting(HttpActionContext actionContext)
    //    {
    //     var routeDate=actionContext.Request.reque
    //        //....
    //    }

    //    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    //    {
    //        //....
    //    }
    //}
    public class CarController : ApiController
    {
        JavaScriptSerializer serialize = new JavaScriptSerializer();
        string json = string.Empty;

        public ServiceResponseBase AddCar([FromBody] CarModel request)
        {

            APILog log = new APILog() { RequestDate = DateTime.Now };
            ServiceResponseBase response = null;
            try
            {
                string errorCode = string.Empty;

                #region Validation

                if (!ModelState.IsValid)
                {

                    foreach (ModelState modelState in ModelState.Values)
                    {
                        foreach (ModelError e in modelState.Errors.Where(x => !String.IsNullOrEmpty(x.ErrorMessage)))
                        {
                            errorCode = e.ErrorMessage;
                            break;
                        }
                    }

                }

                #endregion

                #region Authentication
                if (String.IsNullOrEmpty(errorCode))
                    errorCode = TokenUtility.IsValidToken(request.UserID, errorCode);

                #endregion

                if (!String.IsNullOrEmpty(errorCode))
                    response = CommonUtility.GetStandardErrorMessage<ServiceResponseBase>(errorCode);

                if (response == null || !response.ErrorFound)
                {
                    MyCar myCar = new MyCar();
                    myCar.BeaconID = request.BeaconID;
                    myCar.CarName = request.BeaconName;
                    myCar.IsFavorite = request.IsFavourite;
                    myCar.BeaconImage = request.BeaconImage;
                    myCar.UserID = request.UserID;
                    myCar.DeviceStatus = true;
                    myCar.IPAddress = request.IPAddress;
                    myCar.RegistrationDate = DateTime.Now;
                    myCar.UserDeviceID = request.UserDeviceID;
                    myCar.BeaconUDID = request.BeaconUDID;
                    myCar.IsDeleted = false;
                    response = CarUtility.AddCar(myCar);
                }
            }
            catch (Exception ex)
            {
                response = new ServiceResponseBase()
                {
                    ErrorCode = "ErrAPI01",
                    ErrorMessage = ex.Message,
                    ErrorFound = true
                };
            }
            finally
            {
                log.APICode = 1007;
                log.ErrorCode = response.ErrorCode;
                log.ErrorFound = response.ErrorFound;
                log.APIResponse = response.ErrorMessage;
                log.IPAddress = request.IPAddress;
                log.ResponseDate = DateTime.Now;
                log.Source = request.Source;
                log.UserID = request.UserID;
                log.UserDeviceID = request.UserDeviceID;

                LogUtility.Log(log);
            }
            return response;

        }

        public ServiceResponseBase AddDevice([FromBody] CarModel request)
        {
            json = serialize.Serialize(request);
            LoggingUtility.Log("AddDevice--" + json, true);

            APILog log = new APILog() { RequestDate = DateTime.Now };
            ServiceResponseBase response = null;
            try
            {
                string errorCode = string.Empty;

                #region Validation

                if (!ModelState.IsValid)
                {

                    foreach (ModelState modelState in ModelState.Values)
                    {
                        foreach (ModelError e in modelState.Errors.Where(x => !String.IsNullOrEmpty(x.ErrorMessage)))
                        {
                            errorCode = e.ErrorMessage;
                            break;
                        }
                    }

                }

                #endregion

                #region Authentication
                if (String.IsNullOrEmpty(errorCode))
                    errorCode = TokenUtility.IsValidToken(request.UserID, errorCode);

                #endregion

                if (!String.IsNullOrEmpty(errorCode))
                    response = CommonUtility.GetStandardErrorMessage<ServiceResponseBase>(errorCode);

                if (response == null || !response.ErrorFound)
                {
                    MyCar myCar = new MyCar();
                    myCar.BeaconID = request.BeaconID;
                    myCar.CarName = request.BeaconName;
                    myCar.IsFavorite = request.IsFavourite;
                    myCar.BeaconImage = request.BeaconImage;
                    myCar.UserID = request.UserID;
                    myCar.DeviceStatus = true;
                    myCar.IPAddress = request.IPAddress;
                    myCar.RegistrationDate = DateTime.Now;
                    myCar.UserDeviceID = request.UserDeviceID;
                    myCar.BeaconUDID = request.BeaconUDID;
                    response = CarUtility.AddCar(myCar);
                }
            }
            catch (Exception ex)
            {
                response = new ServiceResponseBase()
                {
                    ErrorCode = "ErrAPI01",
                    ErrorMessage = ex.Message,
                    ErrorFound = true
                };
            }
            finally
            {
                log.APICode = 1007;
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
                LoggingUtility.Log("AddDevice--" + json, false);

            }
            return response;

        }

        [HttpGet]
        public CarModelResponse MyCars(string userID)
        {
            string errorCode = String.Empty;
            #region Authentication
            if (String.IsNullOrEmpty(errorCode))
                errorCode = TokenUtility.IsValidToken(userID, errorCode);

            #endregion

            if (!String.IsNullOrEmpty(errorCode))
                return CommonUtility.GetStandardErrorMessage<CarModelResponse>(errorCode);
            try
            {
                return CarUtility.MyCars(userID);

            }
            catch (Exception ex)
            {
                return new CarModelResponse()
                {
                    ErrorCode = "ErrAPI01",
                    ErrorMessage = ex.Message,
                    ErrorFound = true
                };
            }

            // return new CarModel() { };
        }

        //[Obsolete]
        //public ServiceResponseBase RemoveCar([FromBody] RemoveCar request)
        //{
        //    json = serialize.Serialize(request);
        //    LoggingUtility.Log(json, true);

        //    APILog log = new APILog() { RequestDate = DateTime.Now };
        //    ServiceResponseBase response = null;
        //    try
        //    {

        //        string errorCode = String.Empty;
        //        //#region Authentication
        //        //if (String.IsNullOrEmpty(errorCode))
        //        //    errorCode = TokenUtility.IsValidToken(request.UserID, errorCode);

        //        //#endregion

        //        if (!String.IsNullOrEmpty(errorCode))
        //            response = CommonUtility.GetStandardErrorMessage<ServiceResponseBase>(errorCode);

        //        if (response == null || !response.ErrorFound)
        //        {
        //            response = CarUtility.DeleteCar(request.BeaconID);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response = new ServiceResponseBase()
        //        {
        //            ErrorCode = "ErrAPI01",
        //            ErrorMessage = ex.Message,
        //            ErrorFound = true
        //        };
        //    }
        //    finally
        //    {
        //        log.APICode = 1008;
        //        log.ErrorCode = response.ErrorCode;
        //        log.ErrorFound = response.ErrorFound;
        //        log.APIResponse = response.ErrorMessage;
        //        log.IPAddress = request.IPAddress;
        //        log.ResponseDate = DateTime.Now;
        //        log.Source = request.Source;
        //        log.UserID = request.UserID;
        //        log.UserDeviceID = request.UserDeviceID;

        //        LogUtility.Log(log);
        //    }
        //    return response;
        //}

        [HttpPost]
        public ServiceResponseBase RemoveDevice([FromBody] RemoveCar request)
        {
            json = serialize.Serialize(request);
            LoggingUtility.Log("RemoveDevice--" + json, true);

            APILog log = new APILog() { RequestDate = DateTime.Now };
            ServiceResponseBase response = null;
            try
            {

                string errorCode = String.Empty;
                //#region Authentication
                //if (String.IsNullOrEmpty(errorCode))
                //    errorCode = TokenUtility.IsValidToken(request.UserID, errorCode);

                //#endregion

                if (!String.IsNullOrEmpty(errorCode))
                    response = CommonUtility.GetStandardErrorMessage<ServiceResponseBase>(errorCode);

                if (response == null || !response.ErrorFound)
                {
                    response = CarUtility.DeleteCar(request.BeaconID);
                }
            }
            catch (Exception ex)
            {
                response = new ServiceResponseBase()
                {
                    ErrorCode = "ErrAPI01",
                    ErrorMessage = ex.Message,
                    ErrorFound = true
                };
            }
            finally
            {
                log.APICode = 1008;
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
                LoggingUtility.Log("RemoveDevice--" + json, false);

            }
            return response;
        }
    }
}

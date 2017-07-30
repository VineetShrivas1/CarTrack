using CarTrack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarTrack.DAL
{
    public class CarUtility
    {
        public static ServiceResponseBase AddCar(MyCar car)
        {
            string errorCode = "0";

            using (CarTrackEntities myContext = new CarTrackEntities())
            {

                var user = myContext.UserInformations.Include("MyCars").FirstOrDefault(x => x.UserID == car.UserID);
                if (user == null)
                    errorCode = "Err0008";
                else
                {
                    var inv = myContext.DeviceInventories.Where(x =>
                    
                     x.BeaconID.ToLower() == car.BeaconID.ToLower()
                    ||x.BeaconUDID.ToLower() == car.BeaconUDID.ToLower()).FirstOrDefault();

                    var oldCar = myContext.MyCars.FirstOrDefault(x => 
                    (
                    (!String.IsNullOrEmpty(car.BeaconID) &&x.BeaconID.ToLower() == car.BeaconID.ToLower())
                    ||(!String.IsNullOrEmpty(car.BeaconUDID) && x.BeaconUDID.ToLower() == 
                    car.BeaconUDID.ToLower())) && x.IsDeleted == false);

                    var deletedCar = myContext.MyCars.FirstOrDefault(x => (
                    (!String.IsNullOrEmpty(car.BeaconID) && x.BeaconID.ToLower() == car.BeaconID.ToLower())
                    || (!String.IsNullOrEmpty(car.BeaconUDID) && x.BeaconUDID.ToLower() ==
                    car.BeaconUDID.ToLower())) && x.IsDeleted == true);

                    if (inv == null)
                    {
                        errorCode = "Err0016";//Invalid Beacon ID.
                    }
                    else if (user.MyCars != null && user.MyCars.Count(x => x.IsDeleted == false) >= 5)
                    {
                        errorCode = "Err0020";//Maximum 5 cars are allowed to add in track.
                    }
                    else if (user.MyCars != null && user.MyCars.Count(x => x.IsFavorite == true && x.IsDeleted == false) >= 3)
                    {
                        errorCode = "Err0017";//Maximum 3 cars are allowed to add in favorites.
                    }
                    else if (oldCar != null)
                    {
                        errorCode = "Err0015";//Beacon ID alrealy added.
                    }
                    else if (deletedCar != null)
                    {
                        var defaultSetting = myContext.DefaultSettings.FirstOrDefault();
                        if ((deletedCar.UserID == car.UserID)
                            || (defaultSetting != null && defaultSetting.AddDeviceAfterDays.HasValue
                            && DateTime.Now.Date >= deletedCar.DeletionDate.Value.AddDays(defaultSetting.AddDeviceAfterDays.Value).Date))
                        {
                            deletedCar.IsDeleted = false;
                            deletedCar.DeletionDate = null;
                            deletedCar.UserID = car.UserID;
                            deletedCar.BeaconImage = car.BeaconImage;
                            deletedCar.BeaconUDID = car.BeaconUDID;
                            deletedCar.CarName = car.CarName;
                            deletedCar.IPAddress = car.IPAddress;
                            deletedCar.IsFavorite = car.IsFavorite;
                            deletedCar.UserDeviceID = car.UserDeviceID;
                        }
                        else
                            errorCode = "Err0015";//Beacon ID alrealy added.
                    }
                    else
                    {
                        myContext.MyCars.Add(car);
                    }

                    if (inv != null)
                    {
                        inv.IsRegistered = true;
                        if (deletedCar != null)
                        {
                            deletedCar.BeaconID = inv.BeaconID;
                            deletedCar.BeaconUDID = inv.BeaconUDID;
                            deletedCar.Major = inv.Major;
                            deletedCar.Minor = inv.Minor;
                            deletedCar.ProximityUDID = inv.ProximityUDID;
                        }
                        else if (car != null)
                        {
                            car.IsDeleted = false;
                            car.BeaconID = inv.BeaconID;
                            car.BeaconUDID = inv.BeaconUDID;
                            car.Major = inv.Major;
                            car.Minor = inv.Minor;
                            car.ProximityUDID = inv.ProximityUDID;
                        }
                        myContext.SaveChanges();
                    }
                }
            }
            return CommonUtility.GetStandardErrorMessage<ServiceResponseBase>(errorCode);
        }

        public static List<CarModel> GetDeletedDevices()
        {

            List<CarModel> cars = null;
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                var myCars = myContext.MyCars.Where(x => x.IsDeleted == true).ToList();

                cars = (from c in myCars

                        select new CarModel()
                        {
                            BeaconID = c.BeaconID,
                            BeaconUDID = c.BeaconUDID,
                            UserID = c.UserID,
                            DeletionDate = c.DeletionDate

                        }).ToList();

            }
            return cars;

        }

        public static CarModelResponse MyCars(string userID)
        {

            string errorCode = "0";
            ErrorMessageMaster error = null;
            List<CarModel> cars = null;
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                var myCars = myContext.MyCars.Where(x => x.UserID == userID).ToList();
                if (myCars == null || myCars.Count == 0)
                {
                    errorCode = "Err0018";
                }
                else
                {
                    cars = (from c in myCars

                            select new CarModel()
                            {
                                BeaconID = c.BeaconID,
                                BeaconName = c.CarName,
                                IsFavourite = c.IsFavorite,
                                //BeaconImage = c.BecaonImage.ToString(),
                                UserID = c.UserID

                            }).ToList();
                }
                error = myContext.ErrorMessageMasters.FirstOrDefault(x => x.ErrorCode == errorCode);
            }
            if (error != null)
            {
                return new CarModelResponse()
                {
                    MyCars = cars,
                    ErrorCode = error.ErrorCode,
                    ErrorMessage = error.ErrorMessage,
                    ErrorFound = error.ErrorFound
                };
            }
            return new CarModelResponse()
            {
                ErrorCode = "ErrAPI01",
                ErrorMessage = "Something went wrong.",
                ErrorFound = false
            };
        }

        public static ServiceResponseBase DeleteCar(string beaconID)
        {
            string errorCode = "0";
            ErrorMessageMaster error = null;
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                var car = myContext.MyCars.FirstOrDefault(x => x.BeaconID.ToLower() == beaconID.ToLower()
                || x.BeaconUDID.ToLower() == beaconID.ToLower());
                if (car == null)
                {
                    errorCode = "Err0016";
                }
                else
                {
                    var inv = myContext.DeviceInventories.Where(x => x.BeaconID.ToLower() == car.BeaconID.ToLower()
                        || x.BeaconUDID.ToLower() == car.BeaconUDID.ToLower()).FirstOrDefault();

                    if (inv != null)
                        inv.IsRegistered = false;

                    car.IsDeleted = true;
                    car.DeletionDate = DateTime.Now;

                    //myContext.MyCars.Remove(car);
                    myContext.SaveChanges();
                }
                error = myContext.ErrorMessageMasters.FirstOrDefault(x => x.ErrorCode == errorCode);
            }
            if (error != null)
            {
                return new ServiceResponseBase()
                {
                    ErrorCode = error.ErrorCode,
                    ErrorMessage = error.ErrorMessage,
                    ErrorFound = error.ErrorFound
                };
            }
            return new ServiceResponseBase()
            {
                ErrorCode = "ErrAPI01",
                ErrorMessage = "Something went wrong.",
                ErrorFound = false
            };
        }
    }
}

using CarTrack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarTrack.DAL
{
    public class AccountUtility
    {
        public static SignUpResponse Register(UserInformation register)
        {
            string errorCode = "0";
            ErrorMessageMaster error = null;
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                var user = myContext.UserInformations.FirstOrDefault(x => x.UserID == register.UserID);
                if (user != null)
                {
                    errorCode = "Err0005";
                }
                else if (user == null)
                {
                    myContext.UserInformations.Add(register);
                    myContext.SaveChanges();
                }
                error = CommonUtility.GetStandardErrorMessage(errorCode);
            }
            if (error != null)
            {
                return new SignUpResponse()
                {
                    ErrorCode = error.ErrorCode,
                    ErrorMessage = error.ErrorMessage, //string.Join(",", errors),
                    ErrorFound = error.ErrorFound
                };
            }
            return new SignUpResponse()
            {
                ErrorCode = "ErrAPI01",
                ErrorMessage = "Something went wrong.", //string.Join(",", errors),
                ErrorFound = true
            };
        }

        public static ServiceResponseBase UpdateOTP(OneTimePassword register)
        {
            string errorCode = "0";
            ErrorMessageMaster error = null;
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                var user = myContext.UserInformations.FirstOrDefault(x => x.UserID == register.UserID);
                if (user == null)
                {
                    errorCode = "Err0008";//Invalid User ID.
                }
                var otp = myContext.OneTimePasswords.FirstOrDefault(x => x.UserID == register.UserID);
                if (otp == null)
                {
                    otp = new OneTimePassword();
                    otp.UserID = register.UserID;
                    otp.OTP = register.OTP;
                    otp.IsExpired = false;
                    otp.OTPDate = DateTime.Now;

                    myContext.OneTimePasswords.Add(register);
                    myContext.SaveChanges();
                }
                else
                {
                    otp.OTP = register.OTP;
                    otp.OTPDate = DateTime.Now;
                    otp.IsExpired = false;
                    myContext.SaveChanges();

                }
                error = CommonUtility.GetStandardErrorMessage(errorCode);
            }
            if (error != null)
            {
                return new ServiceResponseBase()
                {
                    ErrorCode = error.ErrorCode,
                    ErrorMessage = error.ErrorMessage, //string.Join(",", errors),
                    ErrorFound = error.ErrorFound
                };
            }
            return new ServiceResponseBase()
            {
                ErrorCode = "ErrAPI01",
                ErrorMessage = "Something went wrong.", //string.Join(",", errors),
                ErrorFound = true
            };

        }

        public static LoginResponse Login(LoginModel loginModel)
        {
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                LoginResponse response = new LoginResponse();
                List<CarModel> mycars = null;
                string errorCode = "0";
                ErrorMessageMaster error = null;
                var user = myContext.UserInformations.FirstOrDefault(x => x.UserID == loginModel.UserID);
                //var user = myContext.UserInformations.FirstOrDefault(x => x.UserID == loginModel.UserID && x.UserPassword == loginModel.UserPassword);

                if (user == null)
                {
                    errorCode = "Err0009";//Invalid User ID/ Password.
                }
                else if (user != null && user.IsLocked.HasValue && user.IsLocked.Value)
                {
                    errorCode = "Err0021";//Your account is locked, Please contact to system administration.
                }
                else if (user != null && user.UserPassword != loginModel.UserPassword)
                {
                    errorCode = "Err0009";//Invalid User ID/ Password.
                    if (user.LoginErrorCount.HasValue)
                        user.LoginErrorCount = user.LoginErrorCount + 1;
                    else
                        user.LoginErrorCount = 1;
                    if (user.LoginErrorCount >= 10)
                        user.IsLocked = true;

                    myContext.SaveChanges();
                }

                else
                {
                    mycars = (from m in user.MyCars.Where(x=>x.IsDeleted==false)
                              select new CarModel()
                              {
                                  BeaconID = m.BeaconID,
                                  BeaconImage = m.BeaconImage,
                                  BeaconName = m.CarName,
                                  IsFavourite = m.IsFavorite,
                                  UserID = m.UserID,
                                  BeaconUDID = m.BeaconUDID

                              }).ToList();
                    if (mycars == null || mycars.Count == 0)
                        mycars = new List<CarModel>();

                    response.MyCars = mycars;
                    response.IsVerified = user.IsVerified;
                    response.IsLocked = user.IsLocked;
                    response.LastLoginDate = user.LastLoginDate;
                    response.EmailAddress = user.EmailAddress;
                    response.FullName = user.FullName;
                    response.MobileNumber = user.MobileNumber;
                    response.UserID = user.UserID;
                    response.AuthToken = user.AuthToken;

                    var otp = CommonUtility.GenerateOTP();

                    if (user.OneTimePassword != null && !String.IsNullOrEmpty(user.OneTimePassword.OTP))
                    {
                        user.OneTimePassword.OTP = otp.OTP;
                        user.OneTimePassword.OTPDate = DateTime.Now;
                        response.OTP = user.OneTimePassword.OTP.Trim();
                    }
                    else
                    {
                        user.OneTimePassword = otp;
                        user.OneTimePassword.UserID = user.UserID;
                    }

                    response.UserSettings = GetUserSetting(user.UserID);

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

        public static Model.UserSetting GetUserSetting(string userID)
        {
            var us = new Model.UserSetting();

            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                var ds = myContext.DefaultSettings.FirstOrDefault();
                var user = myContext.UserInformations.FirstOrDefault(x => x.UserID == userID); //GetUserInfo(userID);
                if (ds != null)
                {
                    us.NotificationDistance = ds.NotificationDistance;
                    us.PurchaseUrl = ds.PurchaseUrl;
                    us.NotificationInterval = ds.NotificationInterval;
                    us.NotificationLanguage = ds.NotificationLanguage;
                    us.AboutUsUrl = ds.AboutUsUrl;
                    us.MinDistance = ds.MinDistance;
                    us.MaxDistance = ds.MaxDistance;
                    us.MinRefreshInterval = ds.MinRefreshInterval;
                    us.MaxRefreshInterval = ds.MaxRefreshInterval;
                }

                if (user.UserSetting != null)
                {
                    us.NotificationInterval = user.UserSetting.NotificationInterval;
                    us.NotificationLanguage = user.UserSetting.NotificationLanguage;
                }
            }
            return us;
        }

        public static UserInformation GetUserInfo(string userID)
        {
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                var user = myContext.UserInformations.FirstOrDefault(x => x.UserID.ToLower() == userID.ToLower());
                user.OneTimePassword = user.OneTimePassword;
                return user;
            }
        }

        public static ServiceResponseBase UpdateUserSetting(Model.UserSetting userSetting)
        {
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                string errorCode = "0";
                ErrorMessageMaster error = null;
                ServiceResponseBase response = new ServiceResponseBase();

                var user = myContext.UserInformations.FirstOrDefault(x => x.UserID == userSetting.UserID);

                if (user == null)
                {
                    errorCode = "Err0008";//Invalid User ID/ Password.
                }
                else
                {
                    var us = new UserSetting();
                    us.NotificationInterval = userSetting.NotificationInterval;
                    us.NotificationLanguage = userSetting.NotificationLanguage;
                    us.UserID = userSetting.UserID;
                    if (user.UserSetting == null)
                    {
                        myContext.UserSettings.Add(us);
                    }
                    else
                        user.UserSetting = us;

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
            return new ServiceResponseBase()
            {
                ErrorCode = "ErrAPI01",
                ErrorMessage = "Something went wrong.", //string.Join(",", errors),
                ErrorFound = true
            };
        }

        public static UserOTP GetOTP(string userID)
        {
            string errorCode = "0";
            ErrorMessageMaster error = null;
            UserOTP otp = new UserOTP();
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                var user = myContext.UserInformations.FirstOrDefault(x => x.UserID == userID);

                if (user == null)
                    errorCode = "Err0008";

                if (user != null)
                {
                    var newOTP = CommonUtility.GenerateOTP();
                    if (user.OneTimePassword == null)
                        user.OneTimePassword = newOTP;
                    else
                    {
                        user.OneTimePassword.OTP = newOTP.OTP;
                        user.OneTimePassword.OTPDate = DateTime.Now;
                    }
                    if (user.OneTimePassword != null)
                    {
                        var onp = user.OneTimePassword;

                        otp.OTP = onp.OTP;
                        otp.UserID = userID;
                        otp.IsExpired = onp.IsExpired;
                        otp.OTPDate = onp.OTPDate;
                    }
                    myContext.SaveChanges();
                }

                error = CommonUtility.GetStandardErrorMessage(errorCode);
                if (error != null)
                {
                    otp.ErrorCode = error.ErrorCode;
                    otp.ErrorFound = error.ErrorFound;
                    otp.ErrorMessage = error.ErrorMessage;
                    return otp;
                }
            }
            return new UserOTP()
            {
                ErrorCode = "ErrAPI01",
                ErrorMessage = "Something went wrong.", //string.Join(",", errors),
                ErrorFound = true
            };
        }

        public static ServiceResponseBase AddFeedback(Feedback feedback)
        {
            string errorCode = "0";
            ErrorMessageMaster error = null;
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                var user = myContext.UserInformations.FirstOrDefault(x => x.UserID == feedback.UserID);
                if (user == null)
                {
                    errorCode = "Err0008";
                }
                else if (user != null)
                {
                    myContext.Feedbacks.Add(feedback);
                    myContext.SaveChanges();
                }
                error = CommonUtility.GetStandardErrorMessage(errorCode);
            }
            if (error != null)
            {
                return new ServiceResponseBase()
                {
                    ErrorCode = error.ErrorCode,
                    ErrorMessage = error.ErrorMessage, //string.Join(",", errors),
                    ErrorFound = error.ErrorFound
                };
            }
            return new ServiceResponseBase()
            {
                ErrorCode = "ErrAPI01",
                ErrorMessage = "Something went wrong.", //string.Join(",", errors),
                ErrorFound = true
            };
        }

        public static ServiceResponseBase VerifyUser(string userID, string OTP)
        {
            string errorCode = "0";
            ErrorMessageMaster error = null;
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                var user = myContext.UserInformations.FirstOrDefault(x => x.UserID == userID);
                if (user == null)
                {
                    errorCode = "Err0008";
                }
                else if (user != null && user.OneTimePassword != null && !String.IsNullOrEmpty(user.OneTimePassword.OTP) && user.OneTimePassword.OTP.Trim() != OTP)
                {
                    errorCode = "Err0008";
                }
                else if (user != null)
                {
                    user.IsVerified = true;
                    if (user.OneTimePassword == null)
                    {

                        var onp = new OneTimePassword();
                        onp.IsExpired = false;
                        onp.OTP = OTP;
                        onp.OTPDate = DateTime.Now;
                        onp.UserID = userID;

                        user.OneTimePassword = onp;

                    }
                    myContext.SaveChanges();
                }
                error = CommonUtility.GetStandardErrorMessage(errorCode);
            }
            if (error != null)
            {
                return new ServiceResponseBase()
                {
                    ErrorCode = error.ErrorCode,
                    ErrorMessage = error.ErrorMessage, //string.Join(",", errors),
                    ErrorFound = error.ErrorFound
                };
            }
            return new ServiceResponseBase()
            {
                ErrorCode = "ErrAPI01",
                ErrorMessage = "Something went wrong.", //string.Join(",", errors),
                ErrorFound = true
            };
        }

        public static void UpdateToken(string key, string authToken)
        {
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                var user = myContext.UserInformations.FirstOrDefault(x => x.UserID == key);
                if (user != null)
                {
                    user.AuthToken = authToken;
                    myContext.SaveChanges();
                }

            }
        }

        public static void ChangePassword(ChangePassword changePassword)
        {
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                var user = myContext.UserInformations.FirstOrDefault(x => x.UserID.ToLower() == changePassword.UserID.ToLower());
                if (user != null)
                {
                    user.UserPassword = changePassword.NewPassword;
                    myContext.SaveChanges();
                }

            }
        }
    }
}

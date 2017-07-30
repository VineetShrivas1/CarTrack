
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
using System.Web.Script.Serialization;

namespace CarTrackAPI.Controllers
{
    public class AccountController : ApiController
    {
        JavaScriptSerializer serialize = new JavaScriptSerializer();
        string json = string.Empty;
        public SignUpResponse Register([FromBody] RegisterModel request)
        {
            json = serialize.Serialize(request);
            LoggingUtility.Log(json, true);
            APILog log = new APILog() { RequestDate = DateTime.Now };
            //request.RequestDate = DateTime.Now;
            SignUpResponse response = null;
            try
            {

                #region Validation

                if (!ModelState.IsValid)
                {
                    ErrorMessageMaster error = null;
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
                        error = CommonUtility.GetStandardErrorMessage(errors.FirstOrDefault());
                        if (error != null)
                        {
                            response = new SignUpResponse()
                            {
                                ErrorCode = error.ErrorCode,
                                ErrorMessage = error.ErrorMessage, //string.Join(",", errors),
                                ErrorFound = error.ErrorFound
                            };

                        }
                        else
                        {
                            response = new SignUpResponse()
                            {
                                ErrorCode = "ErrAPI01",
                                ErrorMessage = "Something went wrong.", //string.Join(",", errors),
                                ErrorFound = true
                            };
                        }
                    }

                }

                #endregion
                if (response == null || !response.ErrorFound)
                {
                    string outMsg = String.Empty;

                    request.UserPassword = CryptoUtility.Encrypt(Constants.Key, request.UserPassword, out outMsg);

                    if (String.IsNullOrEmpty(outMsg))
                    {
                        UserInformation user = new UserInformation();
                        user.EmailAddress = request.UserID;
                        user.FullName = request.FullName;
                        user.MobileNumber = request.MobileNumber;
                        user.UserID = request.UserID;
                        user.UserPassword = request.UserPassword;
                        user.IsVerified = false;
                        user.IsLocked = false;
                        user.UserDeviceID = request.UserDeviceID;
                        user.IPAddress = request.IPAddress;
                        var otp = CommonUtility.GenerateOTP();
                        user.OneTimePassword = otp;

                        response = AccountUtility.Register(user);

                        if (response != null && response.ErrorCode == "0")
                        {
                            response.MyCars = new List<CarModel>();

                            if (string.IsNullOrEmpty(response.AuthToken))
                            {

                                response.AuthToken = TokenUtility.GenerateToken(request.UserID);

                            }
                            response.AuthToken = TokenUtility.GenerateToken(request.UserID);
                            response.EmailAddress = request.UserID;
                            response.FullName = request.FullName;
                            response.IsLocked = false;
                            response.IsVerified = false;
                            response.MobileNumber = request.MobileNumber;
                            response.OTP = otp.OTP;
                            response.UserID = request.UserID;
                            response.UserSettings = AccountUtility.GetUserSetting(request.UserID);

                            #region Email

                            string subject = "Pass Code";
                            string body = "Your Pass Code is :" + otp.OTP;

                            var template = EmaiUtility.GetEmailTemplate("Registration");

                            if (template != null)
                            {
                                subject = template.Subject;
                                body = template.TemplateBody.Replace("<PassCode>", otp.OTP).Replace("<USER>", user.FullName);
                            }

                            EmaiUtility.SendEmail(user.EmailAddress, subject, body);
                            #endregion
                        }

                    }
                    else
                    {
                        response = new SignUpResponse()
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
                response = new SignUpResponse()
                {
                    ErrorCode = "ErrAPI01",
                    ErrorMessage = ex.Message,
                    ErrorFound = true
                };
            }
            finally
            {
                log.APICode = 1000;
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
                LoggingUtility.Log(json, false);

            }
            return response;

        }

        public LoginResponse Login([FromBody] LoginModel request)
        {
            json = serialize.Serialize(request);
            LoggingUtility.Log("Login--"+json, true);

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
                        response = AccountUtility.Login(request);
                        if (response != null && response.ErrorCode == "0")
                        {
                            
                            if (string.IsNullOrEmpty(response.AuthToken))
                            {
                                response.AuthToken = TokenUtility.GenerateToken(request.UserID);
                            }
                            if (!response.IsVerified.HasValue || !response.IsVerified.Value)
                            {
                                #region Email

                                string subject = "Pass Code";
                                string body = "Your Pass Code is :" + response.OTP;

                                var template = EmaiUtility.GetEmailTemplate("Registration");

                                if (template != null)
                                {
                                    subject = template.Subject;
                                    body = template.TemplateBody.Replace("<PassCode>", response.OTP).Replace("<USER>", response.FullName);
                                }


                                EmaiUtility.SendEmail(request.UserID, subject, body);

                                #endregion
                            }
                        }
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
                log.APICode = 1001;
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
                LoggingUtility.Log("Login--" + json, false);

            }
            return response;

        }

        public ServiceResponseBase ForgetPassword([FromBody]ServiceRequestBase request)
        {
            json = serialize.Serialize(request);
            LoggingUtility.Log("ForgetPassword--" + json, true);

            APILog log = new APILog() { RequestDate = DateTime.Now };
            ServiceResponseBase response = null;
            try
            {
                string errorCode = "0";

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
                        response = CommonUtility.GetStandardErrorMessage<ServiceResponseBase>(errors.FirstOrDefault());

                    }

                }

                #endregion
                if (response == null || !response.ErrorFound)
                {

                    UserOTP userOTP = AccountUtility.GetOTP(request.UserID);
                    if (userOTP != null)
                    {
                        if (userOTP.ErrorCode == "0")
                        {
                            string subject = "PassCode";
                            string body = "Your Pass Code is :" + userOTP.OTP;
                            var template = EmaiUtility.GetEmailTemplate("ResetPassword");
                            if (template != null)
                            {
                                subject = template.Subject;
                                body = template.TemplateBody.Replace("<PassCode>", userOTP.OTP);
                            }
                            EmaiUtility.SendEmail(request.UserID, subject, body);
                        }
                        else
                            errorCode = userOTP.ErrorCode;

                        response = CommonUtility.GetStandardErrorMessage<ServiceResponseBase>(errorCode);
                    }
                    else
                    {
                        response = new ServiceResponseBase()
                        {
                            ErrorCode = "ErrAPI01",
                            ErrorMessage = "something went wrong.",
                            ErrorFound = true
                        };
                    }


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
                log.APICode = 1002;
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
                LoggingUtility.Log("ForgetPassword--" + json, false);

            }
            return response;

        }

        public ServiceResponseBase UpdateUserSetting([FromBody]CarTrack.Model.UserSetting request)
        {
            json = serialize.Serialize(request);
            LoggingUtility.Log("UpdateUserSetting--" + json, true);

            APILog log = new APILog() { RequestDate = DateTime.Now };
            ServiceResponseBase response = null;
            string errorCode = String.Empty;

            try
            {
                #region Authentication
                if (String.IsNullOrEmpty(errorCode))
                    errorCode = TokenUtility.IsValidToken(request.UserID, errorCode);

                #endregion

                if (!String.IsNullOrEmpty(errorCode))
                    response = CommonUtility.GetStandardErrorMessage<ServiceResponseBase>(errorCode);
                if (response == null || !response.ErrorFound)
                    response = AccountUtility.UpdateUserSetting(request);
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
                log.APICode = 1003;
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
                LoggingUtility.Log("UpdateUserSetting--" + json, false);

            }
            return response;
        }

        public UserOTP ResendOTP([FromBody]ServiceRequestBase request)
        {
            json = serialize.Serialize(request);
            LoggingUtility.Log("ResendOTP--" + json, true);

            APILog log = new APILog() { RequestDate = DateTime.Now };
            UserOTP response = null;
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
                        response = CommonUtility.GetStandardErrorMessage<UserOTP>(errors.FirstOrDefault());
                    }
                }

                #endregion
                if (response == null || !response.ErrorFound)
                {
                    response = AccountUtility.GetOTP(request.UserID);
                    if (response != null && response.ErrorCode == "0")
                    {
                        #region Email

                        string subject = "Pass Code";
                        string body = "Your Pass Code is :" + response.OTP;

                        var template = EmaiUtility.GetEmailTemplate("Registration");

                        if (template != null)
                        {
                            subject = template.Subject;
                            body = template.TemplateBody.Replace("<PassCode>", response.OTP);
                                //.Replace("<USER>", response.FullName);
                        }


                        EmaiUtility.SendEmail(request.UserID, subject, body);

                        #endregion
                        //EmaiUtility.SendEmail(request.UserID, "OTP", "Your OTP is :" + response.OTP);
                    }
                }
            }
            catch (Exception ex)
            {
                response = new UserOTP()
                {
                    ErrorCode = "ErrAPI01",
                    ErrorMessage = ex.Message,
                    ErrorFound = true
                };
            }
            finally
            {
                log.APICode = 1004;
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
                LoggingUtility.Log("ResendOTP--" + json, false);

            }
            return response;
        }

        public ServiceResponseBase AddFeedback([FromBody]UserFeedback request)
        {
            json = serialize.Serialize(request);
            LoggingUtility.Log("AddFeedback--" + json, true);

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
                    if (request != null)
                    {
                        Feedback f = new Feedback();
                        f.FeedbackDate = DateTime.Now;
                        f.Rating = request.Rating.Value;
                        f.RatingText = request.RatingText;
                        f.UserID = request.UserID;
                        f.UserFeedback = request.FeedbackText;

                        response = AccountUtility.AddFeedback(f);
                        //if (response != null && response.ErrorCode == "0")
                        //{
                        //    var user = AccountUtility.GetUserInfo(request.UserID);
                        //    string subject = "Feedback";
                        //    string body = "Hi Admin,<br/><br/>Following feedback has been sent by <USER>.<br/><br/>\"<FEEDBACK>\"<br/><br/><b>Best Regards,</b><br/><b><USER><b>";
                        //    var template = EmaiUtility.GetEmailTemplate("Feedback");

                        //    body = body.Replace("<USER>", user.FullName).Replace("<FEEDBACK>", request.FeedbackText);
                        //    if (template != null)
                        //    {
                        //        subject = template.Subject;
                        //        body = template.TemplateBody.Replace("<USER>", user.FullName).Replace("<FEEDBACK>", request.FeedbackText);
                        //    }
                        //    EmaiUtility.SendEmail(request.UserID, subject, body);
                        //}

                    }
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
                log.APICode = 1005;
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
                LoggingUtility.Log("AddFeedback--" + json, false);

            }
            return response;
        }

        public ServiceResponseBase VerifyUser([FromBody]VerifyUser request)
        {
            json = serialize.Serialize(request);
            LoggingUtility.Log("VerifyUser--" + json, true);

            APILog log = new APILog() { RequestDate = DateTime.Now };
            ServiceResponseBase response = null;
            try
            {
                response = AccountUtility.VerifyUser(request.UserID, request.OTP);
                if (response != null && response.ErrorCode == "0")
                {
                    var user = AccountUtility.GetUserInfo(request.UserID);
                    string outMsg = String.Empty;
                    var password = CryptoUtility.Decrypt(Constants.Key, user.UserPassword, out outMsg);
                    string subject = "Welcome";
                    string body = "Hi <USER>,<br/><br/>You are successfully registerd.<br/><br/><b>Best Regards,</b><br/><b>The Tracker Team<b>";
                    var template = EmaiUtility.GetEmailTemplate("Welcome");
                    if (template != null)
                    {
                        int len = password.Length;
                        string star = string.Empty;
                        for (int l = 0; l < len - 3; l++)
                        {
                            star += "*";
                        }
                        string mask = password.Substring(0, 2) + star + password.Substring(password.Length - 1);
                        subject = template.Subject;
                        body = template.TemplateBody.Replace("<USER>", user.FullName).Replace("<UserID>", user.EmailAddress)
                            .Replace("<Password>", mask);
                    }
                    EmaiUtility.SendEmail(request.UserID, subject, body);
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
                log.APICode = 1006;
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
                LoggingUtility.Log("VerifyUser--" + json, false);

            }
            return response;

        }

        public ServiceResponseBase ChangePassword([FromBody] ChangePassword request)
        {
            //string json=JavaScriptSerializer
            json = serialize.Serialize(request);
            LoggingUtility.Log("ChangePassword--" + json, true);

            APILog log = new APILog() { RequestDate = DateTime.Now };
            ServiceResponseBase response = null;
            string errorCode = "0";
            try
            {
                UserInformation user = AccountUtility.GetUserInfo(request.UserID);
                if (user != null)
                {
                    if (user.OneTimePassword != null && user.OneTimePassword.OTP.Trim() == request.OTP)
                    {
                        string outMsg = String.Empty;
                        var password = CryptoUtility.Encrypt(Constants.Key, request.NewPassword, out outMsg);
                        if (String.IsNullOrEmpty(outMsg))
                        {
                            request.NewPassword = password;
                            AccountUtility.ChangePassword(request);
                        }
                    }
                    else
                    {
                        errorCode = "Err0026";
                    }

                }
                else
                {
                    errorCode = "Err0008";
                }
                response = CommonUtility.GetStandardErrorMessage<ServiceResponseBase>(errorCode);
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
                log.APICode = 1006;
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
                LoggingUtility.Log("ChangePassword--" + json, false);

            }
            return response;
        }

        //[HttpPost]
        //public ServiceResponseBase ValidateToken([FromBody]string token)
        //{
        //    if (TokenUtility.ValidateToken(token))
        //        return new ServiceResponseBase()
        //        {
        //            ErrorMessage = "successful."
        //        };

        //    return new ServiceResponseBase()
        //    {
        //        ErrorMessage = "failed."
        //    };
        //}
    }
}

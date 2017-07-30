using CarTrack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarTrack.DAL
{
    public class CommonUtility
    {
        public static T GetStandardErrorMessage<T>(string errorCode) where T : ServiceResponseBase, new()
        {
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                var error = myContext.ErrorMessageMasters.Where(x => x.ErrorCode == errorCode).FirstOrDefault();

                T t = new T();
                if (error != null)
                {
                    t.ErrorCode = error.ErrorCode;
                    t.ErrorFound = error.ErrorFound;
                    t.ErrorMessage = error.ErrorMessage;
                }
                else
                {
                    t.ErrorCode = "ErrAPI01";
                    t.ErrorFound = true;
                    t.ErrorMessage = "Something went wrong.";
                }
                return t;
            }
        }
        public static ErrorMessageMaster GetStandardErrorMessage(string errorCode)
        {
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                return myContext.ErrorMessageMasters.Where(x => x.ErrorCode == errorCode).FirstOrDefault();

            }
        }

        public static OneTimePassword GenerateOTP()
        {
            var otp = new Random().Next(100000, 999999).ToString();
            OneTimePassword onp = new OneTimePassword();
            onp.OTP = otp;
            onp.IsExpired = false;
            onp.OTPDate = DateTime.Now;

            return onp;
        }
    }
}

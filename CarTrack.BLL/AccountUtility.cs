using CarTrack.DLL;
using CarTrack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace CarTrack.BLL
{

    public static class AccountUtility
    {
        public static ServiceResponseBase Register(RegisterModel register)
        {
            return CarTrack.DLL.AccountUtility.Register(register);
        }
    }
}

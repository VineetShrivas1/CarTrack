//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CarTrack.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class APIMaster
    {
        public int APICode { get; set; }
        public string APIName { get; set; }
        public string RequestParam { get; set; }
        public string ResponseParam { get; set; }
        public string MethodType { get; set; }
        public Nullable<bool> IsAuthTokenRequired { get; set; }
        public string ApiURL { get; set; }
    }
}
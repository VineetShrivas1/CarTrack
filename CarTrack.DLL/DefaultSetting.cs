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
    
    public partial class DefaultSetting
    {
        public int SettingID { get; set; }
        public string PurchaseUrl { get; set; }
        public int NotificationInterval { get; set; }
        public string NotificationLanguage { get; set; }
        public decimal NotificationDistance { get; set; }
        public string AboutUsUrl { get; set; }
        public Nullable<decimal> MinDistance { get; set; }
        public Nullable<decimal> MaxDistance { get; set; }
        public Nullable<decimal> MinRefreshInterval { get; set; }
        public Nullable<decimal> MaxRefreshInterval { get; set; }
        public Nullable<int> AddDeviceAfterDays { get; set; }
    }
}

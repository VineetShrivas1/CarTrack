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
    
    public partial class MyCar
    {
        public string UserID { get; set; }
        public string BeaconID { get; set; }
        public string CarName { get; set; }
        public Nullable<bool> IsFavorite { get; set; }
        public string BeaconImage { get; set; }
        public string IPAddress { get; set; }
        public string UserDeviceID { get; set; }
        public Nullable<bool> DeviceStatus { get; set; }
        public Nullable<System.DateTime> RegistrationDate { get; set; }
        public string BeaconUDID { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletionDate { get; set; }
        public Nullable<int> Major { get; set; }
        public Nullable<int> Minor { get; set; }
        public string ProximityUDID { get; set; }
    
        public virtual UserInformation UserInformation { get; set; }
    }
}

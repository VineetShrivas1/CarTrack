using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarTrack.Model
{
    public class CarModel : ServiceRequestBase
    {
        //[Required(ErrorMessage = "Beacon ID can not be left blank.")]
        //[MaxLength(100)]
        public string BeaconID { get; set; }

        [Required(ErrorMessage = "Beacon Name can not be left blank.")]
        [MaxLength(100)]
        public string BeaconName { get; set; }

        //[MaxLength(1000)]
        public string BeaconImage { get; set; }

        public bool? IsFavourite { get; set; }

        public string BeaconUDID { get; set; }
    }
    public class CarModelResponse : ServiceResponseBase
    {
        public List<CarModel> MyCars { get; set; }
    }
    public class RemoveCar : ServiceRequestBase
    {
        public string BeaconID { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarTrack.DAL
{
   public class Constants
    {
       public static System.Data.Objects.ObjectParameter ErrorCode
       {

           get { return new System.Data.Objects.ObjectParameter("ErrorCode", typeof(string)); }
       }
       public static System.Data.Objects.ObjectParameter ErrorMessage
       {

           get { return new System.Data.Objects.ObjectParameter("ErrorMessage", typeof(string)); }
       }
       public static System.Data.Objects.ObjectParameter ErrorFound
       {

           get { return new System.Data.Objects.ObjectParameter("ErrorFound", typeof(bool)); }
       }
    }
}

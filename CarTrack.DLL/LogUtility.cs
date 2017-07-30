using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarTrack.DAL
{
    public class LogUtility
    {
        public static void Log(APILog log)
        {
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                myContext.APILogs.Add(log);
                myContext.SaveChanges();
            }
        }
    }
}

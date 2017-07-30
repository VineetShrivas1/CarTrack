using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarTrack.DAL
{
    public static class ContextUtility
    {
        public static CarTrackEntities GetContext()
        {
            return new CarTrackEntities();
        }
    }
}

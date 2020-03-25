using System;
using System.Collections.Generic;
using System.Text;

namespace lskysd.techinventory.lib
{
    public class DeviceFacility
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public int FacilityId { get; set; }
        public Facility Facility { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace lskysd.techinventory
{
    public class DeviceFacility
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public Facility Facility { get; set; }
    }
}

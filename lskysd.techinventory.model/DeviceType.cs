using System;
using System.Collections.Generic;
using System.Text;

namespace lskysd.techinventory
{
    public class DeviceType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        public static DeviceType Unknown = new DeviceType()
        {
            Id = 0,
            Name = "Unknown"
        };
    }
}

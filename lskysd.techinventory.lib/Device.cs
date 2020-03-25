using System;
using System.Collections.Generic;
using System.Text;

namespace lskysd.techinventory.lib
{
    public class Device
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public string Notes { get; set; }
        public int PurchaseYear { get; set; }
        public DateTime PurchaseDate { get; set; }
        public bool IsActive { get; set; }

        public List<DeviceName> Names { get; set; }
        public List<DeviceMACAddress> MACAddresses { get; set; }
        public List<DeviceFacility> Facilities { get; set; }                
    }
}

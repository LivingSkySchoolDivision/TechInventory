using System;
using System.Collections.Generic;
using System.Text;

namespace lskysd.techinventory
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
        public DeviceType DeviceType { get; set; }



        public bool NeedsUpdate(Device thisDevice)
        {
            if (this.Id == thisDevice.Id)
            {
                if(
                    (this.SerialNumber != thisDevice.SerialNumber) ||
                    (this.Model != thisDevice.Model)  ||
                    (this.Notes != thisDevice.Notes) ||
                    (this.PurchaseDate != thisDevice.PurchaseDate) ||
                    (this.PurchaseYear != this.PurchaseYear) ||
                    (this.IsActive != thisDevice.IsActive) ||
                    (this.DeviceType.Id != this.DeviceType.Id)
                    )
                {
                    return true;
                }
            } 

            return false;
        }
    }
}

using lskysd.techinventory.util;
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



        public bool NeedsUpdate(Device comparedDevice)
        {
            
            if(
                (this.SerialNumber != comparedDevice.SerialNumber) ||
                (this.Model != comparedDevice.Model)  ||
                (
                    (this.Notes != comparedDevice.Notes) && 
                    (!string.IsNullOrEmpty(this.Notes) && !string.IsNullOrEmpty(comparedDevice.Notes))
                ) ||
                (
                    (this.PurchaseDate != comparedDevice.PurchaseDate) && 
                    (comparedDevice.PurchaseDate > Parsers.dbMinDate)
                ) ||
                (this.PurchaseYear != comparedDevice.PurchaseYear) ||
                (this.IsActive != comparedDevice.IsActive) ||
                (this.DeviceType.Id != comparedDevice.DeviceType.Id)
                )
            {
                return true;
            }
            
            return false;
        }
    }
}

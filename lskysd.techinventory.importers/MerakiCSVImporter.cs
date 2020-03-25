using CsvHelper;
using lskysd.techinventory.db;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace lskysd.techinventory.importers
{
    public class MerakiCSVImporter
    {
        private DeviceTypeIdentifier _deviceTypeIdentifier;
        private string _connstring = string.Empty;

        public MerakiCSVImporter(string connectionString)
        {
            this._connstring = connectionString;
            this._deviceTypeIdentifier = new DeviceTypeIdentifier(connectionString);
        }

        public void Import(StreamReader csvData, Facility Facility)
        {
            List<Device> importedDevices = new List<Device>();

            Console.WriteLine("ConnStr: " + this._connstring);

            using (CsvReader csv = new CsvReader(csvData, CultureInfo.InvariantCulture))
            {
                var fileFormatDefinition = new
                {
                    Name = string.Empty,
                    Model = string.Empty,
                    Serial = string.Empty
                };

                var records = csv.GetRecords(fileFormatDefinition);

                foreach (var o in records)
                {
                    if (!string.IsNullOrEmpty(o.Serial))
                    {
                        importedDevices.Add(new Device()
                        {
                            SerialNumber = o.Serial,
                            Model = o.Model,
                            DeviceType = _deviceTypeIdentifier.IdentifyByModel(o.Model)
                        });
                    }
                }
            }

            // Insert or update devices
            Console.WriteLine("Devices to insert or update: " + importedDevices.Count);

            DeviceRepository deviceRepo = new DeviceRepository(this._connstring);
            deviceRepo.AddOrUpdate(importedDevices);

            // Insert or update device facilities
            // Once for every device

            // Insert or update device names

            // Insert or update device MAC addresses
            //  Meraki doesn't export MAC addresses :(

        }
    }
}

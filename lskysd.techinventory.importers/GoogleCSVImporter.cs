using CsvHelper;
using lskysd.techinventory.db;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace lskysd.techinventory.importers
{
    public class GoogleCSVImporter
    {
        private readonly string _connstring = string.Empty;

        public GoogleCSVImporter(string connectionString)
        {
            this._connstring = connectionString;
        }

        public void Import(StreamReader csvData)
        {
            List<Device> parsedDevices = new List<Device>();
            Dictionary<string, string> detectedNamesBySerial = new Dictionary<string, string>();
            Dictionary<string, string> detectedFacilitiesBySerial = new Dictionary<string, string>();

            using (CsvReader csv = new CsvReader(csvData, CultureInfo.InvariantCulture))
            {
                var fileFormatDefinition = new
                {
                    serialNumber = string.Empty,
                    model = string.Empty,
                    annotatedLocation = string.Empty,
                    annotatedAssetId = string.Empty,
                    ethernetMacAddress = string.Empty,
                    macAddress = string.Empty
                };


                var records = csv.GetRecords(fileFormatDefinition);

                foreach (var o in records)
                {
                    if (!string.IsNullOrEmpty(o.serialNumber))
                    {
                        // Extract the device records
                        parsedDevices.Add(new Device()
                        {
                            SerialNumber = o.serialNumber,
                            Model = o.model
                        });

                        // Extract device names
                        if (!detectedNamesBySerial.ContainsKey(o.annotatedAssetId))
                        {
                            detectedNamesBySerial.Add(o.serialNumber, o.annotatedAssetId);
                        }

                        // Extract the facility records
                        if (!detectedFacilitiesBySerial.ContainsKey(o.annotatedLocation))
                        {
                            detectedFacilitiesBySerial.Add(o.serialNumber, o.annotatedLocation);
                        }

                        // Extract the MAC addresses

                    }
                    
                }
            }
            
            ImportHandler importHandler = new ImportHandler(_connstring);
            importHandler.Import(
                parsedDevices,
                detectedNamesBySerial,
                detectedFacilitiesBySerial
                );
        }



    }
}

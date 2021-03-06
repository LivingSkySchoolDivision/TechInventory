﻿using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace lskysd.techinventory.importers
{
    public class MerakiCSVImporter
    {
        private readonly string _connstring = string.Empty;

        public MerakiCSVImporter(string connectionString)
        {
            this._connstring = connectionString;
        }

        public void Import(StreamReader csvData, Facility Facility)
        {
            Import(csvData, Facility.Name);
        }

        public void Import(StreamReader csvData, string FacilityName)
        {
            List<Device> parsedDevices = new List<Device>();
            Dictionary<string, string> detectedNamesBySerial = new Dictionary<string, string>();
            Dictionary<string, string> detectedFacilitiesBySerial = new Dictionary<string, string>();

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
                        // Extract the device records
                        parsedDevices.Add(new Device()
                        {
                            SerialNumber = o.Serial,
                            Model = o.Model
                        });

                        // Extract device names
                        if (!detectedNamesBySerial.ContainsKey(o.Serial))
                        {
                            detectedNamesBySerial.Add(o.Serial, o.Name);
                        }

                        // Extract the facility records
                        if (!detectedFacilitiesBySerial.ContainsKey(o.Serial))
                        {
                            detectedFacilitiesBySerial.Add(o.Serial, FacilityName);
                        }
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

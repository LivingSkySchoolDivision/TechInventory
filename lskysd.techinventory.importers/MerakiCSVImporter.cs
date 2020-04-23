using CsvHelper;
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
            Dictionary<string, List<string>> detectedNamesBySerial = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> detectedFacilitiesBySerial = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> detectedMACsBySerial = new Dictionary<string, List<string>>();

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
                        if (!string.IsNullOrEmpty(o.Name))
                        {
                            if (!detectedNamesBySerial.ContainsKey(o.Serial))
                            {
                                detectedNamesBySerial.Add(o.Serial, new List<string>());
                            }
                            if (!detectedNamesBySerial[o.Serial].Contains(o.Name))
                            {
                                detectedNamesBySerial[o.Serial].Add(o.Name);
                            }

                            // Extract the facility records (all the same for Meraki, because 1 CSV = 1 location
                            if (!detectedFacilitiesBySerial.ContainsKey(o.Serial))
                            {
                                detectedFacilitiesBySerial.Add(o.Serial, new List<string>() { FacilityName });
                            }
                        }
                        
                    }
                }
            }

            ImportHandler importHandler = new ImportHandler(_connstring);
            importHandler.Import(
                parsedDevices,
                detectedNamesBySerial,
                detectedFacilitiesBySerial,
                detectedMACsBySerial
                );

        }
    }
}

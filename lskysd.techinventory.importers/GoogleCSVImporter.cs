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

        private string parseOrgUnitPath(string orgUnitPath)
        {
            if (string.IsNullOrEmpty(orgUnitPath))
            {
                return string.Empty;
            }

            // If the first character is a '/', remove it
            // Grab up until the next '/', so we only get the first section
            StringBuilder returnMe = new StringBuilder(orgUnitPath);
            
            if (returnMe[0] == '/')
            {
                returnMe.Remove(0, 1);
            }

            return returnMe.ToString();
        }

        public void Import(StreamReader csvData)
        {
            List<Device> parsedDevices = new List<Device>();

            // the first string for all of these needs to be the serial no of the device
            Dictionary<string, List<string>> detectedNamesBySerial = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> detectedFacilitiesBySerial = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> detectedMACsBySerial = new Dictionary<string, List<string>>();

            using (CsvReader csv = new CsvReader(csvData, CultureInfo.InvariantCulture))
            {
                var fileFormatDefinition = new
                {
                    serialNumber = string.Empty,
                    model = string.Empty,
                    orgUnitPath = string.Empty,
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
                        if (!string.IsNullOrEmpty(o.annotatedAssetId)) 
                        {
                            if (!detectedNamesBySerial.ContainsKey(o.serialNumber))
                            {
                                detectedNamesBySerial.Add(o.serialNumber, new List<string>());
                            }

                            if (!detectedNamesBySerial[o.serialNumber].Contains(o.annotatedAssetId))
                            {
                                detectedNamesBySerial[o.serialNumber].Add(o.annotatedAssetId);
                            }
                        }

                        // Extract the facility records
                        string thisDeviceFacility = parseOrgUnitPath(o.orgUnitPath);
                        if (!string.IsNullOrEmpty(thisDeviceFacility))
                        {
                            if (!detectedFacilitiesBySerial.ContainsKey(o.serialNumber))
                            {
                                detectedFacilitiesBySerial.Add(o.serialNumber, new List<string>());
                            }

                            if (!detectedFacilitiesBySerial[o.serialNumber].Contains(thisDeviceFacility))
                            {
                                detectedFacilitiesBySerial[o.serialNumber].Add(thisDeviceFacility);

                            }
                        }

                        // Extract the MAC addresses
                        if (!string.IsNullOrEmpty(o.macAddress))
                        {
                            if (!detectedMACsBySerial.ContainsKey(o.serialNumber))
                            { 
                                detectedMACsBySerial.Add(o.serialNumber, new List<string>());
                            }

                            if (!detectedMACsBySerial[o.serialNumber].Contains(o.macAddress))
                            {
                                detectedMACsBySerial[o.serialNumber].Add(o.macAddress);
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

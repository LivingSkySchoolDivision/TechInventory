using lskysd.techinventory.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lskysd.techinventory.importers
{
    class ImportHandler
    {
        private DeviceTypeIdentifier _deviceTypeIdentifier;
        private readonly string _connstring = string.Empty;

        public ImportHandler(string ConnectionString)
        {
            this._connstring = ConnectionString;
            this._deviceTypeIdentifier = new DeviceTypeIdentifier(ConnectionString);
        }

        public void Import(
            List<Device> Devices,
            Dictionary<string, List<string>> DeviceNamesBySerialNo, // Serial, Name
            Dictionary<string, List<string>> DeviceFacilitiesBySerialNo, // Serial, Facility
            Dictionary<string, List<string>> DeviceMACsByserialNo // Serial, MAC
            )
        {
            List<DeviceName> deviceNames = new List<DeviceName>();
            List<DeviceFacility> deviceFacilities = new List<DeviceFacility>();
            List<DeviceMACAddress> deviceMACs = new List<DeviceMACAddress>();

            Console.WriteLine("Devices to insert or update: " + Devices.Count);

            // First, determine device types
            foreach (Device device in Devices)
            {
                device.DeviceType = this._deviceTypeIdentifier.IdentifyByModel(device.Model);
            }

            // Insert or update devices
            Console.WriteLine(" Updating devices...");
            DeviceRepository deviceRepo = new DeviceRepository(this._connstring);
            deviceRepo.AddOrUpdate(Devices);

            // Get a list of all devices, because we'll need it to find ID numbers
            Dictionary<string, Device> allDevicesBySerial = deviceRepo.GetAll().ToDictionary(x => x.SerialNumber);
            

            // Insert or update device facilities
            Console.WriteLine(" Updating device facilities...");
            FacilityRepository facilityRepo = new FacilityRepository(this._connstring);

            foreach(KeyValuePair<string, List<string>> df in DeviceFacilitiesBySerialNo)
            {
                if (allDevicesBySerial.ContainsKey(df.Key)) 
                {
                    foreach (string val in df.Value)
                    {
                        Facility f = facilityRepo.GetByName(val);
                        deviceFacilities.Add(new DeviceFacility()
                        {
                            DeviceId = allDevicesBySerial[df.Key].Id,
                            Facility = f
                        }); ;
                    }
                }
            }
            DeviceFacilityRepository deviceFacilityrepo = new DeviceFacilityRepository(this._connstring, facilityRepo);
            deviceFacilityrepo.Add(deviceFacilities);


            // Insert or update device names
            Console.WriteLine(" Updating device names...");
            foreach (KeyValuePair<string, List<string>> deviceName in DeviceNamesBySerialNo)
            {
                if (allDevicesBySerial.ContainsKey(deviceName.Key)) 
                {
                    foreach (string val in deviceName.Value)
                    {
                        deviceNames.Add(new DeviceName()
                        {
                            DeviceId = allDevicesBySerial[deviceName.Key].Id,
                            Value = val
                        });
                    }
                }
            }
            DeviceNameRepository deviceNameRepo = new DeviceNameRepository(this._connstring);
            deviceNameRepo.Add(deviceNames);


            // Insert or update device MAC addresses
            Console.WriteLine(" Updating MAC Addresses...");
            foreach(KeyValuePair<string, List<string>> deviceMAC in DeviceMACsByserialNo)
            {
                if (allDevicesBySerial.ContainsKey(deviceMAC.Key)) 
                {
                    foreach (string val in deviceMAC.Value)
                    {
                        deviceMACs.Add(new DeviceMACAddress()
                        {
                            DeviceId = allDevicesBySerial[deviceMAC.Key].Id,
                            MACAddress = val
                        });
                    }
                }
            }
            DeviceMACRepository macRepo = new DeviceMACRepository(this._connstring);
            macRepo.Add(deviceMACs);

        }
    }
}

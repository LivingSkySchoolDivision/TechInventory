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
            Dictionary<string, string> DeviceNamesBySerialNo,
            Dictionary<string, string> DeviceFacilitiesBySerialNo
            )
        {
            List<DeviceName> deviceNames = new List<DeviceName>();
            List<DeviceFacility> deviceFacilities = new List<DeviceFacility>();

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

            foreach(KeyValuePair<string, string> df in DeviceFacilitiesBySerialNo)
            {
                if (allDevicesBySerial.ContainsKey(df.Key))
                {
                    Facility f = facilityRepo.GetByName(df.Value);
                    if (f.Id == 0) { Console.WriteLine("Unknown facility: " + df.Value); }
                    deviceFacilities.Add(new DeviceFacility()
                    {
                        DeviceId = allDevicesBySerial[df.Key].Id,
                        Facility = f
                    }); ;
                }
            }
            DeviceFacilityRepository deviceFacilityrepo = new DeviceFacilityRepository(this._connstring, facilityRepo);
            deviceFacilityrepo.Add(deviceFacilities);


            // Insert or update device names
            Console.WriteLine(" Updating device names...");
            foreach (KeyValuePair<string, string> deviceName in DeviceNamesBySerialNo)
            {
                if (allDevicesBySerial.ContainsKey(deviceName.Key))
                {
                    deviceNames.Add(new DeviceName()
                    {
                        DeviceId = allDevicesBySerial[deviceName.Key].Id,
                        Value = deviceName.Value
                    });
                }
            }
            DeviceNameRepository deviceNameRepo = new DeviceNameRepository(this._connstring);
            deviceNameRepo.Add(deviceNames);


            // Insert or update device MAC addresses

        }
    }
}

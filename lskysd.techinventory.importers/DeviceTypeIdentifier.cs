using lskysd.techinventory.db;
using System;
using System.Collections.Generic;
using System.Text;

namespace lskysd.techinventory.importers
{
    class DeviceTypeIdentifier
    {
        private Dictionary<string, DeviceType> _modelMappings = new Dictionary<string, DeviceType>();

        public DeviceTypeIdentifier(string connectionString)
        {
            // Get device types from the database

        }

        public DeviceType IdentifyByModel(string model)
        {
            string modelFormatted = model.ToLower().Trim();
            if (_modelMappings.ContainsKey(modelFormatted))
            {
                return _modelMappings[modelFormatted];
            }

            return DeviceType.Unknown;
        }
    }
}

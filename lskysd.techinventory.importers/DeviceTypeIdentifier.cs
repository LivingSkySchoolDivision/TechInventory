using lskysd.techinventory.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lskysd.techinventory.importers
{
    class DeviceTypeIdentifier
    {
        Dictionary<string, int> _modelMappings = new Dictionary<string, int>();
        private Dictionary<int, DeviceType> _deviceTypes = new Dictionary<int, DeviceType>();

        public DeviceTypeIdentifier(string connectionString)
        {
            // Get device types from the database
            DeviceTypeRepository _dtRepo = new DeviceTypeRepository(connectionString);
            _deviceTypes = _dtRepo.GetAllDictionary();

            DeviceTypeMappingRepository _dtmRepo = new DeviceTypeMappingRepository(connectionString);
            _modelMappings.Clear();
            foreach (DeviceTypeMapping dtm in _dtmRepo.GetAll())
            {
                _modelMappings.Add(dtm.ModelString.ToLower(), dtm.DeviceTypeID);
            }
        }

        public DeviceType IdentifyByModel(string model)
        {
            string modelFormatted = model.ToLower().Trim();
            if (_modelMappings.ContainsKey(modelFormatted))
            {
                if (_deviceTypes.ContainsKey(_modelMappings[modelFormatted]))
                {
                    return _deviceTypes[_modelMappings[modelFormatted]];
                } else
                {
                    return DeviceType.Unknown;
                }
            }
            return DeviceType.Unknown;
        }
    }
}

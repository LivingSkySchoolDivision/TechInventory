using lskysd.techinventory.util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace lskysd.techinventory.db
{
    public class DeviceTypeRepository
    {
        private Dictionary<int, DeviceType> _cache;

        public DeviceTypeRepository(string ConnectionString)
        {
            this._cache = new Dictionary<int, DeviceType>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT * FROM DeviceTypes;"
                })
                {
                    sqlCommand.Connection.Open();
                    SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                    if (dbDataReader.HasRows)
                    {
                        while (dbDataReader.Read())
                        {
                            int parsedID = dbDataReader["ID"].ToString().ToInt();
                            string parsedName = dbDataReader["Name"].ToString();

                            if (!_cache.ContainsKey(parsedID))
                            {
                                _cache.Add(parsedID, new DeviceType()
                                {
                                    Id = parsedID,
                                    Name = parsedName
                                });
                            }




                        }
                    }

                    sqlCommand.Connection.Close();
                }
            }

        }

        public List<DeviceType> GetAll()
        {
            return _cache.Values.OrderBy(x => x.Name).ToList();
        }

        public Dictionary<int, DeviceType> GetAllDictionary()
        {
            return _cache;
        }

        public DeviceType Get(int id)
        {
            if (_cache.ContainsKey(id))
            {
                return _cache[id];
            }

            return DeviceType.Unknown;
        }
    }
}

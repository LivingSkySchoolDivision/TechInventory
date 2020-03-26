using lskysd.techinventory.util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace lskysd.techinventory.db
{
    public class DeviceTypeMappingRepository
    {
        private string _connString = string.Empty;

        public DeviceTypeMappingRepository(string ConnectionString)
        {
            this._connString = ConnectionString;
        }

        private DeviceTypeMapping dataReaderToObject(SqlDataReader dataReader)
        {
            return new DeviceTypeMapping()
            {
                Id = dataReader["ID"].ToString().ToInt(),
                ModelString = dataReader["Model"].ToString(),
                DeviceTypeID = dataReader["DeviceTypeId"].ToString().ToInt()
            };
        }

        public List<DeviceTypeMapping> GetAll()
        {
            List<DeviceTypeMapping> returnMe = new List<DeviceTypeMapping>();

            using (SqlConnection connection = new SqlConnection(this._connString))
            {
                using (SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT * FROM DeviceTypeModelMappings;"
                })
                {
                    sqlCommand.Connection.Open();
                    SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                    if (dbDataReader.HasRows)
                    {
                        while (dbDataReader.Read())
                        {
                            DeviceTypeMapping obj = dataReaderToObject(dbDataReader);
                            if (obj != null)
                            {
                                returnMe.Add(obj);
                            }
                        }
                    }

                    sqlCommand.Connection.Close();
                }
            }

            return returnMe;
        }

    }
}

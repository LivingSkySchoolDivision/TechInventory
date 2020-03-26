using lskysd.techinventory.util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace lskysd.techinventory.db
{
    public class DeviceNameRepository
    {
        private string _connString = string.Empty;

        public DeviceNameRepository(string ConnectionString)
        {
            this._connString = ConnectionString;
        }

        private DeviceName dataReaderToObject(SqlDataReader dataReader)
        {
            return new DeviceName()
            {
                Id = dataReader["ID"].ToString().ToInt(),
                DeviceId = dataReader["DeviceId"].ToString().ToInt(),
                Value = dataReader["Value"].ToString()
            };
        }

        public List<DeviceName> GetAll()
        {
            List<DeviceName> returnMe = new List<DeviceName>();

            using (SqlConnection connection = new SqlConnection(this._connString))
            {
                using (SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT * FROM DeviceNames;"
                })
                {
                    sqlCommand.Connection.Open();
                    SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                    if (dbDataReader.HasRows)
                    {
                        while (dbDataReader.Read())
                        {
                            DeviceName obj = dataReaderToObject(dbDataReader);
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

        private Dictionary<int, List<string>> getAllDictionary()
        {
            Dictionary<int, List<string>> returnMe = new Dictionary<int, List<string>>();
            
            using (SqlConnection connection = new SqlConnection(this._connString))
            {
                using (SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT * FROM DeviceNames;"
                })
                {
                    sqlCommand.Connection.Open();
                    SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                    if (dbDataReader.HasRows)
                    {
                        while (dbDataReader.Read())
                        {
                            DeviceName obj = dataReaderToObject(dbDataReader);
                            if (obj != null)
                            {
                                if (!returnMe.ContainsKey(dbDataReader["DeviceId"].ToString().ToInt()))
                                {
                                    returnMe.Add(dbDataReader["DeviceId"].ToString().ToInt(), new List<string>());
                                }

                                returnMe[dbDataReader["DeviceId"].ToString().ToInt()].Add(dbDataReader["Value"].ToString());
                            }
                        }
                    }

                    sqlCommand.Connection.Close();
                }
            }

            return returnMe;
        }

        public void Add(List<DeviceName> DeviceNames)
        {
            // Check to make sure this combination doesn't already exist, and if it does, skip it
            Dictionary<int, List<string>> existingMappings = this.getAllDictionary();
            List<DeviceName> additions = new List<DeviceName>();

            foreach(DeviceName potentialAddition in DeviceNames)
            {
                if (existingMappings.ContainsKey(potentialAddition.DeviceId))
                {
                    if (existingMappings[potentialAddition.DeviceId].Contains(potentialAddition.Value))  {
                        continue;
                    }
                }
                additions.Add(potentialAddition);
            }
            
            using (SqlConnection connection = new SqlConnection(this._connString))
            {
                foreach (DeviceName deviceName in additions)
                {
                    using (SqlCommand sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        CommandType = CommandType.Text,
                        CommandText = "INSERT INTO DeviceNames(DeviceID, Value) " +
                                        "VALUES(@DEVICEID, @NAMEVALUE);"
                    })
                    {
                        sqlCommand.Parameters.AddWithValue("@DEVICEID", deviceName.DeviceId);
                        sqlCommand.Parameters.AddWithValue("@NAMEVALUE", deviceName.Value);
                        sqlCommand.Connection.Open();
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.Connection.Close();
                    }
                }
            }
            
        }

    }
}

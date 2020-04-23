using lskysd.techinventory.util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace lskysd.techinventory.db
{
    public class DeviceMACRepository
    {
        private string _connString = string.Empty;

        public DeviceMACRepository(string ConnectionString)
        {
            this._connString = ConnectionString;
        }

        private DeviceMACAddress dataReaderToObject(SqlDataReader dataReader)
        {
            return new DeviceMACAddress()
            {
                Id = dataReader["ID"].ToString().ToInt(),
                DeviceId = dataReader["DeviceID"].ToString().ToInt(),
                MACAddress = dataReader["MAC"].ToString()
            };
        }


        public List<DeviceMACAddress> GetAll()
        {
            List<DeviceMACAddress> returnMe = new List<DeviceMACAddress>();

            using (SqlConnection connection = new SqlConnection(this._connString))
            {
                using (SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT * FROM DeviceMACAddresses;"
                })
                {
                    sqlCommand.Connection.Open();
                    SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                    if (dbDataReader.HasRows)
                    {
                        while (dbDataReader.Read())
                        {
                            DeviceMACAddress obj = dataReaderToObject(dbDataReader);
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
                    CommandText = "SELECT * FROM DeviceMACAddresses;"
                })
                {
                    sqlCommand.Connection.Open();
                    SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                    if (dbDataReader.HasRows)
                    {
                        while (dbDataReader.Read())
                        {
                            DeviceMACAddress obj = dataReaderToObject(dbDataReader);
                            if (obj != null)
                            {
                                if (!returnMe.ContainsKey(obj.DeviceId))
                                {
                                    returnMe.Add(obj.DeviceId, new List<string>());
                                }

                                returnMe[obj.DeviceId].Add(obj.MACAddress);
                            }
                        }
                    }

                    sqlCommand.Connection.Close();
                }
            }

            return returnMe;
        }

        public void Add(List<DeviceMACAddress> DeviceMACAddresses)
        {
            // Check to make sure this combination doesn't alrady exist, and if it does, skip it
            List<DeviceMACAddress> additions = new List<DeviceMACAddress>();
            Dictionary<int, List<string>> existingMappings = this.getAllDictionary();

            foreach (DeviceMACAddress potentialAddition in DeviceMACAddresses)
            {
                if (existingMappings.ContainsKey(potentialAddition.DeviceId))
                {
                    if (existingMappings[potentialAddition.DeviceId].Contains(potentialAddition.MACAddress))
                    {
                        continue;
                    }
                }
                additions.Add(potentialAddition);
            }

            using (SqlConnection connection = new SqlConnection(this._connString))
            {
                foreach (DeviceMACAddress deviceMAC in additions)
                {
                    using (SqlCommand sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        CommandType = CommandType.Text,
                        CommandText = "INSERT INTO DeviceMACAddresses(DeviceID, MAC) " +
                                        "VALUES(@DEVICEID, @MACVALUE);"
                    })
                    {
                        sqlCommand.Parameters.AddWithValue("@DEVICEID", deviceMAC.DeviceId);
                        sqlCommand.Parameters.AddWithValue("@MACVALUE", deviceMAC.MACAddress);
                        sqlCommand.Connection.Open();
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.Connection.Close();
                    }
                }
            }

        }

    }
}

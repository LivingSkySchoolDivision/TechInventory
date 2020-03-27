using lskysd.techinventory.util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace lskysd.techinventory.db
{
    public class DeviceFacilityRepository
    {
        private string _connString = string.Empty;
        private readonly FacilityRepository _facilityRepo;

        public DeviceFacilityRepository(string ConnectionString)
        {
            this._connString = ConnectionString;
            this._facilityRepo = new FacilityRepository(ConnectionString);
        }

        public DeviceFacilityRepository(string ConnectionString, FacilityRepository FacilityRepo)
        {
            this._connString = ConnectionString;
            this._facilityRepo = FacilityRepo;
        }

        private DeviceFacility dataReaderToObject(SqlDataReader dataReader)
        {
            return new DeviceFacility()
            {
                Id = dataReader["ID"].ToString().ToInt(),
                DeviceId = dataReader["DeviceID"].ToString().ToInt(),
                Facility = _facilityRepo.Get(dataReader["FacilityId"].ToString().ToInt())
            };
        } 

        public List<DeviceFacility> GetAll()
        {
            List<DeviceFacility> returnMe = new List<DeviceFacility>();

            using (SqlConnection connection = new SqlConnection(this._connString))
            {
                using (SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT * FROM DeviceFacilities;"
                })
                {
                    sqlCommand.Connection.Open();
                    SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                    if (dbDataReader.HasRows)
                    {
                        while (dbDataReader.Read())
                        {
                            DeviceFacility obj = dataReaderToObject(dbDataReader);
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


        public void Add(List<DeviceFacility> NewDeviceFacilities)
        {
            // Check to make sure this combination doesn't already exist, and if it does, skip it
            List<DeviceFacility> actualAdditions = new List<DeviceFacility>();
            Dictionary<int, List<int>> existingDeviceFacilitiesByDeviceID = new Dictionary<int, List<int>>();
            foreach(DeviceFacility df in this.GetAll())
            {
                if (!existingDeviceFacilitiesByDeviceID.ContainsKey(df.DeviceId))
                {
                    existingDeviceFacilitiesByDeviceID.Add(df.DeviceId, new List<int>());
                }
                existingDeviceFacilitiesByDeviceID[df.DeviceId].Add(df.Facility.Id);
            }

            foreach(DeviceFacility df in NewDeviceFacilities)
            {
                if (existingDeviceFacilitiesByDeviceID.ContainsKey(df.DeviceId))
                {
                    if  (existingDeviceFacilitiesByDeviceID[df.DeviceId].Contains(df.Facility.Id))
                    {
                        continue;
                    }
                }
                actualAdditions.Add(df);
            }

            using (SqlConnection connection = new SqlConnection(this._connString))
            {
                foreach (DeviceFacility deviceFacility in actualAdditions)
                {
                    if (deviceFacility.Facility != null)
                    {
                        if (deviceFacility.Facility.Id > 0)
                        {
                            using (SqlCommand sqlCommand = new SqlCommand
                            {
                                Connection = connection,
                                CommandType = CommandType.Text,
                                CommandText = "INSERT INTO DeviceFacilities(DeviceID, FacilityID) " +
                                                "VALUES(@DEVICEID, @FACID);"
                            })
                            {
                                sqlCommand.Parameters.AddWithValue("@DEVICEID", deviceFacility.DeviceId);
                                sqlCommand.Parameters.AddWithValue("@FACID", deviceFacility.Facility.Id);
                                sqlCommand.Connection.Open();
                                sqlCommand.ExecuteNonQuery();
                                sqlCommand.Connection.Close();
                            }

                        }
                    }
                }
            }


        } 
    }
}

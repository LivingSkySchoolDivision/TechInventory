using lskysd.techinventory.util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace lskysd.techinventory.db
{
    public class DeviceRepository
    {
        private string _connString = string.Empty;
        private DeviceTypeRepository _deviceTypeRepo;


        public  DeviceRepository(string ConnectionString)
        {
            this._connString = ConnectionString;
            this._deviceTypeRepo = new DeviceTypeRepository(ConnectionString);
        }

        private Device dataReaderToObject(SqlDataReader dataReader)
        {
            return new Device()
            {
                Id = dataReader["ID"].ToString().ToInt(),
                SerialNumber = dataReader["SerialNo"].ToString(),
                Model = dataReader["Model"].ToString(),
                Notes = dataReader["DeviceNotes"].ToString(),
                PurchaseYear = dataReader["PurchaseYear"].ToString().ToInt(),
                PurchaseDate = dataReader["PurchaseDate"].ToString().ToDateTime(),
                IsActive = dataReader["IsActive"].ToString().ToBool(),
                DeviceType = _deviceTypeRepo.Get(dataReader["DeviceType"].ToString().ToInt())
            };
        }

        public List<Device> GetAll()
        {
            List<Device> returnMe = new List<Device>();

            using (SqlConnection connection = new SqlConnection(this._connString))
            {
                using (SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT * FROM Devices;"
                })
                {
                    sqlCommand.Connection.Open();
                    SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                    if (dbDataReader.HasRows)
                    {
                        while (dbDataReader.Read())
                        {
                            Device obj = dataReaderToObject(dbDataReader);
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

        public void AddOrUpdate(List<Device> Devices)
        {
            Console.WriteLine("Adding or inserting " + Devices.Count + " records...");
            // Get a list of all devices, so we know which ones we need to update
            Dictionary<string, Device> allDevicesBySerial = this.GetAll().ToDictionary(x => x.SerialNumber);
            List<Device> newDevices = new List<Device>();
            List<Device> existingDevices = new List<Device>();

            Console.WriteLine(" Analyzing new records...");
            foreach(Device newDevice in Devices)
            {
                if (allDevicesBySerial.ContainsKey(newDevice.SerialNumber))
                {
                    if (allDevicesBySerial[newDevice.SerialNumber].NeedsUpdate(newDevice))
                    {
                        existingDevices.Add(newDevice);
                    }
                } else
                {
                    newDevices.Add(newDevice);
                }
            }

            Console.WriteLine(" Found " + newDevices.Count + " records to add");
            Console.WriteLine(" Found " + existingDevices.Count + " records to update");

            // Add new entries
            this.Add(newDevices);

            // Update existing entries
            this.Update(existingDevices);

        }

        public void Update(List<Device> Devices)
        {
            using (SqlConnection connection = new SqlConnection(this._connString))
            {
                foreach (Device device in Devices)
                {
                    using (SqlCommand sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        CommandType = CommandType.Text,
                        CommandText = "UPDATE Devices SET SerialNo=@SERIAL, Model=@MODELNO, PurchaseYear=@PURCHYEAR, PurchaseDate=@PURCHDATE, IsActive=@ISACTIVE, DeviceNotes=@DEVNOTES, DeviceType=@DEVICETYPEID WHERE Id=@DEVICEID;"
                    })
                    {
                        sqlCommand.Parameters.AddWithValue("DEVICEID", device.Id);
                        sqlCommand.Parameters.AddWithValue("SERIAL", device.SerialNumber);
                        sqlCommand.Parameters.AddWithValue("MODELNO", device.Model);
                        sqlCommand.Parameters.AddWithValue("PURCHYEAR", device.PurchaseYear);
                        sqlCommand.Parameters.AddWithValue("PURCHDATE", device.PurchaseDate.ToDatabaseSafeDateTime());
                        sqlCommand.Parameters.AddWithValue("ISACTIVE", device.IsActive);
                        sqlCommand.Parameters.AddWithValue("DEVNOTES", !string.IsNullOrEmpty(device.Notes) ? device.Notes : string.Empty);
                        sqlCommand.Parameters.AddWithValue("DEVICETYPEID", device.DeviceType.Id);
                        sqlCommand.Connection.Open();
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.Connection.Close();
                    }
                }
            }
        }

        public void Add(List<Device> Devices)
        {
            using (SqlConnection connection = new SqlConnection(this._connString))
            {
                foreach (Device device in Devices) {
                    using (SqlCommand sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        CommandType = CommandType.Text,
                        CommandText = "INSERT INTO Devices(SerialNo, Model, PurchaseYear, PurchaseDate, IsActive, DeviceNotes, DeviceType) " +
                                        "VALUES(@SERIAL, @MODELNO, @PURCHYEAR, @PURCHDATE, @ISACTIVE, @DEVNOTES, @DEVICETYPEID);"
                    })
                    {
                        sqlCommand.Parameters.AddWithValue("@SERIAL", device.SerialNumber);
                        sqlCommand.Parameters.AddWithValue("@MODELNO", device.Model);
                        sqlCommand.Parameters.AddWithValue("@PURCHYEAR", device.PurchaseYear);
                        sqlCommand.Parameters.AddWithValue("@PURCHDATE", device.PurchaseDate.ToDatabaseSafeDateTime());
                        sqlCommand.Parameters.AddWithValue("@ISACTIVE", device.IsActive);
                        sqlCommand.Parameters.AddWithValue("@DEVNOTES", !string.IsNullOrEmpty(device.Notes) ? device.Notes : string.Empty);
                        sqlCommand.Parameters.AddWithValue("@DEVICETYPEID", device.DeviceType.Id);
                        sqlCommand.Connection.Open();
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.Connection.Close();
                    }
                }
            }
        }
    }
}

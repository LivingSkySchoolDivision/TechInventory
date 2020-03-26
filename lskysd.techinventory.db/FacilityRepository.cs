using lskysd.techinventory.util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace lskysd.techinventory.db
{
    public class FacilityRepository
    {
        private string _connString = string.Empty;
        private Dictionary<int, Facility> _cache = new Dictionary<int, Facility>();

        public FacilityRepository(string ConnectionString)
        {
            this._connString = ConnectionString;

            _cache.Clear();
            using (SqlConnection connection = new SqlConnection(this._connString))
            {
                using (SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT * FROM Facilities;"
                })
                {
                    sqlCommand.Connection.Open();
                    SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                    if (dbDataReader.HasRows)
                    {
                        while (dbDataReader.Read())
                        {
                            Facility obj = dataReaderToObject(dbDataReader);
                            if (obj != null)
                            {
                                _cache.Add(obj.Id, obj);
                            }
                        }
                    }

                    sqlCommand.Connection.Close();
                }
            }

        }

        private Facility dataReaderToObject(SqlDataReader dataReader)
        {
            return new Facility()
            {
                Id = dataReader["ID"].ToString().ToInt(),
                Name = dataReader["Name"].ToString()
            };
        }

        public Facility Get(int id)
        {
            if (_cache.ContainsKey(id))
            {
                return _cache[id];
            }
            return Facility.Unknown;
        }
    }
}

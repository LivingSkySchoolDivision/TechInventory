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
        private Dictionary<string, Facility> _nameCache = new Dictionary<string, Facility>();

        public FacilityRepository(string ConnectionString)
        {
            this._connString = ConnectionString;
            _nameCache.Clear();
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

                                // Create an index of names and alt names for easier searching
                                if (!this._nameCache.ContainsKey(obj.Name.ToLower()))
                                {
                                    this._nameCache.Add(obj.Name.ToLower(), obj);
                                }

                                foreach(string name in obj.AlternateNames)
                                {
                                    if (!this._nameCache.ContainsKey(name.ToLower()))
                                    {
                                        this._nameCache.Add(name.ToLower(), obj);
                                    }
                                }
                            }
                        }
                    }

                    sqlCommand.Connection.Close();
                }
            }


        }

        private Facility dataReaderToObject(SqlDataReader dataReader)
        {
            List<string> alternateNames = new List<string>();

            foreach(string name in dataReader["alternatenames"].ToString().Split(';'))
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (name.Length > 1)
                    {
                        if (!alternateNames.Contains(name))
                        {
                            alternateNames.Add(name);
                        }
                    }
                }
            }

            return new Facility()
            {
                Id = dataReader["ID"].ToString().ToInt(),
                Name = dataReader["Name"].ToString(),
                AlternateNames = alternateNames
            };
        }

        public Facility GetByName(string name)
        {
            if (this._nameCache.ContainsKey(name.ToLower()))
            {
                return this._nameCache[name.ToLower()];
            }

            return Facility.Unknown;
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

using System;
using System.Collections.Generic;
using System.Text;

namespace lskysd.techinventory
{
    public class Facility
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> AlternateNames { get; set; }

        public Facility()
        {
            this.AlternateNames = new List<string>();
        }

        public override string ToString()
        {
            return this.Name;
        }

        public static Facility Unknown = new Facility()
        {
            Id = 0,
            Name = "Unknown"
        };
    }
}

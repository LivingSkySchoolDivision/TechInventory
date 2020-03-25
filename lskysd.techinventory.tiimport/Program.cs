using Microsoft.VisualBasic.FileIO;
using System;

namespace lskysd.techinventory.tiimport
{
    class Program
    {
        static void Main(string[] args)
        {
            // Attempt to import a CSV
            string fileName = "meraki.csv";

            using (TextFieldParser parser = new TextFieldParser(fileName))
            {

            }
        }
    }
}

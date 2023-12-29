using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using System.Linq;
using CsvHelper.Configuration;

namespace Galytix.WebApi.Helper
{

  
    public static class CsvR
    {
        public static List<Gwpdata> ReadCsvFile(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true }))
                try
                {
                    return csv.GetRecords<Gwpdata>().ToList();
                }
                catch (Exception ex)
                {
                    
                    throw new Exception("Error reading CSV file.", ex);
                }
        }
    }

}
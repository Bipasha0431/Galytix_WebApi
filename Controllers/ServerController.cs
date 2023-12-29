using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Galytix.WebApi.Helper;
using Galytix.WebApi.Models;

namespace Galytix.WebApi.Controllers
{
    [Route("api/gwp")]
    [ApiController]
    public class CountryGwpController : ControllerBase
    {
        [HttpPost("avg")]
        public async Task<IActionResult> GetAverageGwp()
        {
            try
            {
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    string requestBody = await reader.ReadToEndAsync();
                    var request = JsonConvert.DeserializeObject<GwpReq>(requestBody);

                    if (request == null || string.IsNullOrEmpty(request.Country) || request.Lob == null || !request.Lob.Any())
                    {
                        return BadRequest("Invalid request payload");
                    }

                    Console.WriteLine(Directory.GetCurrentDirectory());

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "gwpByCountry.csv");

                    if (!System.IO.File.Exists(filePath))
                    {
                        return NotFound("CSV file not found");
                    }

                    var data = CsvR.ReadCsvFile(filePath);

                    var result = new Dictionary<string, decimal>();
                    foreach (var lob in request.Lob)
                    {
                        var filteredData = data
                            .Where(entry => entry.CountryCode.ToLower() == request.Country.ToLower() && entry.LineOfBusiness.ToLower() == lob.ToLower())
                            .Select(entry => entry.Gwp)
                            .DefaultIfEmpty(0)
                            .Average();

                        result.Add(lob, filteredData);
                    }

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

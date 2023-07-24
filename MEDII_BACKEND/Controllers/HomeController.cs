using MEDII_BACKEND.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MEDII_BACKEND.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            Dictionary<string, string> message = new Dictionary<string, string>();
            message.Add("Nombre", "MEDIIGSS-BACKEND");
            message.Add("Version", "1.0");

            return Ok(message);
        }

        [HttpGet("/api/Home/DetailLogs/{date}")]
        public IActionResult ShowLogs(string date)
        {
            string currentFile;
            List<Logs> list;
            string[] lines;
            int i;

            try
            {
                currentFile = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Logs"));

                currentFile += "/Log-" + date + ".txt";

                if (!System.IO.File.Exists(currentFile))
                {
                    throw new Exception("File with date " + date + " not found!");
                }

                lines = System.IO.File.ReadAllLines(currentFile);

                list = new List<Logs>();

                i = 0;

                foreach (string line in lines)
                {
                    if (!String.IsNullOrEmpty(line))
                    {
                        i++;

                        var dto = new Logs
                        {
                            Detalle = i + ". " + line
                        };

                        list.Add(dto);
                    }
                }

                return Ok(JsonConvert.SerializeObject(list));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

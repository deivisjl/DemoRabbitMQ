using Microsoft.AspNetCore.Mvc;
using RabbitMQBus.QueueEvent;
using RabbitMQBus.RabbitBus;
using System.Text;

namespace API_BROKER.Controllers
{
    public class SignBoxCCGController : Controller
    {        
        private readonly IRabbitEventBus _rabbitEventBus;

        public SignBoxCCGController(IRabbitEventBus rabbitEventBus) 
        { 
            _rabbitEventBus = rabbitEventBus;
        }

        [HttpGet("/api/CCG/Home")]
        public IActionResult Get()
        {
            Dictionary<string, string> message = new Dictionary<string, string>();
            message.Add("Nombre", "Comunicación asincrona RabbitMQ-MEDIIGSS-CCG");
            message.Add("Version", "1.0");

            return Ok(message);
        }

        [HttpPost("/api/CCG/SignBox/{name}")]
        public async Task<IActionResult> ResponseCCG(string name)
        {
            HttpRequest request;
            StreamReader reader;
            byte[] documentBytes;
            string pdfFile;

            try
            {
                request = Request;

                if (!request.Body.CanSeek)
                {
                    request.EnableBuffering();
                }

                request.Body.Position = 0;

                reader = new StreamReader(request.Body, Encoding.UTF8);

                request.Body.Position = 0;

                documentBytes = default(byte[]);

                using (var memstream = new MemoryStream())
                {
                    var buffer = new byte[Convert.ToInt32(request.ContentLength)];
                    var bytesRead = default(int);

                    while ((bytesRead = await reader.BaseStream.ReadAsync(buffer, 0, buffer.Length)) > 0) 
                    {
                        memstream.Write(buffer, 0, bytesRead);
                    }

                    documentBytes = memstream.ToArray();
                }

                pdfFile = Convert.ToBase64String(documentBytes);

                _rabbitEventBus.Publish(new EmailEventQueue("deivis.lopez@igssgt.org", name, pdfFile));

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

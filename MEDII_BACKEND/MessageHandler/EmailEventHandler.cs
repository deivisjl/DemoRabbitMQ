using RabbitMQBus.QueueEvent;
using RabbitMQBus.RabbitBus;
using System.Text;

namespace MEDII_BACKEND.MessageHandler
{
    public class EmailEventHandler : IDriveRabbitEvent<EmailEventQueue>
    {
        public Task Handle(EmailEventQueue @event)
        {
            byte[] pdfBytes;
            string filePath;
            FileStream pdfFile;

            filePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory,"Logs")) + @event.Title;

            pdfBytes = Convert.FromBase64String(@event.Body);

            pdfFile = new FileStream(filePath, FileMode.Create);
            pdfFile.Write(pdfBytes,0,pdfBytes.Length);
            pdfFile.Close();

            this.SaveLogs(@event.Title,@event.Address);

            return Task.CompletedTask;
        }

        private void SaveLogs(string title, string address)
        {
            StreamWriter logFile;
            string description;
            string currentDate;
            string currentDateTime;
            string currentFile;

            try
            {
                currentDate = DateTime.Now.ToString("dd-MM-yyyy");

                currentDateTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                currentFile = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Logs"));

                currentFile += "/Log-" + currentDate + ".txt";

                description = string.Format(@"Fecha de registro: {0}, Nombre de documento: {1}, Origen: {2}", currentDateTime, title, address);

                if(System.IO.File.Exists(currentFile))
                {
                    logFile = System.IO.File.AppendText(currentFile);

                    logFile.WriteLine(description);

                    logFile.Dispose();

                    return;
                }

                logFile = new StreamWriter(currentFile, true, Encoding.ASCII);

                logFile.WriteLine(description);

                logFile.Close();

                return;
            }
            catch (Exception)
            {
                Console.WriteLine("Exception");
            }
            finally
            {
                Console.WriteLine("Finally");
            }
        }
    }
}

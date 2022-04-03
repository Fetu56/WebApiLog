using Microsoft.AspNetCore.Mvc;
using WebApiLog.Logic;

namespace WebApiLog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private string _uploads;
        public FileController(IWebHostEnvironment environment)
        {
            _uploads = Path.Combine(environment.WebRootPath != null ? environment.WebRootPath : environment.ContentRootPath, "uploads");
            
        }

        [HttpGet("files")]
        public async Task<ActionResult> DownloadFile(string name)
        {
            ActionResult result = Problem();
            try
            {
                ArgumentNullException.ThrowIfNull(name);
                var filePath = _uploads+$"\\{name}";
                if (System.IO.File.Exists(filePath))
                {
                    var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
                    result = File(bytes, "text/plain", Path.GetFileName(filePath));
                }
            } 
            catch(Exception) { }
            DbLogger.Log("Problems with file download", GetIp(), result);
            return result;
        }

        [HttpPost("multiple-files")]
        public void Upload(List<IFormFile> files)
        {
            if(!Directory.Exists(_uploads))
            {
                Directory.CreateDirectory(_uploads);
            }
            files.ForEach(async f =>
            {
                if (f.Length > 0)
                {
                    string filePath = Path.Combine(_uploads, f.FileName);
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        DbLogger.Log("File saved", GetIp());
                        await f.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    DbLogger.Log("Problem with file data", GetIp());
                }
            });
        }
        private string GetIp()
        {
            return this.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}
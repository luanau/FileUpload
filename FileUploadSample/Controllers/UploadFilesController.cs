using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FileUploadSample.Controllers
{
    public class UploadFilesController : Controller
    {
        private IHostingEnvironment _env;
        IConfiguration Configuration;

        public UploadFilesController(IHostingEnvironment env, IConfiguration configuration)
        {
            _env = env;
            Configuration = configuration;
        }
        #region snippet1
        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            var uploadFolder = Path.Combine(_env.WebRootPath, Configuration.GetValue<string>("UploadFolder"));

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine(uploadFolder, formFile.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size });
        }
        #endregion


        [HttpGet]
        public HttpResponseMessage GetFile()
        {
            var stream = new MemoryStream();
            // processing the stream.

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(stream.ToArray())
            };

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") {
                    FileName = @"C:\Users\luan_\AppData\Local\Temp\tmp9CC5.tmp"
            };

            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return result;
        }



        //[HttpGet("{id}")]
        //public async Task<IActionResult> Download(string id)
        //{
        //    var stream = new StreamContent(File.OpenRead(@"C:\Users\luan_\AppData\Local\Temp\tmp9CC5.tmp"));
        //    var stream2 = await stream.ReadAsByteArrayAsync();
        //    //var response = File.r(stream2, "application/octet-stream"); // FileStreamResult
        //    var response = new FileStreamResult(stream2, "application/octet-stream");
        //    return response;
        //}

        public FileResult TestDownload()
        {
            HttpContext.Response.ContentType = "application/pdf";
            FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(@"C:\Users\luan_\AppData\Local\Temp\tmp9CC5.tmp"), "application/octet-stream")
            {
                FileDownloadName = @"C:\Users\luan_\AppData\Local\Temp\tmp9CC5.tmp"
            };

            return result;
        }
    }

}

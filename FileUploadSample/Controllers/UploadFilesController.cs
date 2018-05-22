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

            var uploadFolder = Path.Combine(_env.WebRootPath, Configuration["UploadFolder"]);

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


        /// <summary>
        /// Two parameters File opens it
        /// Three parameters File download it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Download(string id)
        {
            var path = @"C:\Users\luan_\Downloads\Premium_Dancer.png";
            var image = System.IO.File.OpenRead(path);
            return await Task.Run(() => File(image, GetContentType(path)));

        }

        /// <summary>
        /// This one works to download a file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Download2(string filename)
        {
            //if (filename == null)
            //    return Content("filename not present");

            //var path = Path.Combine(
            //               Directory.GetCurrentDirectory(),
            //               "wwwroot", filename);
            //var path = @"C:\Users\luan_\AppData\Local\Temp\tmp9CC5.tmp";
            var path = "C:\\dotNetDev\\FileUpload\\FileUploadSample\\wwwroot\\uploads\\U16-16-18_Scholars.pdf";
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path)); // Open in browser
            //return File(memory, GetContentType(path), Path.GetFileName(path)); // Download
        }

        /// <summary>
        /// This just download the file
        /// </summary>
        /// <returns></returns>
        public FileResult TestDownload()
        {
            var path = "C:\\dotNetDev\\FileUpload\\FileUploadSample\\wwwroot\\uploads\\U16-16-18_Scholars.pdf";

            HttpContext.Response.ContentType = "application/pdf";
            FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(path), "application/octet-stream")
            {
                FileDownloadName = "Cool16.pdf"
            };

            return result;
        }

        /// <summary>
        /// This one open image fine but in current tab, not new tab
        /// To open image in a new tab access this action via an anchor with blank target:
        ///     <a href="UploadFiles\Download3" target="_blank">PDF File link</a>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Download3()
        {
            var path = @"C:\Users\luan_\Downloads\BuddhaTruc.jpg";
            var image = System.IO.File.OpenRead(path);
            return await Task.Run(() => File(image, "image/jpeg"));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }

}

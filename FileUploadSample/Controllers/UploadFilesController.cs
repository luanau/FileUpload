using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeTypes;


namespace FileUploadSample.Controllers
{
    public class UploadFilesController : Controller
    {
        private IHostingEnvironment _env;
        IConfiguration Configuration;

        public string UploadFolder { get; set; }

        public UploadFilesController(IHostingEnvironment env, IConfiguration configuration)
        {
            _env = env;
            Configuration = configuration;
            UploadFolder = Path.Combine(_env.WebRootPath, Configuration["UploadFolder"]);

        }

        #region Upload Files

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            var uploadFolder = Path.Combine(_env.WebRootPath, Configuration["UploadFolder"]);
            var tasks = files.Select(x => CopyFile(uploadFolder,x).ContinueWith(RecordUploadedFile)).ToList();
            await Task.WhenAll(tasks);

            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size, names = files.Select(x => x.FileName) });
        }

        private async Task<IFormFile> CopyFile(string uploadFolder, IFormFile formFile)
        {
            var filePath = Path.Combine(uploadFolder, formFile.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            return formFile;
        }

        private void RecordUploadedFile(Task<IFormFile> prev)
        {
            Console.WriteLine(prev.Result.FileName);
        }

        #endregion

        /// <summary>
        /// Two parameters File opens it
        /// Three parameters File download it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Download(string fileName)
        {
            var path = Path.Combine(UploadFolder, fileName);
            var image = System.IO.File.OpenRead(path);
            return await Task.Run(() => File(image, MimeTypeMap.GetMimeType(Path.GetExtension(path)), fileName));
        }

        /// <summary>
        /// This one works to download a file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Download2(string filename)
        {
            var path = Path.Combine(UploadFolder, filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, MimeTypeMap.GetMimeType(Path.GetExtension(path))); // Open in browser
            //return File(memory,  MimeTypeMap.GetMimeType(Path.GetExtension(path)), Path.GetFileName(path)); // Download
        }


        /// <summary>
        /// To open image in a new tab access this action via an anchor with blank target:
        ///     <a href="UploadFiles\Download3" target="_blank">PDF File link</a>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> OpenFile(string fileName)
        {
            var path = Path.Combine(UploadFolder, fileName);
            var image = System.IO.File.OpenRead(path);
            return await Task.Run(() => File(image, MimeTypeMap.GetMimeType(Path.GetExtension(path))));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string fileName)
        {
            var path = Path.Combine(UploadFolder, fileName);
            await Task.Run(() => System.IO.File.Delete(path));
            return Ok(new { message = fileName + " deleted!" });
        }
    }

}

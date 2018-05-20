using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadSample.Controllers
{
    public class UploadFilesController : Controller
    {
        private IHostingEnvironment _env;

        public UploadFilesController(IHostingEnvironment env)
        {
            _env = env;
        }
        #region snippet1
        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            //var filePath = Path.GetTempFileName();

            foreach (var formFile in files)
            {
                var filePath = Path.Combine(_env.WebRootPath, "documents", formFile.FileName);

                if (formFile.Length > 0)
                {
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
    }
}

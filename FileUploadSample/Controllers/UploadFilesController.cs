using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    }
}

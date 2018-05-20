using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Text;

namespace FileUploadSample.Controllers
{
    public class HomeController : Controller
    {
        private IHostingEnvironment Env { get; set; }

        public HomeController(IHostingEnvironment env)
        {
            Env = env;
        }
        public IActionResult Index()
        {
            ViewData["myFiles"] = getFileNames();
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        private string getFileLinks()
        {
            DirectoryInfo dir;
            StringBuilder sb = new StringBuilder();
            FileInfo[] files;

            dir = new DirectoryInfo(Path.Combine(Env.WebRootPath, "documents"));
            files = dir.GetFiles();
            foreach (FileInfo f in files)
            {
                sb.Append("<a href=\"" + f.Name.ToString() + "\">");
                sb.Append(f.Name.ToString() + "</a><br />");
            }
            return sb.ToString();
        }

        private FileLink[] getFileNames()
        {
            DirectoryInfo dir;
            StringBuilder sb = new StringBuilder();
            FileInfo[] files;

            var dirName = Path.Combine(Env.WebRootPath, "documents");
            dir = new DirectoryInfo(dirName);
            if (!dir.Exists)
            {
                Directory.CreateDirectory(dirName);
            }
            files = dir.GetFiles();
            return files.Select(x => new FileLink{ FullName = x.FullName, FileName = x.Name }).ToArray();
        }

    }


    public class FileLink
    {
        public string FileName { get; set; }
        public string FullName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System.Linq;
using System.IO;
using FileUploadSample.Controllers;
using System.Net;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Configuration;


namespace FileUploadSample.Tests.Home
{
    public class HomeControllerTests
    {
        [Fact]
        public async void UnitTest1()
        {
            var conf = new Mock<IConfiguration>();
            conf.Setup(x => x["UploadFolder"]).Returns(value: "TestUploadFolder");

            var env = new Mock<IHostingEnvironment>();
            env.SetupGet(x => x.WebRootPath).Returns(Path.GetTempPath());

            var controller = new UploadFilesController(env.Object, conf.Object);

            using (var stream = new MemoryStream(new byte[] { 1, 2, 3, 4 }))
            {
                // Arrange

                // create a REAL form file
                var formFile = new FormFile(stream , 0, stream.Length, "name", "filename");

                //Act
                var result = await controller.Post(new List<IFormFile> { formFile });
                var okResult = result as OkObjectResult;

                var data = okResult.Value;

                //Assert
                Assert.IsType<OkObjectResult>(okResult);
            }
    }
    }
}

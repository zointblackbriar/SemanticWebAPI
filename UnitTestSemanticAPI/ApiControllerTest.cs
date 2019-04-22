using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SemanticAPI.Controllers;
using SemanticAPI.Models.Options;
using SemanticAPI.OPCUAModel;
using System.Threading.Tasks;
using Xunit;

namespace UnitTestSemanticAPI
{
    public class ApiControllerTest
    {
        //[Fact]
        //public IActionResult TestGetDataSet()
        //{
        //    IHostingEnvironment env;
        //    //IOptions<ServerOptions> sampleOptions = Options.Create<ServerOptions>(new ServerOptions());
        //    var builder = new ConfigurationBuilder()
        //        .SetBasePath(env.ContentRootPath)
        //        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
        //        .AddEnvironmentVariables();

        //    Configuration = builder.Build();


        //    OPCUAClient _client = new OPCUAClient();
        //    ApiController _apiController = new ApiController(Configuration, _client);

        //    var result = _apiController.getDataSets();
        //    var okResult = result as OkObjectResult;
        //    Assert.NotNull(okResult);
        //    Assert.Equal(200, okResult.StatusCode);
        //    return null;
        //}

        [Fact]
        public Task<IActionResult> TestReadFromServer()
        {
            IOptions<ServerOptions> sampleOptions = Options.Create<ServerOptions>(new ServerOptions());
            OPCUAClient _client = new OPCUAClient();

            ApiController _apiController = new ApiController(sampleOptions, _client);
            var result = _apiController.GetNode(1);
            var okResult = result;

            Assert.NotNull(okResult);
            //Assert.Equal(200, okResult);
            return null;
        }


    }

}

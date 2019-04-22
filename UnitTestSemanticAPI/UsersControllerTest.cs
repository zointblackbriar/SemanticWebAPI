using System;
using Xunit;
using SemanticAPI.Controllers;
using System.Threading.Tasks;
using SemanticAPI.Services;
using SemanticAPI.Helpers;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using Microsoft.AspNetCore.TestHost;
using SemanticAPI.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SemanticAPI.Entities;

namespace UnitTestSemanticAPI
{
    public class UsersControllerTest
    {

        [Fact]
        public async Task DefaultPage()
        {
            var builder = new WebHostBuilder()
            .UseContentRoot(
                    @"G:\AllFiles\Projeler\CSharpProjeler\DotNetCore\SemanticWebService\V2\SemanticAPI\SemanticAPI")
            .UseEnvironment("Development")
            .UseStartup<SemanticAPI.Startup>()
            .UseApplicationInsights();

            TestServer testServer = new TestServer(builder);
            HttpClient client = testServer.CreateClient();

            HttpResponseMessage response = await client.GetAsync("/api");

            response = await client.GetAsync("/api");

            //fail if not successful
            response.EnsureSuccessStatusCode();

            //Get the response as a string
            string responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains("Api Entry point", responseHtml);
        }

        [Fact]
        public async Task TestUsersGet()
        {
            //AppSettings _appSettings;
            IOptions<AppSettings> sampleOptions = Options.Create<AppSettings>(new AppSettings());

            IUserService _userService = new UserService(sampleOptions);
            UsersController _userController = new UsersController(_userService);

            var okResult = await _userController.GetAll();
            Console.WriteLine(okResult);
            var objectResult = Assert.IsType<OkObjectResult>(okResult);
            var model = Assert.IsAssignableFrom<List<IEnumerable<User>>>(objectResult.Value);
            Assert.Equal(2, model.Count);
        }

    }
}


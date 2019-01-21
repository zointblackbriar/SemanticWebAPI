using System;
using Xunit;
using SemanticAPI.Controllers;
using SemanticAPI.Entities;
using System.Threading.Tasks;
using SemanticAPI.Services;
using Moq;
using SemanticAPI.Services;
using SemanticAPI.Entities;
using SemanticAPI.Controllers;
using System.Collections.Generic;
using SemanticAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using Microsoft.AspNetCore.TestHost;

namespace UnitTestSemanticAPI
{
    public class UsersControllerTest
    {
        private IUserService _userService;


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
        public async Task UsersGet()
        {
           var okResult = _userService.GetAll();
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

    }
}


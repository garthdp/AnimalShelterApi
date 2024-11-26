using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SPCAAPI.Controllers;
using SPCAAPI.Data;
using SPCAAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTesting
{
    public class AnimalControllerTest
    {
        /*
        Code Attribution
        Title: Unit Testing ASP.NET Core Web APIs with xUnit and Moq
        Author: Janki Mehta
        Link: https://dev.to/me_janki/unit-testing-aspnet-core-web-apis-with-xunit-and-moq-53i8
        Usage: Used to understand how to make unit tests for api controllers
        */
        /*
        Code Attribution
        Title: How To CORRECTLY Unit Test A .Net API Using Moq & AutoFixture
        Author: Israel Quiroz
        Link: https://www.youtube.com/watch?v=pLNzz7C2_08
        Usage: Used to understand more about how to make unit tests for api controllers
        */

        private readonly Mock<WilDbContext> _mockContext;
        private Fixture _fixture;

        public AnimalControllerTest()
        {
            _fixture = new Fixture();
            _mockContext = new Mock<WilDbContext>();
        }
        // tests to see if it can get animals and the correct amount of animals
        [Fact]
        public async Task GetAnimals_ShouldReturnOkResult()
        {
            var animals = _fixture.CreateMany<Animal>(2).AsQueryable();

            var mockDbSet = new MockDbSet<Animal>(animals);
            _mockContext.Setup(c => c.Animals).Returns(mockDbSet.Object);

            var controller = new AnimalController(_mockContext.Object);

            var result = await controller.GetAnimals() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var returnedEvents = result.Value as List<Animal>;
            Assert.NotNull(returnedEvents);
            Assert.Equal(2, returnedEvents.Count);
        }
    }
}

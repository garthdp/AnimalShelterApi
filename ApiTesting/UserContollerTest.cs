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
    public class UserContollerTest
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

        public UserContollerTest()
        {
            _fixture = new Fixture();
            _mockContext = new Mock<WilDbContext>();
        }

        // tests to see if it can get users the users information
        [Fact]
        public async Task GetUserInfo_ShouldReturnOkResult()
        {
            var users = _fixture.CreateMany<User>(2).AsQueryable();

            var mockDbSet = new MockDbSet<User>(users);
            _mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);

            var controller = new UserController(_mockContext.Object);

            var result = await controller.GetInfo(users.First().UserEmail) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        // test to see if it can update the users information
        [Fact]
        public async Task UpdateUser_ShouldReturnOkResult()
        {
            var userOld = new User
            {
                UserEmail = "test@test.com",
                Password = "testPassword",
                FirstName = "Test",
                LastName = "Test",
                PhoneNumber = "",
                Address = "",
                City = "",
                ProfilePicture = ""
            };
            var userNew = new RecieveUser
            {
                UserEmail = "test@test.com",
                Password = "testPassword1",
                FirstName = "Test1",
                LastName = "Test1",
                PhoneNumber = "0826574549",
                Address = "16 Bradley road",
                City = "PE",
                ProfilePicture = ""
            };

            var users = new List<User> { userOld }.AsQueryable();
            var mockDbSet = new MockDbSet<User>(users);
            _mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);

            var controller = new UserController(_mockContext.Object);

            var result = await controller.Patch(userNew.UserEmail, userNew) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        // test to see if it can register a new user
        [Fact]
        public async Task PostRegistration_ShouldReturnOkResult()
        {
            var user = new User
            {
                UserEmail = "test@test.com",
                Password = "testPassword",
                FirstName = "Test",
                LastName = "Test",
                PhoneNumber = "",
                Address = "",
                City = "",
                ProfilePicture = ""
            };

            var users = new List<User>().AsQueryable();
            var mockDbSet = new MockDbSet<User>(users);
            _mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);

            var controller = new UserController(_mockContext.Object);

            var result = await controller.Register(user) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
    }
}

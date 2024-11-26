using Moq;
using SPCAAPI.Controllers;
using SPCAAPI.Models;
using SPCAAPI.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;

namespace ApiTesting
{
    public class VolunteerControllerTest
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

        public VolunteerControllerTest()
        {
            _fixture = new Fixture();
            _mockContext = new Mock<WilDbContext>();
        }

        // tests to see if it can get volunteers and the correct number of volunteers
        [Fact]
        public async Task GetVolunteers_ShouldReturnOkResult()
        {
            var volunteers = _fixture.CreateMany<Volunteer>(2).AsQueryable();

            var mockDbSet = new MockDbSet<Volunteer>(volunteers);
            _mockContext.Setup(c => c.Volunteers).Returns(mockDbSet.Object);

            var controller = new VolunteerController(_mockContext.Object);

            var result = await controller.GetVolunteers() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var returnedVolunteers = result.Value as List<Volunteer>;
            Assert.NotNull(returnedVolunteers);
            Assert.Equal(2, returnedVolunteers.Count);
        }

        // tests to see if it can add a volunteer and return an ok result
        [Fact]
        public async Task AddVolunteer_ShouldReturnOkResult()
        {
            var volunteer = new Volunteer
            {
                Name = "John",
                Surname = "Doe",
                VolunteerDate = "2024-11-25",
                PhoneNumber = "1234567890",
                Email = "john.doe@example.com"
            };

            var volunteers = new List<Volunteer>().AsQueryable();
            var mockDbSet = new MockDbSet<Volunteer>(volunteers);
            _mockContext.Setup(c => c.Volunteers).Returns(mockDbSet.Object);

            var controller = new VolunteerController(_mockContext.Object);

            var result = await controller.AddVolunteer(volunteer) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        // tests to see if it can update a volunteer
        [Fact]
        public async Task UpdateVolunteer_ShouldReturnOkResult()
        {
            var volunteerId = 1;
            var existingVolunteer = new Volunteer
            {
                VolunteerId = volunteerId,
                Name = "John",
                Surname = "Doe",
                VolunteerDate = "2024-11-25",
                PhoneNumber = "1234567890",
                Email = "john.doe@example.com"
            };
            var updatedVolunteer = new Volunteer
            {
                Name = "John Updated",
                Surname = "Doe Updated",
                VolunteerDate = "2024-11-26",
                PhoneNumber = "0987654321",
                Email = "john.updated@example.com"
            };

            var volunteers = new List<Volunteer> { existingVolunteer }.AsQueryable();
            var mockDbSet = new MockDbSet<Volunteer>(volunteers);
            _mockContext.Setup(c => c.Volunteers).Returns(mockDbSet.Object);

            var controller = new VolunteerController(_mockContext.Object);

            var result = await controller.UpdateVolunteer(volunteerId, updatedVolunteer) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        // tests to see if it can delete a volunteer
        [Fact]
        public async Task DeleteVolunteer_ShouldReturnOkResult()
        {
            var volunteerId = 1;
            var volunteer = new Volunteer
            {
                VolunteerId = volunteerId,
                Name = "John",
                Surname = "Doe",
                VolunteerDate = "2024-11-25",
                PhoneNumber = "1234567890",
                Email = "john.doe@example.com"
            };

            var volunteers = new List<Volunteer> { volunteer }.AsQueryable();
            var mockDbSet = new MockDbSet<Volunteer>(volunteers);
            _mockContext.Setup(c => c.Volunteers).Returns(mockDbSet.Object);

            var controller = new VolunteerController(_mockContext.Object);

            var result = await controller.DeleteEvent(volunteerId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
    }
}

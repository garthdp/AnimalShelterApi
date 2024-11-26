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
using Microsoft.VisualStudio.TestPlatform;

namespace ApiTesting
{
    public class BoardingControllerTest
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

        public BoardingControllerTest()
        {
            _fixture = new Fixture();
            _mockContext = new Mock<WilDbContext>();
        }

        // tests to see if it can get the boarding requests and the correct number of boarding requests
        [Fact]
        public async Task GetBoardingRequests_ShouldReturnOkResult()
        {

            var boardings = _fixture.CreateMany<BoardingRequest>(2).AsQueryable();

            var mockDbSet = new MockDbSet<BoardingRequest>(boardings);
            _mockContext.Setup(c => c.BoardingRequests).Returns(mockDbSet.Object);

            var controller = new BoardingController(_mockContext.Object);

            var result = await controller.GetBoardingRequests() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var returnedRequests = result.Value as List<BoardingRequest>;
            Assert.NotNull(returnedRequests);
            Assert.Equal(2, returnedRequests.Count);
        }

        // tests to see if it can update a boarding request 
        [Fact]
        public async Task UpdateBoarding_ShouldReturnOkResult()
        {
            var boardingId = 1;
            var existingBoardingRequest = new BoardingRequest
            {
                BoardingId = 1,
                Breed = "Breed 1",
                EndDate = "2024-12-01",
                OwnerName = "John Doe",
                OwnerEmail = "john@example.com",
                PetName = "Pet 1",
                StartDate = "2024-11-25"
            };
            var updatedBoardingRequest = new BoardingRequest
            {
                Breed = "Breed Updated",
                EndDate = "2024-12-05",
                OwnerName = "John Updated",
                OwnerEmail = "john.updated@example.com",
                PetName = "Pet Updated",
                StartDate = "2024-11-28"
            };

            var boardingRequests = new List<BoardingRequest> { existingBoardingRequest }.AsQueryable();
            var mockDbSet = new MockDbSet<BoardingRequest>(boardingRequests);
            _mockContext.Setup(c => c.BoardingRequests).Returns(mockDbSet.Object);

            var controller = new BoardingController(_mockContext.Object);

            var result = await controller.UpdateBoarding(boardingId, updatedBoardingRequest) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        // tests to see if it can delete a boarding request
        [Fact]
        public async Task DeleteBoarding_ShouldReturnOkResult()
        {
            var boardingId = 1;
            var boardingRequests = new List<BoardingRequest>
            {
                new BoardingRequest { BoardingId = 1, Breed = "Breed 1", EndDate = "2024-12-01", OwnerName = "John Doe", OwnerEmail = "john@example.com", PetName = "Pet 1", StartDate = "2024-11-25" }
            }.AsQueryable();

            var mockDbSet = new MockDbSet<BoardingRequest>(boardingRequests);
            _mockContext.Setup(c => c.BoardingRequests).Returns(mockDbSet.Object);

            var controller = new BoardingController(_mockContext.Object);

            var result = await controller.Delete(boardingId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        // tests to see if it can post a new boarding request
        [Fact]
        public async Task PostBoardingRequest_ShouldReturnOkResult()
        {
            var boardingRequest = new BoardingRequest
            {
                Breed = "Breed 1",
                EndDate = "2024-12-01",
                OwnerName = "John Doe",
                OwnerEmail = "john@example.com",
                PetName = "Pet 1",
                StartDate = "2024-11-25"
            };

            var boardingRequests = new List<BoardingRequest>().AsQueryable();
            var mockDbSet = new MockDbSet<BoardingRequest>(boardingRequests);
            _mockContext.Setup(c => c.BoardingRequests).Returns(mockDbSet.Object);

            var controller = new BoardingController(_mockContext.Object);

            var result = await controller.Post(boardingRequest) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
    }
}

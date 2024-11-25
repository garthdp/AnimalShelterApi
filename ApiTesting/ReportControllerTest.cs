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
    public class ReportControllerTest
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
        public ReportControllerTest()
        {
            _fixture = new Fixture();
            _mockContext = new Mock<WilDbContext>();
        }

        [Fact]
        public async Task GetReports_ShouldReturnOkResult()
        {
            var reports = _fixture.CreateMany<Report>(2).AsQueryable();

            var mockDbSet = new MockDbSet<Report>(reports);
            _mockContext.Setup(c => c.Reports).Returns(mockDbSet.Object);

            var controller = new ReportController(_mockContext.Object);

            var result = await controller.GetReports() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var returnedReports = result.Value as List<Report>;
            Assert.NotNull(returnedReports);
            Assert.Equal(2, returnedReports.Count);
        }

        [Fact]
        public async Task UserReports_ShouldReturnOkResult()
        {
            var email = "john@example.com";
            var reports = new List<Report>
            {
                new Report { ReportId = 1, ContactInfo = "john@example.com", Description = "User Report 1", Location = "Location 1", Status = "Open" },
                new Report { ReportId = 2, ContactInfo = "john@example.com", Description = "User Report 2", Location = "Location 2", Status = "Closed" }
            }.AsQueryable();

            var mockDbSet = new MockDbSet<Report>(reports);
            _mockContext.Setup(c => c.Reports).Returns(mockDbSet.Object);

            var controller = new ReportController(_mockContext.Object);

            var result = await controller.UserReports(email) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var returnedReports = result.Value as List<Report>;
            Assert.NotNull(returnedReports);
            Assert.Equal(2, returnedReports.Count);
        }

        [Fact]
        public async Task DeleteReport_ShouldReturnOkResult()
        {
            var reportId = 1;
            var reports = new List<Report>
            {
                new Report { ReportId = 1, ContactInfo = "john@example.com", Description = "Report to Delete", Location = "Location 1", Status = "Open" }
            }.AsQueryable();

            var mockDbSet = new MockDbSet<Report>(reports);
            _mockContext.Setup(c => c.Reports).Returns(mockDbSet.Object);

            var controller = new ReportController(_mockContext.Object);

            var result = await controller.Delete(reportId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task PostReport_ShouldReturnOkResult()
        {
            var report = new Report { ReportId = 1, ContactInfo = "john@example.com", Description = "New Report", Location = "New Location", Status = "Open" };

            var reports = new List<Report>().AsQueryable();
            var mockDbSet = new MockDbSet<Report>(reports);
            _mockContext.Setup(c => c.Reports).Returns(mockDbSet.Object);

            var controller = new ReportController(_mockContext.Object);

            var result = await controller.Post(report) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
    }
}

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
    public class EventsControllerTest
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

        public EventsControllerTest()
        {
            _fixture = new Fixture();
            _mockContext = new Mock<WilDbContext>();
        }

        [Fact]
        public async Task GetEvents_ShouldReturnOkResult()
        {
            var events = _fixture.CreateMany<Event>(2).AsQueryable();

            var mockDbSet = new MockDbSet<Event>(events);
            _mockContext.Setup(c => c.Events).Returns(mockDbSet.Object);

            var controller = new EventController(_mockContext.Object);

            var result = await controller.GetEvents() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var returnedEvents = result.Value as List<Event>;
            Assert.NotNull(returnedEvents);
            Assert.Equal(2, returnedEvents.Count);
        }
        [Fact]
        public async Task UpdateEvent_ShouldReturnOkResult()
        {
            var eventId = 1;
            var existingEvent = new Event { EventId = 1, EventName = "Old Event", EventDescription = "Old Description", EventDate = "2024-12-01" };
            var updatedEvent = new Events { eventName = "Updated Event", eventDescription = "Updated Description", eventDate = "2024-12-02" };

            var events = new List<Event> { existingEvent }.AsQueryable();
            var mockDbSet = new MockDbSet<Event>(events);
            _mockContext.Setup(c => c.Events).Returns(mockDbSet.Object);

            var controller = new EventController(_mockContext.Object);

            var result = await controller.UpdateEvent(eventId, updatedEvent) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task DeleteEvent_ShouldReturnOkResult()
        {
            var eventId = 1;
            var events = new List<Event>
            {
                new Event { EventId = 1, EventName = "Event to Delete", EventDescription = "Description", EventDate = "2024-12-01" }
            }.AsQueryable();

            var mockDbSet = new MockDbSet<Event>(events);
            _mockContext.Setup(c => c.Events).Returns(mockDbSet.Object);

            var controller = new EventController(_mockContext.Object);

            var result = await controller.DeleteEvent(eventId) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
    }
}

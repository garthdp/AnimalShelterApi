using SPCAAPI.Controllers;

namespace ApiTesting
{
    public class EventsControllerTest
    {
        [Fact]
        public async Task TestGetMethodAsync()
        {
            var controller = new EventController();
            var result = await controller.GetEvents();
            Assert.NotNull(result);
        }
    }
}
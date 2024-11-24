using SPCAAPI.Controllers;

namespace ApiTesting
{
    public class EventsControllerTest
    {
        [Fact]
        public async Task TestGetMethodAsync()
        {
            var controller = new EventController();
            await controller.GetEvents();
        }
    }
}
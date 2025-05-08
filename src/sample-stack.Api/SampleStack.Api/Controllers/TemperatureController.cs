using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SampleStack.Api.Providers;
using System.Reactive.Subjects;

namespace SampleStack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemperatureController : ControllerBase
    {

        private ISocketDataProvider _socketDataProvider;

        public TemperatureController(ISocketDataProvider socketDataProvider)
        {
            this._socketDataProvider = socketDataProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return this.Ok(this._socketDataProvider.SocketData.Value);
        }
    }
}

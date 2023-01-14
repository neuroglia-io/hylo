using Hylo.Api.Core.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hylo.Api.Server.Endpoints
{

    [Route("test")]
    public class TestEndpoint
        : Controller
    {

        [HttpGet]
        [Authorize(AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Get()
        {
            return this.Ok();
        }

    }

}

using Microsoft.AspNetCore.Mvc;

namespace Hylo.Api.Server.Endpoints;


[ApiAuthorize, Route("test")]
public class TestEndpoint
    : Controller
{

    [HttpGet]
    public IActionResult Get()
    {
        return this.Ok();
    }

}

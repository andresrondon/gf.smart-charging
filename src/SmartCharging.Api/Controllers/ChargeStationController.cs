using Microsoft.AspNetCore.Mvc;
using SmartCharging.Api.Models;
using SmartCharging.Api.Models.Requests;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SmartCharging.Api.Controllers;

[ApiController]
[Route("stations")]
[ApiVersion("1.0")]
public class ChargeStationController : ControllerBase
{
    private readonly ILogger<ChargeStationController> _logger;

    public ChargeStationController(ILogger<ChargeStationController> logger)
    {
        _logger = logger;
    }

    [HttpGet, Route("{id}")]
    [ProducesResponseType(typeof(ChargeStation), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationException), (int)HttpStatusCode.PreconditionFailed)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    public Task<ChargeStation> GetAsync([FromRoute, NotNull] string id)
    {
        return Task.FromResult(new ChargeStation
        {
            Id = id,
            Name = id
        });
    }

    [HttpGet, Route("all")]
    [ProducesResponseType(typeof(IEnumerable<ChargeStation>), (int)HttpStatusCode.OK)]
    public Task<IActionResult> GetAllAsync()
    {
        var stations = new ChargeStation[]
        {
            new ChargeStation
            {
                Id = "abc123",
                Name = "ChargeStation 1"
            }
        };

        IActionResult result = new JsonResult(stations)
        {
            StatusCode = (int)HttpStatusCode.OK,
        };
        
        return Task.FromResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationException), (int)HttpStatusCode.PreconditionFailed)]
    public Task<IActionResult> CreateAsync([FromBody] ChargeStationCreateRequest request)
    {
        IActionResult result = Created("stations", request);

        return Task.FromResult(result);
    }

    [HttpPatch]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationException), (int)HttpStatusCode.PreconditionFailed)]
    public Task<IActionResult> UpdateAsync([FromBody] ChargeStationUpdateRequest request)
    {
        IActionResult result = Ok();

        return Task.FromResult(result);
    }

    [HttpDelete, Route("{id}")]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ValidationException), (int)HttpStatusCode.PreconditionFailed)]
    public Task<IActionResult> DeleteAsync([FromRoute, NotNull] string id)
    {
        IActionResult result = Ok();

        return Task.FromResult(result);
    }
}

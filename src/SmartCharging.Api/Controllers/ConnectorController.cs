using Microsoft.AspNetCore.Mvc;
using SmartCharging.Api.Models;
using SmartCharging.Api.Models.Requests;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SmartCharging.Api.Controllers;

[ApiController]
[Route("connectors")]
[ApiVersion("1.0")]
public class ConnectorController : ControllerBase
{
    private readonly ILogger<ConnectorController> _logger;

    public ConnectorController(ILogger<ConnectorController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationException), (int)HttpStatusCode.PreconditionFailed)]
    public Task<IActionResult> CreateAsync([FromBody] ConnectorCreateRequest request)
    {
        IActionResult result = Created("connectors", request);

        return Task.FromResult(result);
    }

    [HttpPatch]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationException), (int)HttpStatusCode.PreconditionFailed)]
    public Task<IActionResult> UpdateAsync([FromBody] ConnectorUpdateRequest request)
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

using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SmartCharging.Lib.Models;
using SmartCharging.Lib.Services.Connectors;
using SmartCharging.Lib.Constants;
using SmartCharging.Api.Requests;

namespace SmartCharging.Api.Controllers;

[ApiController]
[Route("groups/{groupId}/stations/{stationId}/connectors")]
[ApiVersion("1.0")]
public class ConnectorController : ControllerBase
{
    private readonly IConnectorService connectorService;

    public ConnectorController(IConnectorService connectorService)
    {
        this.connectorService = connectorService;
    }

    [HttpGet, Route("{connectorId}")]
    [ProducesResponseType(typeof(Connector), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute, NotNull] string groupId, [FromRoute, NotNull] string stationId, [FromRoute, NotNull] int connectorId)
    {
        var entity = await connectorService.FindAsync(groupId, stationId, connectorId);
        return new JsonResult(entity);
    }

    [HttpPost]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> CreateAsync(
        [FromRoute, NotNull] string groupId, 
        [FromRoute, NotNull] string stationId, 
        [FromBody] ConnectorCreateRequest request, 
        [FromQuery] string locationArea = Defaults.Location)
    {
        var entity = request.ToEntity();
        await connectorService.AddAsync(locationArea, groupId, stationId, entity);

        return Created("connectors", entity);
    }

    [HttpPatch, Route("{connectorId}")]
    [ProducesResponseType(typeof(Connector), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute, NotNull] string groupId, 
        [FromRoute, NotNull] string stationId, 
        [FromRoute, NotNull] int connectorId, 
        [FromBody] ConnectorUpdateRequest request, 
        [FromQuery] string locationArea = Defaults.Location)
    {
        var entity = await connectorService.FindAsync(groupId, stationId, connectorId);

        entity.MaxCurrentInAmps = request.MaxCurrentInAmps ?? entity.MaxCurrentInAmps;

        await connectorService.UpdateAsync(locationArea, groupId, stationId, entity);

        return new JsonResult(entity);
    }

    [HttpDelete, Route("{connectorId}")]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute, NotNull] string groupId, 
        [FromRoute, NotNull] string stationId, 
        [FromRoute, NotNull] int connectorId)
    {
        await connectorService.DeleteAsync(groupId, stationId, connectorId);
        return Ok();
    }
}

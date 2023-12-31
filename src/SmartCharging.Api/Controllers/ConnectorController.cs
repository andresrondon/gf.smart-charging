using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SmartCharging.Lib.Models;
using SmartCharging.Lib.Services.Connectors;
using SmartCharging.Api.Requests;

namespace SmartCharging.Api.Controllers;

/// <summary>
/// Controller for interacting with <see cref="Connector"/> domain model.
/// </summary>
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

    /// <summary>
    /// Performs a query to get a specific <see cref="Connector"/>.
    /// </summary>
    [HttpGet, Route("{connectorId}")]
    [ProducesResponseType(typeof(Connector), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute, NotNull] string groupId, [FromRoute, NotNull] string stationId, [FromRoute, NotNull] int connectorId)
    {
        var entity = await connectorService.FindAsync(groupId, stationId, connectorId);
        return new JsonResult(entity);
    }

    /// <summary>
    /// Sends a command to create a new <see cref="Connector"/>.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> CreateAsync(
        [FromRoute, NotNull] string groupId, 
        [FromRoute, NotNull] string stationId, 
        [FromBody] ConnectorCreateRequest request)
    {
        var entity = request.ToEntity();
        await connectorService.AddAsync(entity, groupId, stationId);

        return Created("connectors", entity);
    }

    /// <summary>
    /// Updates the specified <see cref="Connector"/>.
    /// </summary>
    [HttpPatch, Route("{connectorId}")]
    [ProducesResponseType(typeof(Connector), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute, NotNull] string groupId, 
        [FromRoute, NotNull] string stationId, 
        [FromRoute, NotNull] int connectorId, 
        [FromBody] ConnectorUpdateRequest request)
    {
        var entity = await connectorService.FindAsync(groupId, stationId, connectorId);

        entity.MaxCurrentInAmps = request.MaxCurrentInAmps ?? entity.MaxCurrentInAmps;

        await connectorService.UpdateAsync(entity, groupId, stationId);

        return new JsonResult(entity);
    }

    /// <summary>
    /// Deletes a <see cref="Connector"/>.
    /// </summary>
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

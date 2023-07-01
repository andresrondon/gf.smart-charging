using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SmartCharging.Lib.Models;
using SmartCharging.Api.Models.Requests;
using SmartCharging.Lib.Services.Groups;
using SmartCharging.Lib.Services.Connectors;

namespace SmartCharging.Api.Controllers;

[ApiController]
[Route("connectors")]
[ApiVersion("1.0")]
public class ConnectorController : ControllerBase
{
    private readonly IConnectorService connectorService;
    private readonly IGroupService groupService;

    public ConnectorController(IConnectorService connectorService, IGroupService groupService)
    {
        this.connectorService = connectorService;
        this.groupService = groupService;
    }

    [HttpGet, Route("{id}")]
    [ProducesResponseType(typeof(Connector), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute, NotNull] string id, [FromQuery] string chargeStationId)
    {
        var entity = await connectorService.FindAsync(id, chargeStationId);
        return entity is not null ? new JsonResult(entity) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> CreateAsync([FromBody] ConnectorCreateRequest request)
    {
        // Validations
        var parentGroup = await groupService.FindAsync(request.GroupId);
        var parentStation = parentGroup?.ChargeStations.FirstOrDefault(cs => cs.Id == request.ChargeStationId);
        
        if (!request.Validate(parentGroup, parentStation, out IEnumerable<string> errors))
        {
            return StatusCode((int)HttpStatusCode.PreconditionFailed, new { errors });
        }

        // Save to DB
        var entity = request.ToEntity();
        await connectorService.AddAsync(entity);

        return Created("connectors", entity);
    }

    [HttpPatch, Route("{id}")]
    [ProducesResponseType(typeof(Connector), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> UpdateAsync([FromRoute, NotNull] string id, [FromQuery] string chargeStationId, [FromBody] ConnectorUpdateRequest request)
    {
        var entity = await connectorService.FindAsync(id, chargeStationId);

        if (entity is null)
        {
            return NotFound();
        }

        entity.MaxCurrentInAmps = request.MaxCurrentInAmps ?? entity.MaxCurrentInAmps;

        await connectorService.UpdateAsync(entity);

        return new JsonResult(entity);
    }

    [HttpDelete, Route("{id}")]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> DeleteAsync([FromRoute, NotNull] string id, [FromQuery] string chargeStationId)
    {
        await connectorService.DeleteAsync(id, chargeStationId);
        return Ok();
    }
}

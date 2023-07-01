using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SmartCharging.Lib.Models;
using SmartCharging.Lib.Repositories.Connectors;
using SmartCharging.Api.Models.Requests;

namespace SmartCharging.Api.Controllers;

[ApiController]
[Route("connectors")]
[ApiVersion("1.0")]
public class ConnectorController : ControllerBase
{
    private readonly IConnectorRepository repository;

    public ConnectorController(IConnectorRepository repository)
    {
        this.repository = repository;
    }

    [HttpGet, Route("{id}")]
    [ProducesResponseType(typeof(Connector), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute, NotNull] string id, [FromQuery] string chargeStationId)
    {
        var entity = await repository.FindAsync(id, chargeStationId);
        return entity is not null ? new JsonResult(entity) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> CreateAsync([FromBody] ConnectorCreateRequest request)
    {
        var entity = request.ToEntity();
        await repository.AddAsync(entity);

        return Created("connectors", entity);
    }

    [HttpPatch, Route("{id}")]
    [ProducesResponseType(typeof(Connector), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> UpdateAsync([FromRoute, NotNull] string id, [FromQuery] string chargeStationId, [FromBody] ConnectorUpdateRequest request)
    {
        var entity = await repository.FindAsync(id, chargeStationId);

        if (entity is null)
        {
            return NotFound();
        }

        entity.MaxCurrentInAmps = request.MaxCurrentInAmps ?? entity.MaxCurrentInAmps;

        await repository.UpdateAsync(entity);

        return new JsonResult(entity);
    }

    [HttpDelete, Route("{id}")]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> DeleteAsync([FromRoute, NotNull] string id, [FromQuery] string chargeStationId)
    {
        await repository.DeleteAsync(id, chargeStationId);
        return Ok();
    }
}

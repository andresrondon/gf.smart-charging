using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SmartCharging.Lib.Models;
using SmartCharging.Lib.Repositories.ChargeStations;
using SmartCharging.Api.Models.Requests;

namespace SmartCharging.Api.Controllers;

[ApiController]
[Route("stations")]
[ApiVersion("1.0")]
public class ChargeStationController : ControllerBase
{
    private readonly IChargeStationRepository repository;

    public ChargeStationController(IChargeStationRepository repository)
    {
        this.repository = repository;
    }

    [HttpGet, Route("{id}")]
    [ProducesResponseType(typeof(ChargeStation), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute, NotNull] string id, [FromQuery] string groupId)
    {
        var entity = await repository.FindAsync(id, groupId);
        return entity is not null ? new JsonResult(entity) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> CreateAsync([FromBody] ChargeStationCreateRequest request)
    {
        var entity = request.ToEntity();
        await repository.AddAsync(entity);

        return Created("stations", entity);
    }

    [HttpPatch, Route("{id}")]
    [ProducesResponseType(typeof(ChargeStation), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> UpdateAsync([FromRoute, NotNull] string id, [FromBody] ChargeStationUpdateRequest request)
    {
        var entity = await repository.FindAsync(id, "default-partition");

        if (entity is null)
        {
            return NotFound();
        }

        entity.Name = request.Name ?? entity.Name;

        await repository.UpdateAsync(entity);

        return new JsonResult(entity);
    }

    [HttpDelete, Route("{id}")]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> DeleteAsync([FromRoute, NotNull] string id, [FromQuery] string partitionKey = "default-partition")
    {
        await repository.DeleteAsync(id, partitionKey);
        return Ok();
    }
}

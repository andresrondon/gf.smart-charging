using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SmartCharging.Lib.Models;
using SmartCharging.Api.Models.Requests;
using SmartCharging.Lib.Services.ChargeStations;
using SmartCharging.Lib.Constants;
using SmartCharging.Lib.Exceptions;
using SmartCharging.Lib.Services.Connectors;

namespace SmartCharging.Api.Controllers;

[ApiController]
[Route("groups/{groupId}/stations")]
[ApiVersion("1.0")]
public class ChargeStationController : ControllerBase
{
    private readonly IChargeStationService stationService;

    public ChargeStationController(IChargeStationService stationService)
    {
        this.stationService = stationService;
    }

    [HttpGet, Route("{stationId}")]
    [ProducesResponseType(typeof(ChargeStation), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute, NotNull] string groupId, [FromRoute, NotNull] string stationId)
    {
        var entity = await stationService.FindAsync(groupId, stationId);
        return entity is not null ? new JsonResult(entity) : NotFound(new { groupId, stationId });
    }

    [HttpPost]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> CreateAsync([FromRoute, NotNull] string groupId, [FromBody] ChargeStationCreateRequest request,
        [FromQuery] string locationArea = Defaults.Location)
    {
        var entity = request.ToEntity(groupId);

        try
        {
            await stationService.AddAsync(locationArea, entity);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }

        return Created("stations", entity);
    }

    [HttpPatch, Route("{stationId}")]
    [ProducesResponseType(typeof(ChargeStation), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> UpdateAsync([FromRoute, NotNull] string groupId, [FromRoute, NotNull] string stationId, [FromBody] ChargeStationUpdateRequest request)
    {
        var entity = await stationService.FindAsync(groupId, stationId);

        if (entity is null)
        {
            return NotFound();
        }

        entity.Name = request.Name ?? entity.Name;

        await stationService.UpdateAsync(entity);

        return new JsonResult(entity);
    }

    [HttpDelete, Route("{stationId}")]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> DeleteAsync([FromRoute, NotNull] string groupId, [FromRoute, NotNull] string stationId)
    {
        await stationService.DeleteAsync(groupId, stationId);
        return Ok();
    }
}

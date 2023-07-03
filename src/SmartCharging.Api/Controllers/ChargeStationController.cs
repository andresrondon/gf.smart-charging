using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SmartCharging.Lib.Models;
using SmartCharging.Lib.Services.ChargeStations;
using SmartCharging.Lib.Constants;
using SmartCharging.Api.Requests;

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
        return new JsonResult(entity);
    }

    [HttpPost]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> CreateAsync([FromRoute, NotNull] string groupId, [FromBody] ChargeStationCreateRequest request,
        [FromQuery] string locationArea = Defaults.Location)
    {
        var entity = request.ToEntity(groupId);
        await stationService.AddAsync(locationArea, entity);
        
        return Created("stations", entity);
    }

    [HttpPatch, Route("{stationId}")]
    [ProducesResponseType(typeof(ChargeStation), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> UpdateAsync([FromRoute, NotNull] string groupId, [FromRoute, NotNull] string stationId, [FromBody] ChargeStationUpdateRequest request,
        [FromQuery] string locationArea = Defaults.Location)
    {
        var entity = await stationService.FindAsync(groupId, stationId);

        entity.Name = request.Name ?? entity.Name;
        entity.Connectors = request.Connectors ?? entity.Connectors;

        await stationService.UpdateAsync(locationArea, entity);

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

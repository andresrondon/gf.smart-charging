using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SmartCharging.Lib.Models;
using SmartCharging.Api.Models.Requests;
using SmartCharging.Lib.Services.ChargeStations;
using SmartCharging.Lib.Services.Groups;

namespace SmartCharging.Api.Controllers;

[ApiController]
[Route("stations")]
[ApiVersion("1.0")]
public class ChargeStationController : ControllerBase
{
    private readonly IChargeStationService stationService;
    private readonly IGroupService groupService;

    public ChargeStationController(IChargeStationService stationService, IGroupService groupService)
    {
        this.stationService = stationService;
        this.groupService = groupService;
    }

    [HttpGet, Route("{id}")]
    [ProducesResponseType(typeof(ChargeStation), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute, NotNull] string id, [FromQuery] string groupId)
    {
        var entity = await stationService.FindAsync(id, groupId);
        return entity is not null ? new JsonResult(entity) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> CreateAsync([FromBody] ChargeStationCreateRequest request)
    {
        // Validations
        var parentGroup = await groupService.FindAsync(request.GroupId);
        if (!request.Validate(parentGroup, out IEnumerable<string> errors))
        {
            return StatusCode((int)HttpStatusCode.PreconditionFailed, new { errors });
        }

        // Save to DB
        var entity = request.ToEntity();
        await stationService.AddAsync(entity);

        return Created("stations", entity);
    }

    [HttpPatch, Route("{id}")]
    [ProducesResponseType(typeof(ChargeStation), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> UpdateAsync([FromRoute, NotNull] string id, [FromBody] ChargeStationUpdateRequest request)
    {
        var entity = await stationService.FindAsync(id, "default-partition");

        if (entity is null)
        {
            return NotFound();
        }

        entity.Name = request.Name ?? entity.Name;

        await stationService.UpdateAsync(entity);

        return new JsonResult(entity);
    }

    [HttpDelete, Route("{id}")]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> DeleteAsync([FromRoute, NotNull] string id, [FromQuery] string partitionKey = "default-partition")
    {
        await stationService.DeleteAsync(id, partitionKey);
        return Ok();
    }
}

using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SmartCharging.Lib.Models;
using SmartCharging.Api.Models.Requests;
using SmartCharging.Lib.Services.Groups;
using SmartCharging.Lib.Services.ChargeStations;

namespace SmartCharging.Api.Controllers;

[ApiController]
[Route("groups")]
[ApiVersion("1.0")]
public class GroupController : ControllerBase
{
    private readonly IGroupService groupService;
    private readonly IChargeStationService stationService;

    public GroupController(IGroupService groupService, IChargeStationService stationService)
    {
        this.groupService = groupService;
        this.stationService = stationService;
    }

    [HttpGet, Route("{id}")]
    [ProducesResponseType(typeof(Group), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute, NotNull] string id)
    {
        var entity = await groupService.FindAsync(id);
        return entity is not null ? new JsonResult(entity) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> CreateAsync([FromBody] GroupCreateRequest request)
    {
        // Save to DB
        var entity = request.ToEntity();
        await groupService.AddAsync(entity);

        return Created("groups", entity);
    }

    [HttpPatch, Route("{id}")]
    [ProducesResponseType(typeof(Group), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> UpdateAsync([FromRoute, NotNull] string id, [FromBody] GroupUpdateRequest request)
    {
        var entity = await groupService.FindAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        entity.Name = request.Name ?? entity.Name;
        entity.CapacityInAmps = request.CapacityInAmps ?? entity.CapacityInAmps;

        await groupService.UpdateAsync(entity);
        
        return new JsonResult(entity);
    }

    [HttpDelete, Route("{id}")]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> DeleteAsync([FromRoute, NotNull] string id)
    {
        await groupService.DeleteAsync(id);
        await stationService.BulkDelete(groupId: id);
        return Ok();
    }
}

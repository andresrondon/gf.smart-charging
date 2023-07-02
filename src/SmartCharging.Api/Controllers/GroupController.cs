using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SmartCharging.Lib.Models;
using SmartCharging.Api.Models.Requests;
using SmartCharging.Lib.Services.Groups;
using SmartCharging.Lib.Constants;
using SmartCharging.Lib.Services.Connectors;
using SmartCharging.Lib.Exceptions;

namespace SmartCharging.Api.Controllers;

[ApiController]
[Route("groups")]
[ApiVersion("1.0")]
public class GroupController : ControllerBase
{
    private readonly IGroupService groupService;

    public GroupController(IGroupService groupService)
    {
        this.groupService = groupService;
    }

    [HttpGet, Route("{id}")]
    [ProducesResponseType(typeof(Group), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute, NotNull] string id, [FromQuery] string locationArea = Defaults.Location)
    {
        var entity = await groupService.FindAsync(locationArea, id);
        return entity is not null ? new JsonResult(entity) : NotFound(new { groupId = id });
    }

    [HttpPost]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> CreateAsync([FromBody] GroupCreateRequest request)
    {
        var entity = request.ToEntity();
        await groupService.AddAsync(entity);

        return Created("groups", entity);
    }

    [HttpPatch, Route("{id}")]
    [ProducesResponseType(typeof(Group), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> UpdateAsync([FromRoute, NotNull] string id, [FromBody] GroupUpdateRequest request, [FromQuery] string locationArea = Defaults.Location)
    {
        var entity = await groupService.FindAsync(locationArea, id);

        if (entity is null)
        {
            return NotFound(new { groupId = id });
        }

        entity.Name = request.Name ?? entity.Name;
        entity.CapacityInAmps = request.CapacityInAmps ?? entity.CapacityInAmps;

        try
        {
            await groupService.UpdateAsync(entity);
        }
        catch (ValidationException ex)
        {
            return StatusCode((int)HttpStatusCode.PreconditionFailed, new { message = ex.Message, errors = ex.Errors });
        }

        return new JsonResult(entity);
    }

    [HttpDelete, Route("{id}")]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> DeleteAsync([FromRoute, NotNull] string id, [FromQuery] string locationArea = Defaults.Location)
    {
        await groupService.DeleteAsync(locationArea, id);
        return Ok();
    }
}

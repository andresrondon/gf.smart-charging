using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SmartCharging.Lib.Models;
using SmartCharging.Lib.Services.Groups;
using SmartCharging.Api.Requests;

namespace SmartCharging.Api.Controllers;

/// <summary>
/// Controller for interacting with <see cref="Group"/> domain model.
/// </summary>
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

    /// <summary>
    /// Performs a query to get a specific <see cref="Group"/>.
    /// </summary>
    [HttpGet, Route("{id}")]
    [ProducesResponseType(typeof(Group), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute, NotNull] string id)
    {
        var entity = await groupService.FindAsync(id);
        return new JsonResult(entity);
    }

    /// <summary>
    /// Sends a command to create a new <see cref="Group"/>.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> CreateAsync([FromBody] GroupCreateRequest request)
    {
        var entity = request.ToEntity();
        await groupService.AddAsync(entity);

        return Created("groups", entity);
    }

    /// <summary>
    /// Updates the specified <see cref="Group"/>.
    /// </summary>
    [HttpPatch, Route("{id}")]
    [ProducesResponseType(typeof(Group), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> UpdateAsync([FromRoute, NotNull] string id, [FromBody] GroupUpdateRequest request)
    {
        var entity = await groupService.FindAsync(id);

        entity.Name = request.Name ?? entity.Name;
        entity.CapacityInAmps = request.CapacityInAmps ?? entity.CapacityInAmps;

        await groupService.UpdateAsync(entity);

        return new JsonResult(entity);
    }

    /// <summary>
    /// Deletes a <see cref="Group"/>.
    /// </summary>
    [HttpDelete, Route("{id}")]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> DeleteAsync([FromRoute, NotNull] string id)
    {
        await groupService.DeleteAsync(id);
        return Ok();
    }
}

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SmartCharging.Lib.Models;
using SmartCharging.Lib.Repositories.Groups;
using SmartCharging.Api.Models.Requests;

namespace SmartCharging.Api.Controllers;

[ApiController]
[Route("groups")]
[ApiVersion("1.0")]
public class GroupController : ControllerBase
{
    private readonly IGroupRepository repository;

    public GroupController(IGroupRepository repository)
    {
        this.repository = repository;
    }

    [HttpGet, Route("{id}")]
    [ProducesResponseType(typeof(Group), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute, NotNull] string id, [FromQuery] string partitionKey = "default-partition")
    {
        var entity = await repository.FindAsync(id, partitionKey);
        return entity is not null ? new JsonResult(entity) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> CreateAsync([FromBody] GroupCreateRequest request)
    {
        var entity = request.ToEntity();
        await repository.AddAsync(entity);

        return Created("groups", entity);
    }

    [HttpPatch, Route("{id}")]
    [ProducesResponseType(typeof(Group), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.PreconditionFailed)]
    public async Task<IActionResult> UpdateAsync([FromRoute, NotNull] string id, [FromBody] GroupUpdateRequest request, [FromQuery] string partitionKey = "default-partition")
    {
        var entity = await repository.FindAsync(id, partitionKey);

        if (entity is null)
        {
            return NotFound();
        }

        entity.Name = request.Name ?? entity.Name;
        entity.CapacityInAmps = request.CapacityInAmps ?? entity.CapacityInAmps;

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

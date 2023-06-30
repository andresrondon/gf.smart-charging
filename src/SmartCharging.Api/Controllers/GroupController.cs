using Microsoft.AspNetCore.Mvc;
using SmartCharging.Lib.Models;
using SmartCharging.Api.Models.Requests;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SmartCharging.Api.Controllers;

[ApiController]
[Route("groups")]
[ApiVersion("1.0")]
public class GroupController : ControllerBase
{
    private readonly ILogger<GroupController> _logger;

    public GroupController(ILogger<GroupController> logger)
    {
        _logger = logger;
    }

    [HttpGet, Route("{id}")]
    [ProducesResponseType(typeof(Group), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationException), (int)HttpStatusCode.PreconditionFailed)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    public Task<Group> GetAsync([FromRoute, NotNull] string id)
    {
        return Task.FromResult(new Group
        {
            Id = id,
            Name = id
        });
    }

    [HttpGet, Route("all")]
    [ProducesResponseType(typeof(IEnumerable<Group>), (int)HttpStatusCode.OK)]
    public Task<IActionResult> GetAllAsync()
    {
        var groups = new Group[]
        {
            new Group
            {
                Id = "abc123",
                Name = "Group 1"
            }
        };

        IActionResult result = new JsonResult(groups)
        {
            StatusCode = (int)HttpStatusCode.OK,
        };
        
        return Task.FromResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationException), (int)HttpStatusCode.PreconditionFailed)]
    public Task<IActionResult> CreateAsync([FromBody] GroupCreateRequest request)
    {
        IActionResult result = Created("groups", request);

        return Task.FromResult(result);
    }

    [HttpPatch]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationException), (int)HttpStatusCode.PreconditionFailed)]
    public Task<IActionResult> UpdateAsync([FromBody] GroupUpdateRequest request)
    {
        IActionResult result = Ok();

        return Task.FromResult(result);
    }

    [HttpDelete, Route("{id}")]
    [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ValidationException), (int)HttpStatusCode.PreconditionFailed)]
    public Task<IActionResult> DeleteAsync([FromRoute, NotNull] string id)
    {
        IActionResult result = Ok();

        return Task.FromResult(result);
    }
}

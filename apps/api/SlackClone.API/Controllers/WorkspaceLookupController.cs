using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlackClone.Application.Features.Workspaces.GetWorkspaceBySlug;

namespace SlackClone.API.Controllers;

[ApiController]
[Route("api/v1/workspaces/lookup")]
public sealed class WorkspaceLookupController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpGet("by-slug/{slug}")]
    public async Task<IActionResult> BySlug(string slug, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetWorkspaceBySlugQuery(slug), cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error?.Code switch
            {
                "WORKSPACE_NOT_FOUND" => NotFound(result.Error),
                "VALIDATION_ERROR" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        return Ok(result.Value);
    }
}


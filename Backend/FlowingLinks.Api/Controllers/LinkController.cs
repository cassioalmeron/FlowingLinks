using FlowingLinks.Core.Models;
using FlowingLinks.Core.Services;
using FlowingLinks.Core;
using FlowingLinks.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FlowingLinks.Api.Controllers;

[Route("[controller]")]
public class LinkController : AuthorizeController
{
    private readonly ILogger<LinkController> _logger;
    private readonly LinkService _linkService;

    public LinkController(ILogger<LinkController> logger, LinkService linkService) : base(logger)
    {
        _logger = logger;
        _linkService = linkService;
    }

    /// <summary>
    /// Get all links for the current user
    /// </summary>
    /// <returns>List of all links for the current user</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LinkDto>>> GetAll()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var links = await _linkService.GetByUserId(currentUserId);
            return Ok(links);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt");
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving links for user");
            return StatusCode(500, "An error occurred while retrieving links");
        }
    }

    /// <summary>
    /// Get link by ID for the current user
    /// </summary>
    /// <param name="id">Link ID</param>
    /// <returns>Link if found, NotFound if not found</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Link>> GetById(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var link = await _linkService.GetById(id, currentUserId);

            if (link == null)
                return NotFound($"Link with ID {id} not found");

            return Ok(link);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt");
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving link with ID {LinkId}", id);
            return StatusCode(500, "An error occurred while retrieving the link");
        }
    }

    /// <summary>
    /// Get links by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of links for the specified user</returns>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<LinkDto>>> GetByUserId(int userId)
    {
        try
        {
            var links = await _linkService.GetByUserId(userId);
            return Ok(links);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving links for user {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving user links");
        }
    }

    /// <summary>
    /// Create a new link
    /// </summary>
    /// <param name="dto">Link data</param>
    /// <returns>Created link with ID</returns>
    [HttpPost]
    public async Task<ActionResult<Link>> Create([FromBody] LinkDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = GetCurrentUserId();
            await _linkService.Save(dto, currentUserId);

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        catch (FlowingLinksException ex)
        {
            _logger.LogWarning(ex, "Business rule violation while creating link");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating link");
            return StatusCode(500, "An error occurred while creating the link");
        }
    }

    /// <summary>
    /// Update an existing link
    /// </summary>
    /// <param name="id">Link ID</param>
    /// <param name="dto">Updated link data</param>
    /// <returns>Updated link</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<Link>> Update(int id, [FromBody] LinkDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = GetCurrentUserId();
            var existingLink = await _linkService.GetById(id, currentUserId);
            if (existingLink == null)
                return NotFound($"Link with ID {id} not found");

            dto.Id = id;

            await _linkService.Save(dto, currentUserId);

            return Ok(dto);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt");
            return Unauthorized(ex.Message);
        }
        catch (FlowingLinksException ex)
        {
            _logger.LogWarning(ex, "Business rule violation while updating link {LinkId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating link {LinkId}", id);
            return StatusCode(500, "An error occurred while updating the link");
        }
    }

    /// <summary>
    /// Delete a link
    /// </summary>
    /// <param name="id">Link ID</param>
    /// <returns>No content if deleted, NotFound if not found</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var deleted = await _linkService.Delete(id, currentUserId);

            if (!deleted)
                return NotFound($"Link with ID {id} not found");

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt");
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting link {LinkId}", id);
            return StatusCode(500, "An error occurred while deleting the link");
        }
    }

    /// <summary>
    /// Check if link exists for the current user
    /// </summary>
    /// <param name="id">Link ID</param>
    /// <returns>True if link exists, false otherwise</returns>
    [HttpGet("exists/{id}")]
    public async Task<ActionResult<bool>> LinkExists(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var exists = await _linkService.LinkExists(id, currentUserId);
            return Ok(exists);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt");
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if link {LinkId} exists", id);
            return StatusCode(500, "An error occurred while checking link existence");
        }
    }

    /// <summary>
    /// Update the favorite status of a link
    /// </summary>
    /// <param name="id">Link ID</param>
    /// <param name="favorite">Favorite status to set</param>
    /// <returns>Updated link if successful, NotFound if link not found</returns>
    [HttpPatch("{id}/favorite")]
    public async Task<ActionResult> UpdateFavorite(int id, [FromBody] bool favorite)

    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var updated = await _linkService.UpdateFavorite(id, currentUserId, favorite);

            if (!updated)
                return NotFound($"Link with ID {id} not found");

            return Ok();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt");
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating favorite status for link {LinkId}", id);
            return StatusCode(500, "An error occurred while updating the favorite status");
        }
    }
}
using FlowingLinks.Core.Models;
using FlowingLinks.Core.Services;
using FlowingLinks.Core;
using FlowingLinks.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using FlowingLinks.Api.Services;

namespace FlowingLinks.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly LoginService _loginService;
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService, ILogger<AuthController> logger, LoginService loginService)
        {
            _jwtService = jwtService;
            _logger = logger;
            _loginService = loginService;
        }

        /// <summary>
        /// Authenticate user with username and password
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>User information if authentication successful</returns>
        [HttpPost]
        public async Task<ActionResult<LoginResponseDto>> Authenticate([FromBody] LoginRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _loginService.Execute(request.Username, request.Password);

                var token = _jwtService.GenerateToken(
                    userId: user.Id.ToString(),
                    username: user.Username
                );

                var response = new LoginResponseDto
                {
                    Name = user.Name,
                    IsAdmin = user.Id == 1,
                    Token = token
                };

                _logger.LogInformation("User {Username} authenticated successfully", request.Username);

                return Ok(response);
            }
            catch (FlowingLinksException ex)
            {
                _logger.LogWarning("Authentication failed for username {Username}: {Message}", 
                    request?.Username ?? "unknown", ex.Message);
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication for username {Username}", 
                    request?.Username ?? "unknown");
                return StatusCode(500, "An error occurred during authentication");
            }
        }
    }
} 
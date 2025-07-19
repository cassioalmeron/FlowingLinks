namespace FlowingLinks.Api.DTOs;

public class LoginResponseDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;
    public string Token { get; set; } = string.Empty;
}
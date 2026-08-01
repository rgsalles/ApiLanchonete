using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiLanchonete.Authentication;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<TokenResponseDto>> Register(RegisterDto dto)
    {
        var response = await authService.Register(dto);

        if (response is null)
            return Conflict(new { Message = "There is already a user registered with this e-mail." });
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login(LoginDto dto)
    {
        var response = await authService.Login(dto);

        if (response is null)
            return Unauthorized(new {Message = "E-mail or password invalid."});

        return Ok(response);
    }
}
using Microsoft.AspNetCore.Mvc;

namespace ApiLanchonete.Authentication;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login(LoginDto dto)
    {
        var response = await authService.Login(dto);

        if (response is null)
            return Unauthorized("E-mail or password invalid.");

        return Ok(response);
    }
}
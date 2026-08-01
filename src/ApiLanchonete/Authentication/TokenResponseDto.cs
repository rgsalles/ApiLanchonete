namespace ApiLanchonete.Authentication;

public class TokenResponseDto
{
    public required string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
}
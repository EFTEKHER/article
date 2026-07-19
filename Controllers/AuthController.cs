using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using article.Dtos;
using article.Models;
using article.Repositories.Interfaces;
using article.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;


namespace article.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _users;
        private readonly IRefreshTokenRepository _refresh;
        private readonly ITokenService _tokens;
        private readonly IAuthCookieService _cookies;

        public AuthController(IUserRepository users, IRefreshTokenRepository refresh, ITokenService tokens, IAuthCookieService cookies)
        {
            _users = users;
            _refresh = refresh;
            _tokens = tokens;
            _cookies= cookies;

        }

        private async Task IssueTokens(User user)
        {
            var access= _tokens.CreateAccessToken(user);
            var (refresh, refreshHash)=_tokens.CreateRefreshToken();
            await _refresh.SaveAsync(user.Id, refreshHash, _tokens.RefreshTokenExpiry(), Ip());
            _cookies.SetAuthCookies(Response,access,_tokens.AccessTokenExpiry(), refresh, _tokens.RefreshTokenExpiry());
        }
        private string? Ip() =>HttpContext.Connection.RemoteIpAddress?.ToString();

        [HttpPost("register")]
        public async Task <IActionResult> Register(RegisterDto dto)
        {
            var user = new User
            {
                Username= dto.Username,
                Email=dto.EmailAddress,
                PasswordHash=BCrypt.Net.BCrypt.HashPassword(dto.Password)

            };
            var newId=await _users.RegisterAsync(user);

            if(newId== -1)
            {
                return Conflict(new { message = "Username or email exists" });

            }
            user.Id = newId;
            await IssueTokens(user);

            return Ok(new UserDto { Id=user.Id, Username= user.Username });
        }

        [HttpPost("login")]

        public async Task<IActionResult>Login(LoginDto dto)
        {
            var user= await _users.GetByUsernameAsync(dto.Username);
            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return Unauthorized(new { message = "invalid credentials" });
            await IssueTokens(user);
            return Ok(new UserDto { Id=user.Id, 
            Username=user.Username});

        }

        [HttpPost("refresh")]

        public async Task<IActionResult> Refresh()
        {
            var incoming = _cookies.ReadRefreshToken(Request);
            if (incoming is null) return Unauthorized();


            var hash = _tokens.Hash(incoming);
            var stored=await _refresh.GetAsync(hash);
            if(stored is null) return Unauthorized();

            if(stored.RevokedAt is not null)
            {
                await _refresh.RevokeAllForUserAsync(stored.UserId, Ip());
                _cookies.ClearAuthCookies(Response);
                return Unauthorized(new {messa})

            }
        }

    }
}

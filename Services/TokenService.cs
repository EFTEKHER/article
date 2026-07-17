using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using article.Models;
using Microsoft.IdentityModel.Tokens;


namespace article.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateAccessToken(User user)
        {
            var jwt= _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt.GetSection("Key").Value!)
                );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var token= new JwtSecurityToken(issuer: jwt.GetSection("Issuer").Value,
                audience: jwt.GetSection("Audience").Value,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public (string token, string tokenHash) CreateRefreshToken()
        {
           var bytes=RandomNumberGenerator.GetBytes(64);
            var token = Convert.ToBase64String(bytes);
            var tokenHash = Hash(token);
            return (token, tokenHash);
        }

        public string Hash(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }


        public DateTime AccessTokenExpiry() => DateTime.UtcNow.AddMinutes(Convert.ToInt32(_config["Jwt:AccessTokenMinutes"]));
        public DateTime RefreshTokenExpiry() => DateTime.UtcNow.AddDays(Convert.ToInt32(_config["Jwt:RefreshTokenDays"]));
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Public.Security
{
    public class JwtSessionService
    {
        private readonly PlayIntegrityOptions _options;
        private readonly byte[] _key;

        public JwtSessionService(IOptions<PlayIntegrityOptions> options)
        {
            _options = options.Value;
            _key = Encoding.UTF8.GetBytes(_options.SessionSecret);
        }

        public string CreateToken(string subject)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(JwtRegisteredClaimNames.Sub, subject) }),
                Expires = DateTime.UtcNow.AddMinutes(_options.SessionMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256)
            });
            return handler.WriteToken(token);
        }

        public bool Validate(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                }, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

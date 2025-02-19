using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.CommonLibrary.Security
{
    public interface ITokenValidator
    {
        (ClaimsPrincipal, SecurityToken) ValidateToken(string token);
        bool TryValidateToken(string token, out ClaimsPrincipal principal);
        string GenerateAccessToken(IEnumerable<Claim> claims);
    }

    public class TokenValidator : ITokenValidator
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly TokenValidationParameters _validationParameters;
        private readonly JwtOptions _jwtOptions;

        public TokenValidator(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
            _tokenHandler = new JwtSecurityTokenHandler();
            _validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Convert.FromBase64String(_jwtOptions.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };
        }

        public (ClaimsPrincipal, SecurityToken) ValidateToken(string token)
        {
            var principal = _tokenHandler.ValidateToken(token,
                _validationParameters, out SecurityToken validatedToken);

            return (principal, validatedToken);
        }

        public bool TryValidateToken(string token, out ClaimsPrincipal principal)
        {
            try
            {
                (principal, _) = ValidateToken(token);
                return true;
            }
            catch
            {
                principal = null;
                return false;
            }
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(
                Convert.FromBase64String(_jwtOptions.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
                signingCredentials: credentials
            );

            return _tokenHandler.WriteToken(token);
        }
    }

    public class JwtOptions
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiryMinutes { get; set; }
    }
}
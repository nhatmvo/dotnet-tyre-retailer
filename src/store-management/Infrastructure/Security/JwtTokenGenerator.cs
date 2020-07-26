using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace store_management.Infrastructure.Security
{
    public interface IJwtTokenGenerator
    {
        public Task<string> CreateToken(ClaimsIdentity identity);
        public Task<ClaimsIdentity> GetClaimsIdentity(string username, string role);
    }

    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtIssuerOptions _jwtOptions;

        public JwtTokenGenerator(IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<string> CreateToken(ClaimsIdentity identity)
        {
            var claims = identity.Claims;

            var jwt = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims ,
                _jwtOptions.NotBefore,
                _jwtOptions.Expiration,
                _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }


        public async Task<ClaimsIdentity> GetClaimsIdentity(string username, string role)
        {
            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    new DateTimeOffset(_jwtOptions.IssuedAt).ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token");

            return claimsIdentity;
        }
    }
}

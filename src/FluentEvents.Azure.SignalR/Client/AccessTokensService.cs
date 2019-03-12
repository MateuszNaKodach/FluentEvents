using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FluentEvents.Azure.SignalR.Client
{
    internal class AccessTokensService : IAccessTokensService
    {
        private readonly JwtSecurityTokenHandler m_JwtTokenHandler = new JwtSecurityTokenHandler();
        private readonly string m_ServerName;

        public AccessTokensService()
        {
            m_ServerName = GenerateServerName();
        }

        private string GenerateServerName() => $"{Environment.MachineName}_{Guid.NewGuid():N}";

        public string GenerateAccessToken(
            ConnectionString connectionString, 
            string audience, 
            string nameIdentifier = null,
            TimeSpan? lifetime = null
        )
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, nameIdentifier ?? m_ServerName)
            };

            return GenerateAccessTokenInternal(connectionString, audience, claims, lifetime ?? TimeSpan.FromHours(1));
        }

        private string GenerateAccessTokenInternal(
            ConnectionString connectionString,
            string audience,
            IEnumerable<Claim> claims,
            TimeSpan lifetime
        )
        {
            var expire = DateTime.UtcNow.Add(lifetime);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(connectionString.AccessKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtSecurityToken;
            try
            {
                jwtSecurityToken = m_JwtTokenHandler.CreateJwtSecurityToken(
                    issuer: null,
                    audience: audience,
                    subject: claims == null ? null : new ClaimsIdentity(claims),
                    expires: expire,
                    signingCredentials: credentials
                );
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new InvalidConnectionStringAccessKeyException(e);
            }

            return m_JwtTokenHandler.WriteToken(jwtSecurityToken);
        }
    }
}

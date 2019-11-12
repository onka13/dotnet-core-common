using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace CoreCommon.Infra.Helpers
{
    public class AuthHelper
    {
        public static string HashPassword(string value, string salt, int iterationCount = 10000)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                                password: value,
                                salt: Encoding.UTF8.GetBytes(salt),
                                prf: KeyDerivationPrf.HMACSHA512,
                                iterationCount: iterationCount,
                                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valueBytes);
        }

        public static bool ValidateHash(string value, string salt, string hash)
        {
            return HashPassword(value, salt) == hash;
        }

        public static string CreateToken(string secretKey, int expiryInMinutes, object userData)
        {
            var serializedUserData = ConversionHelper.Serialize(userData);

            var tokenHandler = new JwtSecurityTokenHandler();
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.UserData, serializedUserData)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expiryInMinutes),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static T ValidateToken<T>(string token, string secretKey, out bool isExpired)
        {
            isExpired = false;
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            try
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                var principal = handler.ValidateToken(token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = false,
                    ValidateAudience = false
                }, out SecurityToken validToken);

                var validJwt = validToken as JwtSecurityToken;

                if (validJwt == null)
                {
                    return default(T);
                }

                //if (!validJwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.Ordinal))
                //{
                //    return default(T);
                //    //throw new ArgumentException($"Algorithm must be '{SecurityAlgorithms.HmacSha256}'");
                //}

                isExpired = validJwt.ValidTo < DateTime.UtcNow;
                var userData = validJwt.Claims.ToList().FirstOrDefault(x => x.Type == ClaimTypes.UserData)?.Value;
                if (string.IsNullOrWhiteSpace(userData))
                {
                    return default(T);
                }
                return ConversionHelper.Deserialize<T>(userData);
            }
            catch (SecurityTokenExpiredException)
            {
                isExpired = true;
            }
            catch (Exception ex)
            {
                // TODO: log exception
                //throw ex;
            }
            return default(T);
        }
    }
}

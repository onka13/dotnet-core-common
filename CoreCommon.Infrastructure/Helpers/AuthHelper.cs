using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace CoreCommon.Infrastructure.Helpers
{
    /// <summary>
    /// Authentication Helpers.
    /// </summary>
    public class AuthHelper
    {
        /// <summary>
        /// Performs key derivation using the PBKDF2 algorithm.
        /// </summary>
        /// <param name="value">The password from which to derive the key.</param>
        /// <param name="salt">The salt to be used during the key derivation process.</param>
        /// <param name="iterationCount">The number of iterations of the pseudo-random function to apply during the key derivation process.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Validates a hash.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="salt"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool ValidateHash(string value, string salt, string hash)
        {
            return HashPassword(value, salt) == hash;
        }

        /// <summary>
        /// Create a JWT token.
        /// </summary>
        /// <param name="secretKey"></param>
        /// <param name="expiryInMinutes"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static string CreateToken(string secretKey, int expiryInMinutes, object userData)
        {
            var serializedUserData = ConversionHelper.SerializeObject(userData);

            var tokenHandler = new JwtSecurityTokenHandler();
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.UserData, serializedUserData),
                }),
                Expires = DateTime.UtcNow.AddMinutes(expiryInMinutes),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Validates a JWT token.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token"></param>
        /// <param name="secretKey"></param>
        /// <param name="isExpired"></param>
        /// <returns></returns>
        public static T ValidateToken<T>(string token, string secretKey, out bool isExpired)
        {
            isExpired = false;
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            try
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                var principal = handler.ValidateToken(
                    token,
                    new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    },
                    out SecurityToken validToken);

                var validJwt = validToken as JwtSecurityToken;

                if (validJwt == null)
                {
                    return default;
                }

                // if (!validJwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.Ordinal))
                // {
                //    return default(T);
                //    //throw new ArgumentException($"Algorithm must be '{SecurityAlgorithms.HmacSha256}'");
                // }
                isExpired = validJwt.ValidTo < DateTime.UtcNow;
                var userData = validJwt.Claims.ToList().FirstOrDefault(x => x.Type == ClaimTypes.UserData)?.Value;
                if (string.IsNullOrWhiteSpace(userData))
                {
                    return default;
                }

                return ConversionHelper.DerializeObject<T>(userData);
            }
            catch (SecurityTokenExpiredException)
            {
                isExpired = true;
            }

            return default;
        }

        // https://samueleresca.net/2016/12/developing-token-authentication-using-asp-net-core/
        public static string EncryptTicket(string name, string secretKey, TimeSpan expiration, object userData)
        {
            var serializedUserData = ConversionHelper.SerializeObject(userData);

            DateTime issuedAt = DateTime.UtcNow;
            DateTime expires = DateTime.UtcNow.Add(expiration);

            // http://stackoverflow.com/questions/18223868/how-to-encrypt-jwt-security-token
            var tokenHandler = new JwtSecurityTokenHandler();

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, name),
                new Claim(JwtRegisteredClaimNames.Nonce, serializedUserData),
            });
            var now = DateTime.UtcNow;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var securityKey2 = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var ep = new EncryptingCredentials(securityKey2, SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            // create the jwt
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Issuer = issuer,
                // Audience = issuer,
                Subject = claimsIdentity,
                Expires = expires,

                // NotBefore = issuedAt,
                // IssuedAt = issuedAt,
                SigningCredentials = signingCredentials,

                // EncryptingCredentials = ep
            };

            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

            // var token = tokenHandler.CreateToken(tokenDescriptor);

            // var token2 = new JwtSecurityToken(issuer,
            //              issuer,
            //              claimsIdentity.Claims,
            //              expires: DateTime.Now.AddMinutes(30),
            //              signingCredentials: signingCredentials);
            return tokenHandler.WriteToken(token);
        }

        public static T DecryptTicket<T>(string token, string secretKey, out bool isExpired)
        {
            isExpired = false;
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingKey2 = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            try
            {
                SecurityToken validToken;
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                TokenValidationParameters validationParameters = new TokenValidationParameters()
                {
                    // ValidAudience = issuer,
                    // ValidIssuer = issuer,
                    ValidateIssuerSigningKey = true,

                    // ValidateLifetime = true,
                    // LifetimeValidator = this.LifetimeValidator,
                    IssuerSigningKey = signingKey,

                    // TokenDecryptionKey = signingKey2,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };

                var principal = handler.ValidateToken(token, validationParameters, out validToken);

                var validJwt = validToken as JwtSecurityToken;

                if (validJwt == null)
                {
                    return default;
                }

                // if (!validJwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.Ordinal))
                // {
                //    return default(T);
                //    //throw new ArgumentException($"Algorithm must be '{SecurityAlgorithms.HmacSha256}'");
                // }
                isExpired = validJwt.ValidTo < DateTime.UtcNow;
                var userData = validJwt.Claims.ToList().FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Nonce)?.Value;
                if (string.IsNullOrWhiteSpace(userData))
                {
                    return default;
                }

                return ConversionHelper.Deserialize<T>(userData);
            }
            catch (SecurityTokenExpiredException)
            {
                isExpired = true;
            }
            catch
            {
                // ignored
            }

            return default;
        }

        public static T DecryptTicket<T>(string encryptedTicket, string secretKey)
        {
            bool isExpired;
            var data = DecryptTicket<T>(encryptedTicket, secretKey, out isExpired);
            return isExpired ? default : data;
        }

        public static string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public static string Sha256Hash(string text)
        {
            var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.Unicode.GetBytes(text));
            return Encoding.Unicode.GetString(hash);
        }

        public static string HashHmac(string secret, string message)
        {
            Encoding encoding = Encoding.UTF8;
            using (var hmac = new HMACSHA512(encoding.GetBytes(secret)))
            {
                var hash = hmac.ComputeHash(encoding.GetBytes(message));
                return BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);
            }
        }
    }
}

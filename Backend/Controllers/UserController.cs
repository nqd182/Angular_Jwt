using Jwt.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Jwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly AppSetting _appSetting;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(MyDbContext context, IOptions<AppSetting> otionsMonitor, UserManager<ApplicationUser> userManager) //  IOptions để lấy cấu hình đã đăng ký bằng Configure.
        {
            _context = context;
            _appSetting = otionsMonitor.Value;
            _userManager = userManager;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login (LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "Đăng nhập thất bại",
                    
                });
            }
            // cấp token
            var token = await GenerateToken(user);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Đăng nhập thành công",
                Data = token
            });
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var user = new ApplicationUser
            {
                HoTen = model.HoTen,
                UserName = model.UserName,
                Email = model.Email,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Đăng ký thành công",
                    Data = user
                });
            }
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = string.Join("; ", result.Errors.Select(e => e.Description))
            });
        }
        private async Task<TokenModel> GenerateToken(ApplicationUser user)        
        {
            // code tạo token   
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSetting.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // id cua jwt access token,
                    // role
                        
                    
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();
            // lưu refresh token vào db
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                JwtId = token.Id,
                IsUsed = false,
                IsRevoked = false,
                UserId = user.Id,
                Token = refreshToken,
                ExpiredAt = DateTime.UtcNow.AddHours(1)
            };
            await _context.RefreshTokens.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }
        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken(TokenModel model)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSetting.SecretKey);

            var tokenParam = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                ValidateLifetime = false // ko check exp để lấy dc claim ngay cả khi token hết hạn

            };
            try
            {
                // check 1 format token
                var tokenInVerification = jwtTokenHandler.ValidateToken(model.AccessToken, tokenParam, out var validatedToken); // token cần xác thực, điều kiện xác thực, token đã xác thực
                // check 2 check alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (result == false)
                    {
                        return Unauthorized(new ApiResponse
                        {
                            Success = false,
                            Message = "Invalid token",
                        });
                    }
                    // check 3 check exp nếu token chưa hết hạn thì ko cần cấp lại
                    var utcExp = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                    var expDate = ConvertUnixTimeToDateTime(utcExp);
                    if(expDate > DateTime.UtcNow)
                    {
                        return Unauthorized(new ApiResponse
                        {
                            Success = false,
                            Message = "Access token has not yet expired",
                        });
                    }
                    // check 4 check refresh token có tồn tại trong db ko
                    var storedRefreshToken = _context.RefreshTokens.FirstOrDefault(x => x.Token == model.RefreshToken);
                    if (storedRefreshToken == null)
                    {
                        return Unauthorized(new ApiResponse
                        {
                            Success = false,
                            Message = "Refresh token does not exist",
                        });
                    }
                    // check 5 check refresh token đã dc sử dụng hay thu hồi chưa
                    if (storedRefreshToken.IsUsed)
                    {
                        return Unauthorized(new ApiResponse
                        {
                            Success = false,
                            Message = "Refresh token has been used",
                        });
                    }
                    if (storedRefreshToken.IsRevoked)
                    {
                        return Unauthorized(new ApiResponse
                        {
                            Success = false,
                            Message = "Refresh token has been revoked",
                        });
                    }
                    // check 6 check xem id access token có khớp không
                    var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                    if (storedRefreshToken.JwtId != jti)
                    {
                        return Unauthorized(new ApiResponse
                        {
                            Success = false,
                            Message = "Token doesn't match",
                        });
                    }
                    // Update refresh token đã sử dụng
                    storedRefreshToken.IsUsed = true;
                    storedRefreshToken.IsRevoked = true;
                    _context.RefreshTokens.Update(storedRefreshToken);
                    await _context.SaveChangesAsync();
                    // cấp lại token
                    var user = await _userManager.FindByIdAsync(storedRefreshToken.UserId);
                    var token = await GenerateToken(user);
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Renew token thành công",
                        Data = token
                    });
                }
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid token",
                });
            }
            catch
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Something went wrong",
                });
            }
        }

        private DateTime ConvertUnixTimeToDateTime(long utcExp)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTimeInterval.AddSeconds(utcExp).ToUniversalTime();
        }


    }
}

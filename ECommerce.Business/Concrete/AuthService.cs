
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerce.Business.Abstract;
using ECommerce.Business.Configs;
using ECommerce.Business.DTOs;
using ECommerce.Business.DTOs.ResponseDtos;
using ECommerce.Entity.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;



namespace ECommerce.Business.Concrete;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly JwtConfig _jwtConfig;
    private readonly AppUrlConfig _appUrlConfig;

    public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, IOptions<JwtConfig> jwtConfig, IOptions<AppUrlConfig> appUrlConfig)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtConfig = jwtConfig.Value;
        _appUrlConfig = appUrlConfig.Value;
    }

    public async Task<ResponseDto<TokenDto>> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var appUser = await _userManager.FindByNameAsync(loginDto.UserNameOrEmail) ?? await _userManager.FindByEmailAsync(loginDto.UserNameOrEmail);
            if (appUser is null)
            {
                return ResponseDto<TokenDto>.Fail("Kullanıcı adı/Email hatalı", StatusCodes.Status400BadRequest);
            }
            // Eğer email mekanizmasını sisteme kattıysak, tam bu noktada email onaylı mı değil mi kontrolü yaparız.
            var isValidPassword = await _userManager.CheckPasswordAsync(appUser, loginDto.Password);
            if (!isValidPassword)
            {
                return ResponseDto<TokenDto>.Fail("Şifre hatalı", StatusCodes.Status400BadRequest);
            }
            // Token üret ve gönder
            var tokenDto = await GenerateJwtToken(appUser);
            return ResponseDto<TokenDto>.Success(tokenDto, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<TokenDto>.Fail($"HATA: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<AppUserDto>> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            var existsUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existsUser is not null)
            {
                return ResponseDto<AppUserDto>.Fail("Bu kullanıcı adı zaten mevcut!", StatusCodes.Status400BadRequest);
            }
            var existsEmailUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existsEmailUser is not null)
            {
                return ResponseDto<AppUserDto>.Fail("Bu email zaten mevcut!", StatusCodes.Status400BadRequest);
            }
            var appUser = new AppUser
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                EmailConfirmed = true
            };

            var resultCreateUser = await _userManager.CreateAsync(appUser, registerDto.Password);
            if (!resultCreateUser.Succeeded)
            {
                return ResponseDto<AppUserDto>.Fail("Kullanıcı oluşturulurken bir hata meydana geldi!", StatusCodes.Status500InternalServerError);
            }
            var resultAddToRole = await _userManager.AddToRoleAsync(appUser, "User");
            if (!resultAddToRole.Succeeded)
            {
                return ResponseDto<AppUserDto>.Fail("Kullanıcı rol atanırken bir hata meydana geldi!", StatusCodes.Status500InternalServerError);
            }
            // Email onayı, sistemimizde varsa; tam bu noktada kullanıcıya onay emaili göndeririz.
            var appUserDto = new AppUserDto
            {
                Id = appUser.Id,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                UserName = appUser.UserName,
                Email = appUser.Email,
                EmailConfirmed = appUser.EmailConfirmed
            };
            return ResponseDto<AppUserDto>.Success(appUserDto, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return ResponseDto<AppUserDto>.Fail($"HATA: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    private async Task<TokenDto> GenerateJwtToken(AppUser appUser)
    {
        try
        {
            // 1.Adım: Kullanıcının rollerini tespit et, rollerle birlikte kullanıcının diğer bilgilerini içerecek ve Token içine eklenecek claimleri yarat
            var roles = await _userManager.GetRolesAsync(appUser);  // admin, student
            var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier,appUser.Id),
            new (ClaimTypes.Name, appUser.UserName!),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        }.Union(roles.Select(x => new Claim(ClaimTypes.Role, x)));

            // 2.Adım: İmza için gerekli anahatar(key) değerini ve Credentials oluştur.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3.Adım: Token'ın geçerlilik süresini belirle
            var expiry = DateTime.Now.AddDays(_jwtConfig.AccessTokenExpiration);

            // 4.Adım: Token oluştur
            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: expiry,
                signingCredentials: credentials
            );
            var tokenDto = new TokenDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                AccessTokenExpirationDate = expiry
            };
            return tokenDto;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Token yaratılırken bir hata oluştu: {ex.Message}");
        }
    }
}

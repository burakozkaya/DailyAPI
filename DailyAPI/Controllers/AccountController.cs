using DailyAPI.Dto;
using DailyAPI.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DailyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> AccountRegister(RegisterDto registerDto)
        {
            var result = await _userManager.CreateAsync(new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            }, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);

            }

            _userManager.AddClaimAsync(await _userManager.FindByEmailAsync(registerDto.Email),
                new Claim("new_user", ""));
            return StatusCode(201);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> AccountLogin(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return BadRequest("Email or password is wrong");
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);
            if (!result.Succeeded)
            {
                return BadRequest("Email or password is wrong");
            }
            var claims = _userManager.GetClaimsAsync(user).Result.ToList();
            claims.Add(new(ClaimTypes.NameIdentifier, user.Id));
            return StatusCode(200, GenerateToken(claims));
        }

        [HttpGet("AddBalance")]
        [Authorize]
        public async Task<IActionResult> AccountAddBalance([FromBody] int amount)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (userId == null)
            {
                return BadRequest("User not found");
            }
            var user = await _userManager.FindByIdAsync(userId);
            user.Balance += amount;
            await _userManager.UpdateAsync(user);
            return StatusCode(200);
        }
        [HttpGet("newUserGiveAway")]
        [Authorize]
        public async Task<IActionResult> AccountGiveAway()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (userId == null)
            {
                return BadRequest("User not found");
            }
            var user = await _userManager.FindByIdAsync(userId);
            var userClaims = await _userManager.GetClaimsAsync(user);
            var claim = userClaims.First(x => x.Type == "new_user");
            await _userManager.RemoveClaimAsync(user, claim);
            userClaims.Add(new("PaidAccessPolicy", DateTime.Now.AddDays(100).ToString()));
            var newToken = GenerateToken(userClaims.ToList());
            return Ok(newToken);
        }
        private string GenerateToken(List<Claim> claims)
        {
            claims.Add(new Claim("new_user", "true"));
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Super secret token key"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokenOptions = new JwtSecurityToken(
                issuer: "https://localhost",
                audience: "https://localhost",
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: signinCredentials
                                          );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return tokenString;
        }
    }
}


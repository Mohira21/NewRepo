using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Task_and_Time_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();
        private readonly IConfiguration _configuration;
/*        private readonly IUserService _userService;*/

        public AuthController(IConfiguration configuration/*, IUserService userService*/)
        {
            _configuration = configuration;
            /*_userService = userService;*/
        }

        [HttpGet, Authorize]
        public ActionResult<string> GetMe()
        {
            /* var userName = _userService.GetMyName();
             return Ok(userName);*/

            var firstName = User?.Identity?.Name;
            var role = User.FindFirstValue(ClaimTypes.Role);
            return Ok(new { firstName, role });
        }

        [HttpPost("register for user")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
           /* CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);*/

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.Role = "user";
            /*            user.PasswordHash = passwordHash;
                        user.PasswordSalt = passwordSalt;*/

            return Ok(user);
        }
        [HttpPost("register for admin")]
        public async Task<ActionResult<User>> RegisterA(UserDto request)
        {
            /* CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);*/

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.Role = "Admin";
            /*            user.PasswordHash = passwordHash;
                        user.PasswordSalt = passwordSalt;*/

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            if (user.FirstName != request.FirstName)
                return BadRequest("User not found");

/*            if (!VerifyPasswordHAsh(request.Password, user.PasswordHash, user.PasswordSalt))
                return BadRequest("Wrong password");*/

            string token = CreateToken(user);

            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
/*        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHAsh(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }*/
    }
}

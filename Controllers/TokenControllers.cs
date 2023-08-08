using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tourism.Model;


namespace TravellAgent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {

        public IConfiguration _configuration;
        private readonly ADbContext _context;

        public TokenController(IConfiguration config, ADbContext context)
        {
            _configuration = config;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Tourism.DTO.Login _userdata)
        {
            if (_userdata != null && _userdata.UserEmail != null && _userdata.UserPassword != null)
            {
                var user = await GetUser(_userdata.UserEmail, _userdata.UserPassword);

                if (user != null)
                {
                    if (user.ApprovalStatus == false && user.Role=="TravelAgency")
                    {
                        return BadRequest("Travel agent is not approved yet.");
                    }

                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                         new Claim("UserId", user.UserId.ToString()),
                         new Claim("Email", user.UserEmail),
                        new Claim("Password",user.UserPassword),
                        new Claim("Name",user.UserName),
                        new Claim(ClaimTypes.Role, user.Role)

                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(60),
                        signingCredentials: signIn);

                    return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        private async Task<User> GetUser(string email, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == email && u.UserPassword == password);
        }

    }
}

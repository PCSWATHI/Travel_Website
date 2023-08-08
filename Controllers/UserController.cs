using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tourism.Model;
using Tourism.Repository; // Add this namespace

namespace Tourism.Model
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository; // Use the repository interface

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] User model)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(model.UserEmail);
            if (existingUser != null)
            {
                return BadRequest("User with the same email already exists");
            }

            await _userRepository.RegisterUserAsync(model);

            if (model.Role == "TravelAgency")
            {
                var travelagency = new Travelagencies
                {
                    TravelAgencyName = model.UserName,
                    TravelAgencyEmail = model.UserEmail,
                    TravelAgencyCountry = model.Country,
                    TravelAgencyState = model.State,
                    TravelAgencyCity = model.City,
                    TravelAgencyPhone = model.UserPhone,
                    TravelAgencyPassword = model.UserPassword,
                };

                await _userRepository.RegisterTravelAgencyAsync(travelagency);
            }

            return Ok("Registration successful");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                // Handle exceptions and logging here
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}

using System;
using System.Linq;
using System.Net;
using System.IO;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tourism.Model;
using System.Numerics;
using System.Security.Claims;
using System.Threading.Tasks; // Import this namespace for async Task support
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Tourism.Model
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToursController : ControllerBase
    {
        private readonly ADbContext _dbContext;

        public ToursController(ADbContext context)
        {
            _dbContext = context;
        }

        [HttpPost]

        public async Task<IActionResult> PostTour([FromForm] Tour createTourDto, IFormFile file)
        {
            var travelAgent = _dbContext.Travelagencies.FirstOrDefault(t => t.TraveAgencylId == createTourDto.TravelAgencyId);
            if (travelAgent == null)
            {
                return NotFound("TravelAgentId not found in the TravelAgent table.");
            }
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Please upload a valid image file.");
                }

                // Read the image data from the uploaded file.
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    var imageData = memoryStream.ToArray();

                    // Create a new instance of the Tour entity and set its properties.
                    var tour = new Tour
                    {
                        TravelAgencyId = createTourDto.TravelAgencyId,
                        TravelAgencyName = travelAgent.TravelAgencyName,
                        TourName = createTourDto.TourName,
                        TourCountry = createTourDto.TourCountry,
                        TourState = createTourDto.TourState,
                        TourCity = createTourDto.TourCity,
                        Itenary = createTourDto.Itenary,
                        Description = createTourDto.Description,
                        Type_of_package = createTourDto.Type_of_package,
                        AgencyImages = imageData,
                        PackagePrice = createTourDto.PackagePrice,
                        Extra_Person_Price = createTourDto.Extra_Person_Price
                    };

                    // Save the tour to the database.
                    _dbContext.Tours.Add(tour);
                    await _dbContext.SaveChangesAsync();

                    return Ok("Tour created successfully.");
                }
            }
            catch (Exception ex)
            {
                // If an error occurs during the creation process, you can return an HTTP 500 (Internal Server Error) response.
                // You should handle errors appropriately based on your application's needs.
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error occurred while creating the tour.");
            }
        }

        //public async Task<IActionResult> PostTour(Tour createTourDto, int userId)
        //{
        //    //var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
        //    //var roleClaim = claimsIdentity.FindFirst(ClaimTypes.Role);

        //    ////  Check if the user has the "Admin" role
        //    //if (roleClaim == null || roleClaim.Value != "TravelAgency")
        //    //{
        //    //    return Unauthorized("Only users with the 'TravelAgent' role can post  Tours");
        //    //}
        //    var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        //    if (user == null)
        //    {
        //        return NotFound("User not found.");
        //    }

        //    // Use the user's PhoneNumber to find the associated TravelAgent
        //    var travelAgent = await _dbContext.Travelagencies.FirstOrDefaultAsync(t => t.TravelAgencyPhone == user.UserPhone);
        //    if (travelAgent == null)
        //    {
        //        return NotFound("TravelAgent not found for the user.");
        //    }

        //    try
        //    {


        //        // Read the image data from the uploaded file.




        //        // Create a new instance of the Tour entity and set its properties.
        //        var tour = new Tour
        //        {
        //            TravelAgencyId = createTourDto.TravelAgencyId,
        //            TravelAgencyName = travelAgent.TravelAgencyName,
        //            TourName = createTourDto.TourName,
        //            TourCountry = createTourDto.TourCountry,
        //            TourState = createTourDto.TourState,
        //            TourCity = createTourDto.TourCity,
        //            Itenary = createTourDto.Itenary,
        //            Description = createTourDto.Description,
        //            Type_of_package = createTourDto.Type_of_package,
        //            AgencyImages = createTourDto.AgencyImages,
        //            PackagePrice = createTourDto.PackagePrice,
        //            Extra_Person_Price = createTourDto.Extra_Person_Price



        //        };

        //        // Save the tour to the database.
        //        _dbContext.Tours.Add(tour);
        //        await _dbContext.SaveChangesAsync();

        //        return Ok("Tour created successfully.");

        //    }
        //    catch (Exception ex)
        //    {
        //        // If an error occurs during the creation process, you can return an HTTP 500 (Internal Server Error) response.
        //        // You should handle errors appropriately based on your application's needs.
        //        return StatusCode((int)HttpStatusCode.InternalServerError, "Error occurred while creating the tour.");
        //    }
        //}



        //[HttpGet("GetToursByTravelAgent/{userId}")]
        //public async Task<IActionResult> GetToursByTravelAgent(int userId)
        //{
        //    var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        //    if (user == null)
        //    {
        //        return NotFound("User not found.");
        //    }

        //    // Use the user's PhoneNumber to find the associated TravelAgent
        //    var travelAgent = await _dbContext.Travelagencies.FirstOrDefaultAsync(t => t.TravelAgencyPhone == user.UserPhone);
        //    if (travelAgent == null)
        //    {
        //        return NotFound("TravelAgent not found for the user.");
        //    }
        //    var tours = _dbContext.Tours.Where(t => t.TravelAgencyId == travelAgent.TraveAgencylId).ToList();

        //    if (tours.Count == 0)
        //    {
        //        return NotFound("No tours found for the specified travel agent.");
        //    }

        //    return Ok(tours);
        //}
        [HttpGet("GetToursByTravelAgent/{userId}")]
        //[Authorize(Roles = "TravelAgency")]
        public async Task<IActionResult> GetToursByTravelAgent(int userId)
        {
            //var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            //var roleClaim = claimsIdentity.FindFirst(ClaimTypes.Role);

            ////  Check if the user has the "Admin" role
            //if (roleClaim == null || roleClaim.Value != "TravelAgency")
            //{
            //    return Unauthorized("Only users with the 'TravelAgent' role can view Tours");
            //}

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Use the user's PhoneNumber to find the associated TravelAgent
            var travelAgent = await _dbContext.Travelagencies.FirstOrDefaultAsync(t => t.TravelAgencyPhone == user.UserPhone);
            if (travelAgent == null)
            {
                return NotFound("TravelAgent not found for the user.");
            }
            var tours = _dbContext.Tours.Where(t => t.TravelAgencyId == travelAgent.TraveAgencylId).ToList();

            if (tours.Count == 0)
            {
                return NotFound("No tours found for the specified travel agent.");
            }

            return Ok(tours);
        }


            [HttpPut]
        //[Authorize(Roles = "TravelAgency")]
        public async Task<IActionResult> UpdateTour(int tourId, string fieldname, [FromForm] object updatedvalue, IFormFile file)
        {
            //var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            //var roleClaim = claimsIdentity.FindFirst(ClaimTypes.Role);

            ////  Check if the user has the "Admin" role
            //if (roleClaim == null || roleClaim.Value != "TravelAgency")
            //{
            //    return Unauthorized("Only users with the 'Admin' role canupdate tours");
            //}
            var tour = await _dbContext.Tours.FirstOrDefaultAsync(d => d.TourId == tourId);

            if (tour == null)
            {
                return BadRequest("Tour does not exist");
            }
            if (fieldname == "TourImage")
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Invalid file or empty file");
                }

                // Read the uploaded image into a byte array
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    var imageData = memoryStream.ToArray();
                }
            }
            else
            {

                PropertyInfo property = typeof(Tour).GetProperty(fieldname, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                {
                    return BadRequest("Invalid field name");
                }

                property.SetValue(tour, Convert.ChangeType(updatedvalue, property.PropertyType));




            }
            await _dbContext.SaveChangesAsync();

            return Ok("Tour details updated successfully");




        }

        //travelagency get booked tours
        [HttpGet("getbookedtoursfortravelagent/{userId}")]
        //[Authorize(Roles = "TravelAgency")]
        public async Task<ActionResult<dynamic>> GetBookedToursForTravelAgent(int userId)
        {
            //var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            //var roleClaim = claimsIdentity.FindFirst(ClaimTypes.Role);

            ////  Check if the user has the "Admin" role
            //if (roleClaim == null || roleClaim.Value != "TravelAgency")
            //{
            //    return Unauthorized("Only users with the 'TravelAGent' role can view Booked Tours");
            //}
            // Step 1: Fetch travel agent id by user id (based on matching phone number)
            var agent = await _dbContext.Travelagencies
                .FirstOrDefaultAsync(a => a.TravelAgencyPhone == _dbContext.Users
                    .Where(u => u.UserId == userId)
                    .Select(u => u.UserPhone)
                    .FirstOrDefault());

            if (agent == null)
            {
                return NotFound("Travel Agent not found");
            }

            // Step 2: Get all tours of the travel agent
            var agentTours = await _dbContext.Tours
                .Where(tour => tour.TravelAgencyId == agent.TraveAgencylId)
                .ToListAsync();

            // Step 3: Get tour details and user details for booked tours
            var bookedToursDetails = await (
                from booking in _dbContext.Bookings
                join tour in _dbContext.Tours
                    on booking.TourId equals tour.TourId
                join user in _dbContext.Users
                    on booking.UserId equals user.UserId
                where tour.TravelAgencyId == agent.TraveAgencylId
                select new
                {
                    Tour = new
                    {
                        tour.TourName,
                        tour.Description,
                        tour.Type_of_package,
                        tour.PackagePrice,
                        tour.AgencyImages
                    },
                    User = new
                    {
                        user.UserName,
                        user.UserEmail,
                        user.UserPhone,
                        user.Country,
                        user.State,
                        user.City
                    }
                }
            ).ToListAsync();

            return Ok(bookedToursDetails);
        }

      
    }
}



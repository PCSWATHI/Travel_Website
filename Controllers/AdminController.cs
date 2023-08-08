using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tourism.Model;
using Tourism.DTO;

namespace Tourism.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly ADbContext _dbContext;

        public AdminController(ADbContext dbContext)
        {
            _dbContext = dbContext;
        }
     

        [HttpPost("upload")]
       

        public async Task<IActionResult> UploadImage(IFormFile file)
        {
             
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

           
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var imageData = memoryStream.ToArray();

                var imageModel = new Admin
                {
                    Images= imageData,
                 
                };

                
                _dbContext.Adminis.Add(imageModel);
                await _dbContext.SaveChangesAsync();

                return Ok("Image uploaded successfully.");
            }
        }
       

        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            try
            {
                var imageModel = await _dbContext.Adminis.FirstOrDefaultAsync(a => a.IId == id);

                if (imageModel == null)
                {
                    return NotFound("Image not found.");
                }

                byte[] imageData = imageModel.Images;

                // Return the image as a file stream
                return File(imageData, "image/jpeg"); // Adjust the content type according to your image type (e.g., "image/png", "image/gif", etc.).
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an appropriate response.
                return StatusCode(500, "Error fetching image: " + ex.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllImages()
        {
            try
            {
                
                var imageModels = await _dbContext.Adminis.ToListAsync();

                if (imageModels.Count == 0)
                {
                    return NotFound("No images found.");
                }

         
                var imageUrls = imageModels.Select(imageModel => imageModel.Images).ToList();

                return Ok(imageUrls);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Error fetching images: " + ex.Message);
            }
        }

       

        [HttpPost("Users/Approval")]
       
        public async Task<IActionResult> SetApprovalStatus(int UserId)
        {
           
            var travelagency = await _dbContext.Users.FindAsync(UserId);
            if (travelagency == null)
            {
                return NotFound(" TravelAgency not Found");
            }

            travelagency.ApprovalStatus = true;
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        

        [HttpPost("agentactive/{agentid}/setactive")]


        public async Task<IActionResult> SetAgentActive(int agentid)
        {
           


            var agent = await _dbContext.Travelagencies.FindAsync(agentid);
            if (agent == null)
            {
                return NotFound("Agent not found");
            }

            var userWithSamePhoneNumber = await _dbContext.Users
        .FirstOrDefaultAsync(u => u.UserPhone == agent.TravelAgencyPhone);

            // If no user with the same phone number found or the approval status is false, return an error
            if (userWithSamePhoneNumber == null || !userWithSamePhoneNumber.ApprovalStatus)
            {
                return BadRequest("User not found or approval status is false");
            }

            agent.ActiveStatus = true;
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        [HttpPost("agentactive/{agentid}/setinactive")]
       
        public async Task<IActionResult> SetAgentInactive(int agentid)
        {
          
            var agent = await _dbContext.Travelagencies.FindAsync(agentid);
            if (agent == null)
            {
                return NotFound("Agent not found");
            }

            var userWithSamePhoneNumber = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.UserPhone == agent.TravelAgencyPhone);

            // If no user with the same phone number found or the approval status is false, return an error
            if (userWithSamePhoneNumber == null || !userWithSamePhoneNumber.ApprovalStatus)
            {
                return BadRequest("User not found or approval status is false");
            }

            agent.ActiveStatus = false; // Set active status to false (inactive)
            await _dbContext.SaveChangesAsync();

            return Ok();
        }


        [HttpGet("GetTravelAgenciesPendingApproval")]
       
        public IActionResult GetTravelAgenciesPendingApproval()
        {
           
            var travelAgencies = _dbContext.Users
                .Where(user => user.Role == "TravelAgency" && user.ApprovalStatus == false)
                .ToList();

            return Ok(travelAgencies);
        }
        //get of travel Agencies

        [HttpGet("GetTravelAgenciesForApproval")]
    
        public IActionResult GetTravelAgenciesForApproval()
        {
      
            var travelAgencies = _dbContext.Users
                .Where(user => user.Role == "TravelAgency")
                .ToList();

            return Ok(travelAgencies);
        }
     
        [HttpPost("Users/DisApproval")]
      
        
        public async Task<IActionResult> SetDisApprovalStatus(int UserId)
        {
            
            
            var travelagency = await _dbContext.Users.FindAsync(UserId);
            if (travelagency == null)
            {
                return NotFound(" TravelAgency not Found");
            }

            travelagency.ApprovalStatus = false;
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("GetTravelAgenciesActivationStatus")]
       
        public IActionResult GetTravelAgenciesActivationStatus()
        {

           
            var travelAgencies = _dbContext.Travelagencies.ToList();



            return Ok(travelAgencies);
        }

    }
   
}


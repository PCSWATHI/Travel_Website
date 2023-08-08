using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using Tourism.Model;
namespace Tourism.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingControllers : ControllerBase
    {
        private readonly ADbContext _dbContext;

        public BookingControllers(ADbContext context)
        {
            _dbContext = context;
        }

        [HttpPost("{tourId}/{userId}")]
       
        public IActionResult BookTour(int tourId, int userId, int noofextrapersons, int packagequantity, int noofpersonsfrompackage)
        {
           
            try
            {
                var tour = _dbContext.Tours.FirstOrDefault(u => u.TourId == tourId);
                if (tour == null)
                {
                    return BadRequest("Tour does not exist");
                }

                var price = (packagequantity * tour.PackagePrice) + (noofextrapersons * tour.Extra_Person_Price);


                var booking = new Booking
                {
                    TourId = tourId,
                    UserId = userId,
                    Price = price,
                    No_Of_Persons = (noofpersonsfrompackage *packagequantity) + noofextrapersons
                };

                _dbContext.Bookings.Add(booking);
                _dbContext.SaveChanges();

                return Ok(booking);
            }
            catch
            {
                return BadRequest("An error occurred while adding the patient.");
            }
        }
       
        [HttpGet("Booked")]
       
        public async Task<ActionResult<IEnumerable<Tour>>> GetBookedTours(int userId)
        {
            var bookedTours = await _dbContext.Tours
                .Where(t => _dbContext.Bookings.Any(b => b.TourId == t.TourId && b.UserId == userId))
                .ToListAsync();

            return Ok(bookedTours);
        }

        [HttpGet("notbookedtours/{userId}")]

        public async Task<ActionResult<IEnumerable<Tour>>> GetNotBookedTours(int userId)
        {

            var unbookedTours = await _dbContext.Tours
    .Where(t => !_dbContext.Bookings.Any(b => b.TourId == t.TourId && b.UserId == userId)) // Filter unbooked tours
    .Where(t => _dbContext.Travelagencies.Any(a => a.TraveAgencylId == t.TravelAgencyId && a.ActiveStatus)) // Filter tours of active travel agents
    .ToListAsync();

            return Ok(unbookedTours);
        }



      



    }
}

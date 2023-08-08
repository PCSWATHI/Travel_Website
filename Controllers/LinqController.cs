using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Tourism.Model;

namespace Tourism.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinqController : ControllerBase


    {
       // Assuming ApplicationUser is your user model
        private readonly ADbContext _dbContext;

        public LinqController(ADbContext context)
        {
            _dbContext = context;
        }

        [HttpGet("filter")]
        public IActionResult FilterToursByPackageType(string packageType)
        {
            // Get all tours from the database using the _dbContext
            IEnumerable<Tour> allTours = _dbContext.Tours;

            // Filter tours based on the package type criteria
            IEnumerable<Tour> filteredTours = allTours.Where(tour => tour.Type_of_package == packageType);

            // Return the filtered tours
            return Ok(filteredTours);
        }


        // ... (other actions)


        [HttpGet("GetUniqueStates")]
        public async Task<ActionResult<IEnumerable<string>>> GetUniqueStates()
        {
            var uniqueStates = await _dbContext.Tours.Select(t => t.TourState).Distinct().ToListAsync();
            return Ok(uniqueStates);
        }


        
        [HttpGet("GetUniqueCountries")]
        public async Task<ActionResult<IEnumerable<string>>> GetUniqueCountries()
        {
            var uniqueCountries = await _dbContext.Tours.Select(t => t.TourCountry).Distinct().ToListAsync();
            return Ok(uniqueCountries);
        }


        // GET: api/Tour/GetUniquePackagePrices
        [HttpGet("GetUniqueCities")]
        public async Task<ActionResult<IEnumerable<string>>> GetUniqueCities()
        {
            var uniqueCities = await _dbContext.Tours.Select(t => t.TourCity).Distinct().ToListAsync();
            return Ok(uniqueCities);
        }

        [HttpGet("GetTours")]
        public async Task<ActionResult<IEnumerable<object>>> GetTours(
            string typeOfPackage, string? country, string? state, string? city, int? minPrice, int? maxPrice, int userId)
        {
            IQueryable<Tour> query = _dbContext.Tours;

            // Filter tours based on the provided arguments
            if (!string.IsNullOrEmpty(typeOfPackage))
            {
                query = query.Where(t => t.Type_of_package == typeOfPackage);
            }

            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(t => t.TourCountry == country);
            }

            if (!string.IsNullOrEmpty(state))
            {
                query = query.Where(t => t.TourState == state);
            }

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(t => t.TourCity == city);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(t => t.PackagePrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(t => t.PackagePrice <= maxPrice.Value);
            }

            // Left Join Tour with Booking to get tour details along with booking details for the specified user
            var result = await query.GroupJoin(
                _dbContext.Bookings.Where(b => b.UserId == userId), // Filter bookings for the specified user
                tour => tour.TourId,
                booking => booking.TourId,
                (tour, bookings) => new
                {
                    Tour = tour,
                    Bookings = bookings
                })
                .SelectMany(
                    x => x.Bookings.DefaultIfEmpty(), // Perform Left Join
                    (tourWithBookings, booking) => new
                    {
                        TourId = tourWithBookings.Tour.TourId,
                        TravelAgentId = tourWithBookings.Tour.TravelAgencyId,
                        TravelAgentName = tourWithBookings.Tour.TravelAgencyName,
                        TourName = tourWithBookings.Tour.TourName,
                        Description = tourWithBookings.Tour.Description,
                        Type_Of_Package = tourWithBookings.Tour.Type_of_package,
                        Country = tourWithBookings.Tour.TourCountry,
                        State = tourWithBookings.Tour.TourState,
                        City = tourWithBookings.Tour.TourCity,
                        PackagePrice = tourWithBookings.Tour.PackagePrice,
                        ExtraPersonPrice = tourWithBookings.Tour.Extra_Person_Price,
                        Itenary = tourWithBookings.Tour.Itenary,
                        TourImage = tourWithBookings.Tour.AgencyImages,
                        AgentName = tourWithBookings.Tour.TravelAgencyName,
                        AgentCountry = tourWithBookings.Tour.TourCountry,
                        AgentState = tourWithBookings.Tour.TourState,
                        AgentCity = tourWithBookings.Tour.TourCity,
                        IsBooked = booking != null // Determine if the tour is booked or not
                    })
                .Join(
                    _dbContext.Travelagencies.Where(agent => agent.ActiveStatus == true), // Filter active travel agents
                    tour => tour.TravelAgentId,
                    agent => agent.TraveAgencylId,
                    (tour, agent) => new
                    {
                        tour.TourId,
                        tour.TravelAgentId,
                        tour.TravelAgentName,
                        tour.TourName,
                        tour.Description,
                        tour.Type_Of_Package,
                        tour.Country,
                        tour.State,
                        tour.City,
                        tour.PackagePrice,
                        tour.ExtraPersonPrice,
                        tour.Itenary,
                        tour.TourImage,
                        tour.AgentName,
                        tour.AgentCountry,
                        tour.AgentState,
                        tour.AgentCity,
                        tour.IsBooked
                    })
                .Where(x => !x.IsBooked) // Exclude booked tours
                .ToListAsync();

            return Ok(result);
        }



    }








}

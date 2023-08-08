using System.ComponentModel.DataAnnotations;

namespace Tourism.Model
{
    public class Travelagencies
    {
        [Key]
        public int TraveAgencylId { get; set; }

        public string? TravelAgencyName { get; set; }

        public string? TravelAgencyEmail { get; set; }

        public string? TravelAgencyCountry { get; set; }

        public string? TravelAgencyState { get; set; }

        public string? TravelAgencyCity { get; set; }
        public string? TravelAgencyPhone { get; set; }

        public string? TravelAgencyPassword { get; set; }

        public bool ActiveStatus { get; set; } = false;
       public ICollection<Tour>? Tours { get; set; }
    }
}

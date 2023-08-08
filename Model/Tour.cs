using System.ComponentModel.DataAnnotations.Schema;

namespace Tourism.Model
{
    public class Tour
    {
        public int TourId { get; set; }

        [ForeignKey("Travelagencies")]
        public int TravelAgencyId { get; set; }
        public string? TravelAgencyName { get; set; }

        public string? TourName { get; set; }

        public string? TourCountry { get; set; }

        public string ? TourState { get; set; }

        public string? TourCity { get; set; }

        public string? Itenary { get; set; }
        public string? Description { get; set;}

        public string? Type_of_package { get; set; }

      
        public int PackagePrice { get; set; }

        public int Extra_Person_Price { get; set; }

        public byte[]? AgencyImages { get; set; }

        

  
    }
}

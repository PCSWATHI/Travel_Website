using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tourism.Model
{
    public class Booking
    {
        [ForeignKey("Tour")]
        public int TourId { get; set; }

        [ForeignKey("User")]

        public int UserId { get; set; }


        public int Price { get; set; }


        [Key]
        public int BookingId { get; set; }

        public int No_Of_Persons { get; set; }

    }
}

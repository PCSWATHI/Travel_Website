using System.ComponentModel.DataAnnotations;

namespace Tourism.Model
{
    public class Admin
    {
        [Key]
        public  int IId { get; set; }
        public byte[]? Images { get; set; }
    }
}

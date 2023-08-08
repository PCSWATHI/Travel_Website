using System.ComponentModel.DataAnnotations;

namespace Tourism.Model
{
    public class User
    {
        [Key]
       public int UserId { get; set; }

        public string? UserName { get; set; }

        public string? UserEmail { get; set;}

        public string? Country { get; set; }

        public string? State { get; set; }

        public string? City { get; set; }
        public string? UserPhone { get; set;}

        public string? UserPassword { get; set;}

        public string? Role { get; set;}

        public bool ApprovalStatus { get; set; } = false;
    }
}

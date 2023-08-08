using System.Threading.Tasks;
using Tourism.Model;

namespace Tourism.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task RegisterUserAsync(User user);
        Task RegisterTravelAgencyAsync(Travelagencies travelAgency);
        Task<User> GetUserByIdAsync(int userId);
    }
}
